using System.Diagnostics;
using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CourtCaller.Persistence.Seeding.Strategies
{
    public class CoreDataSeedingStrategy : ISeedingStrategy
    {
        private readonly CourtCallerDbContext _context;
        private readonly ILogger<CoreDataSeedingStrategy> _logger;
        private readonly SeedingConfiguration _config;

        public string Name => "Core Data";
        public int Priority => 1; // Highest priority

        public CoreDataSeedingStrategy(
            CourtCallerDbContext context,
            ILogger<CoreDataSeedingStrategy> logger,
            SeedingConfiguration config
        )
        {
            _context = context;
            _logger = logger;
            _config = config;
        }

        public bool ShouldSeed(string environment)
        {
            var settings = _config.GetEnvironmentSettings(environment);
            return settings.SeedCoreData && !settings.ExcludedEntities.Contains("CoreData");
        }

        public async Task<SeedingResult> SeedAsync(CancellationToken cancellationToken = default)
        {
            var result = new SeedingResult();
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Starting core data seeding...");

                // Seed Identity Roles (if not already done in OnModelCreating)
                await SeedIdentityRolesAsync(result, cancellationToken);

                // Seed default admin user (if needed)
                await SeedDefaultAdminUserAsync(result, cancellationToken);

                result.IsSuccess = true;
                result.Message = "Core data seeding completed successfully";

                _logger.LogInformation(
                    "Core data seeding completed in {Duration}ms",
                    stopwatch.ElapsedMilliseconds
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Core data seeding failed");
                result.Errors.Add($"Core data seeding failed: {ex.Message}");
                result.IsSuccess = false;
            }
            finally
            {
                stopwatch.Stop();
                result.Duration = stopwatch.Elapsed;
            }

            return result;
        }

        private async Task SeedIdentityRolesAsync(
            SeedingResult result,
            CancellationToken cancellationToken
        )
        {
            var existingRolesCount = await _context.Roles.CountAsync(cancellationToken);

            if (existingRolesCount > 0 && !_config.ForceReseed)
            {
                _logger.LogInformation(
                    "Skipping role seeding: {Count} roles already exist",
                    existingRolesCount
                );
                return;
            }

            var requiredRoles = new[]
            {
                new IdentityRole
                {
                    Id = "R001",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                },
                new IdentityRole
                {
                    Id = "R002",
                    Name = "Staff",
                    NormalizedName = "STAFF",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                },
                new IdentityRole
                {
                    Id = "R003",
                    Name = "Customer",
                    NormalizedName = "CUSTOMER",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                },
            };

            var missingRoles = new List<IdentityRole>();

            foreach (var role in requiredRoles)
            {
                var existingRole = await _context.Roles.FirstOrDefaultAsync(
                    r => r.Name == role.Name,
                    cancellationToken
                );
                if (existingRole == null)
                {
                    missingRoles.Add(role);
                }
            }

            if (missingRoles.Any())
            {
                await _context.Roles.AddRangeAsync(missingRoles, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                result.SeededEntities.Add("IdentityRoles");
                result.RecordsSeeded += missingRoles.Count;

                _logger.LogInformation("Seeded {Count} identity roles", missingRoles.Count);
            }
        }

        private async Task SeedDefaultAdminUserAsync(
            SeedingResult result,
            CancellationToken cancellationToken
        )
        {
            // Check if any admin users exist
            var adminRole = await _context.Roles.FirstOrDefaultAsync(
                r => r.NormalizedName == "ADMIN",
                cancellationToken
            );
            if (adminRole == null)
            {
                _logger.LogWarning("Admin role not found, skipping default admin user creation");
                return;
            }

            var adminUsers = await _context
                .UserRoles.Where(ur => ur.RoleId == adminRole.Id)
                .CountAsync(cancellationToken);

            if (adminUsers > 0)
            {
                _logger.LogInformation(
                    "Admin users already exist, skipping default admin creation"
                );
                return;
            }

            // Note: In a real application, you might want to create a default admin user here
            // For this example, we'll just log that this would be done
            _logger.LogInformation(
                "No admin users found. In production, consider creating a default admin user through a separate secure process"
            );

            // It's generally better to handle this through separate admin setup
        }
    }
}
