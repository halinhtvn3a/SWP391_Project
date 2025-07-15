using System.ComponentModel.DataAnnotations;

namespace CourtCaller.Persistence.Seeding
{
    public class SeedingConfiguration
    {
        public const string SectionName = "Seeding";

        /// <summary>
        /// Enable or disable seeding entirely
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Enable backup before seeding (recommended for production)
        /// </summary>
        public bool EnableBackup { get; set; } = true;

        /// <summary>
        /// Enable rollback on failure
        /// </summary>
        public bool EnableRollback { get; set; } = true;

        /// <summary>
        /// Maximum retry attempts on failure
        /// </summary>
        [Range(0, 5)]
        public int MaxRetryAttempts { get; set; } = 3;

        /// <summary>
        /// Timeout for each seeding operation in seconds
        /// </summary>
        [Range(30, 3600)]
        public int TimeoutSeconds { get; set; } = 300;

        /// <summary>
        /// Force reseed even if data exists (dangerous in production)
        /// </summary>
        public bool ForceReseed { get; set; } = false;

        /// <summary>
        /// Validate data integrity after seeding
        /// </summary>
        public bool ValidateAfterSeed { get; set; } = true;

        /// <summary>
        /// Environment-specific settings
        /// </summary>
        public EnvironmentSettings Development { get; set; } = new();
        public EnvironmentSettings Staging { get; set; } = new();
        public EnvironmentSettings Production { get; set; } = new();

        public EnvironmentSettings GetEnvironmentSettings(string environment)
        {
            return environment.ToLowerInvariant() switch
            {
                "development" => Development,
                "staging" => Staging,
                "production" => Production,
                _ => Development,
            };
        }
    }

    public class EnvironmentSettings
    {
        public bool SeedCoreData { get; set; } = true;
        public bool SeedReferenceData { get; set; } = true;
        public bool SeedTestData { get; set; } = false;
        public bool SeedDemoData { get; set; } = false;
        public List<string> ExcludedEntities { get; set; } = new();
        public int BatchSize { get; set; } = 1000;
    }
}
