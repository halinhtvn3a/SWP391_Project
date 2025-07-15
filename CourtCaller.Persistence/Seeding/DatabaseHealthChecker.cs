using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CourtCaller.Persistence.Seeding
{
    public interface IDatabaseHealthChecker
    {
        Task<DatabaseHealthResult> CheckHealthAsync(CancellationToken cancellationToken = default);
    }

    public class DatabaseHealthChecker : IDatabaseHealthChecker
    {
        private readonly CourtCallerDbContext _context;
        private readonly ILogger<DatabaseHealthChecker> _logger;

        public DatabaseHealthChecker(
            CourtCallerDbContext context,
            ILogger<DatabaseHealthChecker> logger
        )
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DatabaseHealthResult> CheckHealthAsync(
            CancellationToken cancellationToken = default
        )
        {
            var result = new DatabaseHealthResult();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Starting database health check...");

                // Check 1: Database connectivity
                result.CanConnect = await _context.Database.CanConnectAsync(cancellationToken);
                if (!result.CanConnect)
                {
                    result.Issues.Add("Cannot connect to database");
                    return result;
                }

                // Check 2: Pending migrations
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync(
                    cancellationToken
                );
                result.HasPendingMigrations = pendingMigrations.Any();
                result.PendingMigrations = pendingMigrations.ToList();

                if (result.HasPendingMigrations)
                {
                    result.Issues.Add(
                        $"Found {result.PendingMigrations.Count} pending migrations: {string.Join(", ", result.PendingMigrations)}"
                    );
                }

                // Check 3: Database exists and tables are created
                result.DatabaseExists =
                    await _context.Database.EnsureCreatedAsync(cancellationToken) == false; // false means it already existed

                // Check 4: Required tables exist
                var tableChecks = new (string TableName, Func<Task<int>> CheckFunc)[]
                {
                    ("Branches", () => _context.Branches.Take(1).CountAsync(cancellationToken)),
                    ("Courts", () => _context.Courts.Take(1).CountAsync(cancellationToken)),
                    ("TimeSlots", () => _context.TimeSlots.Take(1).CountAsync(cancellationToken)),
                    ("AspNetRoles", () => _context.Roles.Take(1).CountAsync(cancellationToken)),
                };

                foreach (var (tableName, checkFunc) in tableChecks)
                {
                    try
                    {
                        await checkFunc();
                        result.RequiredTablesExist = true;
                    }
                    catch (Exception ex)
                    {
                        result.Issues.Add(
                            $"Table {tableName} does not exist or is not accessible: {ex.Message}"
                        );
                        result.RequiredTablesExist = false;
                        break;
                    }
                }

                // Check 5: Database performance
                var performanceStart = System.Diagnostics.Stopwatch.StartNew();
                await _context.Branches.Take(1).ToListAsync(cancellationToken);
                performanceStart.Stop();
                result.ResponseTimeMs = performanceStart.ElapsedMilliseconds;

                if (result.ResponseTimeMs > 5000) // 5 seconds is concerning
                {
                    result.Issues.Add($"Database response time is slow: {result.ResponseTimeMs}ms");
                }

                // Overall health assessment
                result.IsHealthy =
                    result.CanConnect
                    && result.RequiredTablesExist
                    && !result.HasPendingMigrations
                    && result.ResponseTimeMs < 10000;

                stopwatch.Stop();
                result.CheckDuration = stopwatch.Elapsed;

                _logger.LogInformation(
                    "Database health check completed in {Duration}ms. Status: {Status}",
                    result.CheckDuration.TotalMilliseconds,
                    result.IsHealthy ? "Healthy" : "Unhealthy"
                );

                if (result.Issues.Any())
                {
                    _logger.LogWarning(
                        "Database health issues found: {Issues}",
                        string.Join("; ", result.Issues)
                    );
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                result.Issues.Add($"Health check failed: {ex.Message}");
                result.IsHealthy = false;
                stopwatch.Stop();
                result.CheckDuration = stopwatch.Elapsed;
                return result;
            }
        }
    }

    public class DatabaseHealthResult
    {
        public bool IsHealthy { get; set; }
        public bool CanConnect { get; set; }
        public bool DatabaseExists { get; set; }
        public bool RequiredTablesExist { get; set; }
        public bool HasPendingMigrations { get; set; }
        public List<string> PendingMigrations { get; set; } = new();
        public List<string> Issues { get; set; } = new();
        public long ResponseTimeMs { get; set; }
        public TimeSpan CheckDuration { get; set; }
    }
}
