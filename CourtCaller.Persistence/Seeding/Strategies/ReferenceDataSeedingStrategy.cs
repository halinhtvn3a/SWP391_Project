using System.Diagnostics;
using BuildingBlocks.Abstractions.Files;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CourtCaller.Persistence.Seeding.Strategies
{
    public class ReferenceDataSeedingStrategy : ISeedingStrategy
    {
        private readonly CourtCallerDbContext _context;
        private readonly IFileReader _fileReader;
        private readonly ILogger<ReferenceDataSeedingStrategy> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly SeedingConfiguration _config;
        private readonly string _rootPath;

        public string Name => "Reference Data";
        public int Priority => 2;

        public ReferenceDataSeedingStrategy(
            CourtCallerDbContext context,
            IFileReader fileReader,
            ILogger<ReferenceDataSeedingStrategy> logger,
            ILoggerFactory loggerFactory,
            SeedingConfiguration config,
            string rootPath
        )
        {
            _context = context;
            _fileReader = fileReader;
            _logger = logger;
            _loggerFactory = loggerFactory;
            _config = config;
            _rootPath = rootPath;
        }

        public bool ShouldSeed(string environment)
        {
            var settings = _config.GetEnvironmentSettings(environment);
            return settings.SeedReferenceData
                && !settings.ExcludedEntities.Contains("ReferenceData");
        }

        public async Task<SeedingResult> SeedAsync(CancellationToken cancellationToken = default)
        {
            var result = new SeedingResult();
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Starting reference data seeding...");

                // Define seeding order based on dependencies
                var seedingTasks = new (string EntityName, Func<Task> SeedingTask)[]
                {
                    (
                        "Branches",
                        () =>
                            SeedEntityAsync<Branch>(
                                AppCts.SeederRelativePath.BranchPath,
                                result,
                                cancellationToken
                            )
                    ),
                    (
                        "Courts",
                        () =>
                            SeedEntityAsync<Court>(
                                AppCts.SeederRelativePath.CourtPath,
                                result,
                                cancellationToken
                            )
                    ),
                    (
                        "Prices",
                        () =>
                            SeedEntityAsync<Price>(
                                AppCts.SeederRelativePath.PricePath,
                                result,
                                cancellationToken
                            )
                    ),
                    (
                        "News",
                        () =>
                            SeedEntityAsync<News>(
                                AppCts.SeederRelativePath.NewsPath,
                                result,
                                cancellationToken
                            )
                    ),
                };

                foreach (var (entityName, seedingTask) in seedingTasks)
                {
                    var settings = _config.GetEnvironmentSettings(
                        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                            ?? "Development"
                    );

                    if (settings.ExcludedEntities.Contains(entityName))
                    {
                        _logger.LogInformation(
                            "Skipping {EntityName} - excluded in environment settings",
                            entityName
                        );
                        continue;
                    }

                    _logger.LogInformation("Seeding {EntityName}...", entityName);
                    await seedingTask();
                }

                result.IsSuccess = result.Errors.Count == 0;
                result.Message = result.IsSuccess
                    ? "Reference data seeding completed successfully"
                    : $"Reference data seeding completed with {result.Errors.Count} errors";

                _logger.LogInformation(
                    "Reference data seeding completed in {Duration}ms. Success: {Success}",
                    stopwatch.ElapsedMilliseconds,
                    result.IsSuccess
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reference data seeding failed");
                result.Errors.Add($"Reference data seeding failed: {ex.Message}");
                result.IsSuccess = false;
            }
            finally
            {
                stopwatch.Stop();
                result.Duration = stopwatch.Elapsed;
            }

            return result;
        }

        private async Task SeedEntityAsync<T>(
            string relativePath,
            SeedingResult parentResult,
            CancellationToken cancellationToken
        )
            where T : class
        {
            try
            {
                var seeder = new EnhancedJsonDataSeeder<T>(
                    _fileReader,
                    _context,
                    _loggerFactory.CreateLogger<EnhancedJsonDataSeeder<T>>(),
                    _config
                );

                var entityResult = await seeder
                    .AddRelativeFilePath(_rootPath, relativePath)
                    .SeedAsync(skipIfExists: true, cancellationToken);

                if (entityResult.IsSuccess)
                {
                    parentResult.SeededEntities.AddRange(entityResult.SeededEntities);
                    parentResult.RecordsSeeded += entityResult.RecordsSeeded;
                }
                else
                {
                    parentResult.Errors.AddRange(entityResult.Errors);
                }
            }
            catch (Exception ex)
            {
                var error = $"Failed to seed {typeof(T).Name}: {ex.Message}";
                _logger.LogError(ex, error);
                parentResult.Errors.Add(error);
            }
        }
    }
}
