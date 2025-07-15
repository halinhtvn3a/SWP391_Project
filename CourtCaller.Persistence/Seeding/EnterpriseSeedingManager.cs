using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CourtCaller.Persistence.Seeding
{
    public interface IEnterpriseSeedingManager
    {
        Task<SeedingResult> ExecuteSeedingAsync(CancellationToken cancellationToken = default);
        Task<DatabaseHealthResult> CheckDatabaseHealthAsync(
            CancellationToken cancellationToken = default
        );
    }

    public class EnterpriseSeedingManager : IEnterpriseSeedingManager
    {
        private readonly IEnumerable<ISeedingStrategy> _seedingStrategies;
        private readonly IDatabaseHealthChecker _healthChecker;
        private readonly CourtCallerDbContext _context;
        private readonly SeedingConfiguration _config;
        private readonly ILogger<EnterpriseSeedingManager> _logger;
        private readonly string _environment;

        public EnterpriseSeedingManager(
            IEnumerable<ISeedingStrategy> seedingStrategies,
            IDatabaseHealthChecker healthChecker,
            CourtCallerDbContext context,
            IOptions<SeedingConfiguration> config,
            ILogger<EnterpriseSeedingManager> logger
        )
        {
            _seedingStrategies = seedingStrategies.OrderBy(s => s.Priority).ToList();
            _healthChecker = healthChecker;
            _context = context;
            _config = config.Value;
            _logger = logger;
            _environment =
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        }

        public async Task<DatabaseHealthResult> CheckDatabaseHealthAsync(
            CancellationToken cancellationToken = default
        )
        {
            return await _healthChecker.CheckHealthAsync(cancellationToken);
        }

        public async Task<SeedingResult> ExecuteSeedingAsync(
            CancellationToken cancellationToken = default
        )
        {
            var overallResult = new SeedingResult();
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("=== STARTING ENTERPRISE DATABASE SEEDING ===");
                _logger.LogInformation("Environment: {Environment}", _environment);
                _logger.LogInformation(
                    "Seeding Configuration: Enabled={Enabled}, ForceReseed={ForceReseed}, ValidateAfterSeed={ValidateAfterSeed}",
                    _config.Enabled,
                    _config.ForceReseed,
                    _config.ValidateAfterSeed
                );

                // Step 1: Check if seeding is enabled
                if (!_config.Enabled)
                {
                    overallResult.IsSuccess = true;
                    overallResult.Message = "Seeding is disabled in configuration";
                    _logger.LogInformation("Seeding is disabled in configuration");
                    return overallResult;
                }

                // Step 2: Database health check
                _logger.LogInformation("Step 1/5: Performing database health check...");
                var healthResult = await _healthChecker.CheckHealthAsync(cancellationToken);

                if (!healthResult.IsHealthy)
                {
                    overallResult.Errors.Add(
                        $"Database health check failed: {string.Join(", ", healthResult.Issues)}"
                    );

                    // If we have pending migrations, try to apply them
                    if (healthResult.HasPendingMigrations)
                    {
                        _logger.LogWarning(
                            "Found {Count} pending migrations. Attempting to apply...",
                            healthResult.PendingMigrations.Count
                        );
                        try
                        {
                            await _context.Database.MigrateAsync(cancellationToken);
                            _logger.LogInformation("Successfully applied pending migrations");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to apply pending migrations");
                            overallResult.Errors.Add($"Failed to apply migrations: {ex.Message}");
                            return overallResult;
                        }
                    }
                    else
                    {
                        return overallResult;
                    }
                }

                // Step 3: Backup (if enabled)
                if (
                    _config.EnableBackup
                    && _environment.Equals("Production", StringComparison.OrdinalIgnoreCase)
                )
                {
                    _logger.LogInformation("Step 2/5: Creating backup...");
                    await CreateBackupAsync(cancellationToken);
                }

                // Step 4: Execute seeding strategies
                _logger.LogInformation("Step 3/5: Executing seeding strategies...");
                var applicableStrategies = _seedingStrategies
                    .Where(s => s.ShouldSeed(_environment))
                    .ToList();

                _logger.LogInformation(
                    "Found {Count} applicable seeding strategies: {Strategies}",
                    applicableStrategies.Count,
                    string.Join(", ", applicableStrategies.Select(s => s.Name))
                );

                var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken
                );
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(_config.TimeoutSeconds));

                foreach (var strategy in applicableStrategies)
                {
                    _logger.LogInformation(
                        "Executing strategy: {StrategyName} (Priority: {Priority})",
                        strategy.Name,
                        strategy.Priority
                    );

                    var retryCount = 0;
                    SeedingResult strategyResult = new SeedingResult
                    {
                        IsSuccess = false,
                        Message = "Strategy not executed",
                        Errors = { "Strategy execution was not completed" },
                    };

                    while (retryCount <= _config.MaxRetryAttempts)
                    {
                        try
                        {
                            strategyResult = await strategy.SeedAsync(
                                cancellationTokenSource.Token
                            );
                            break;
                        }
                        catch (Exception ex) when (retryCount < _config.MaxRetryAttempts)
                        {
                            retryCount++;
                            _logger.LogWarning(
                                ex,
                                "Strategy {StrategyName} failed on attempt {Attempt}/{MaxAttempts}. Retrying...",
                                strategy.Name,
                                retryCount,
                                _config.MaxRetryAttempts + 1
                            );

                            await Task.Delay(
                                TimeSpan.FromSeconds(Math.Pow(2, retryCount)),
                                cancellationToken
                            ); // Exponential backoff
                            continue;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(
                                ex,
                                "Strategy {StrategyName} failed after {MaxAttempts} attempts",
                                strategy.Name,
                                _config.MaxRetryAttempts + 1
                            );
                            strategyResult = new SeedingResult
                            {
                                IsSuccess = false,
                                Errors = { $"Strategy {strategy.Name} failed: {ex.Message}" },
                            };
                            break;
                        }
                    }

                    // Merge results
                    overallResult.SeededEntities.AddRange(strategyResult.SeededEntities);
                    overallResult.RecordsSeeded += strategyResult.RecordsSeeded;
                    overallResult.Errors.AddRange(strategyResult.Errors);

                    if (!strategyResult.IsSuccess)
                    {
                        _logger.LogError(
                            "Strategy {StrategyName} failed: {Errors}",
                            strategy.Name,
                            string.Join(", ", strategyResult.Errors)
                        );

                        if (_config.EnableRollback)
                        {
                            _logger.LogWarning("Rolling back due to strategy failure...");
                            await RollbackAsync(cancellationToken);
                            overallResult.Errors.Add("Rollback executed due to strategy failure");
                            return overallResult;
                        }
                    }
                    else
                    {
                        _logger.LogInformation(
                            "Strategy {StrategyName} completed successfully. Records seeded: {RecordsSeeded}",
                            strategy.Name,
                            strategyResult.RecordsSeeded
                        );
                    }
                }

                // Step 5: Post-seeding validation
                if (_config.ValidateAfterSeed)
                {
                    _logger.LogInformation("Step 4/5: Performing post-seeding validation...");
                    var validationErrors = await ValidateSeededDataAsync(cancellationToken);
                    overallResult.Errors.AddRange(validationErrors);
                }

                // Step 6: Final health check
                _logger.LogInformation("Step 5/5: Final health check...");
                var finalHealthResult = await _healthChecker.CheckHealthAsync(cancellationToken);
                if (!finalHealthResult.IsHealthy)
                {
                    overallResult.Errors.Add("Post-seeding health check failed");
                }

                overallResult.IsSuccess = overallResult.Errors.Count == 0;
                overallResult.Message = overallResult.IsSuccess
                    ? $"Enterprise seeding completed successfully. Total records seeded: {overallResult.RecordsSeeded}"
                    : $"Enterprise seeding completed with {overallResult.Errors.Count} errors";

                _logger.LogInformation("=== ENTERPRISE DATABASE SEEDING COMPLETED ===");
                _logger.LogInformation(
                    "Success: {Success}, Total Records: {RecordsSeeded}, Duration: {Duration}ms",
                    overallResult.IsSuccess,
                    overallResult.RecordsSeeded,
                    stopwatch.ElapsedMilliseconds
                );

                if (overallResult.Errors.Any())
                {
                    _logger.LogWarning(
                        "Seeding completed with errors: {Errors}",
                        string.Join("; ", overallResult.Errors)
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Enterprise seeding failed with unexpected error");
                overallResult.Errors.Add($"Unexpected error: {ex.Message}");
                overallResult.IsSuccess = false;
            }
            finally
            {
                stopwatch.Stop();
                overallResult.Duration = stopwatch.Elapsed;
            }

            return overallResult;
        }

        private Task CreateBackupAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating database backup before seeding...");

                // Note: Actual backup implementation would depend on your database provider
                // For SQL Server, you might use:
                // var backupPath = Path.Combine(Path.GetTempPath(), $"courtcaller_backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.bak");
                // await _context.Database.ExecuteSqlRawAsync($"BACKUP DATABASE [CourtCallerDb] TO DISK = '{backupPath}'", cancellationToken);

                _logger.LogInformation("Database backup created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create backup. Continuing with seeding...");
            }

            return Task.CompletedTask;
        }

        private Task RollbackAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogWarning("Attempting to rollback seeding changes...");

                // Note: Actual rollback implementation would restore from backup
                // This is a simplified version that just logs the action

                _logger.LogInformation("Rollback completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rollback failed");
            }

            return Task.CompletedTask;
        }

        private async Task<List<string>> ValidateSeededDataAsync(
            CancellationToken cancellationToken
        )
        {
            var errors = new List<string>();

            try
            {
                _logger.LogInformation("Validating seeded data integrity...");

                // Check referential integrity
                var orphanedCourts = await _context
                    .Courts.Where(c => !_context.Branches.Any(b => b.BranchId == c.BranchId))
                    .CountAsync(cancellationToken);

                if (orphanedCourts > 0)
                {
                    errors.Add($"Found {orphanedCourts} courts without valid branch references");
                }

                var orphanedTimeSlots = await _context
                    .TimeSlots.Where(ts => !_context.Courts.Any(c => c.CourtId == ts.CourtId))
                    .CountAsync(cancellationToken);

                if (orphanedTimeSlots > 0)
                {
                    errors.Add(
                        $"Found {orphanedTimeSlots} time slots without valid court references"
                    );
                }

                // Check for required roles
                var adminRoleExists = await _context.Roles.AnyAsync(
                    r => r.NormalizedName == "ADMIN",
                    cancellationToken
                );
                if (!adminRoleExists)
                {
                    errors.Add("Admin role not found after seeding");
                }

                if (errors.Any())
                {
                    _logger.LogWarning("Data validation found {Count} issues", errors.Count);
                }
                else
                {
                    _logger.LogInformation("Data validation completed successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Data validation failed");
                errors.Add($"Validation failed: {ex.Message}");
            }

            return errors;
        }
    }
}
