using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Abstractions.Files;
using BuildingBlocks.Core.Files;
using CourtCaller.Persistence.Seeding;
using CourtCaller.Persistence.Seeding.Strategies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CourtCaller.Persistence.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IApplicationDbContext, CourtCallerDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("CourtCallerDb");
                options.UseSqlServer(connectionString);
            });

            // Add enterprise seeding services
            services.AddEnterpriseSeeding(configuration);

            return services;
        }

        public static IServiceCollection AddEnterpriseSeeding(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register configuration
            services.Configure<SeedingConfiguration>(configuration.GetSection(SeedingConfiguration.SectionName));

            // Register core seeding services
            services.AddScoped<IFileReader, FileReader>();
            services.AddScoped<IDatabaseHealthChecker, DatabaseHealthChecker>();
            services.AddScoped<IEnterpriseSeedingManager, EnterpriseSeedingManager>();

            // Register seeding strategies
            services.AddSeedingStrategy<CoreDataSeedingStrategy>();
            services.AddSeedingStrategy<ReferenceDataSeedingStrategy>();
            services.AddSeedingStrategy<TestDataSeedingStrategy>();

            return services;
        }

        private static IServiceCollection AddSeedingStrategy<T>(this IServiceCollection services)
                where T : class, ISeedingStrategy
        {
            services.AddScoped<ISeedingStrategy>(provider =>
            {
                var context = provider.GetRequiredService<CourtCallerDbContext>();
                var fileReader = provider.GetRequiredService<IFileReader>();
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var config = provider.GetRequiredService<IOptions<SeedingConfiguration>>().Value;

                var seedFilesPath = AppDomain.CurrentDomain.BaseDirectory;

                return typeof(T).Name switch
                {
                    nameof(CoreDataSeedingStrategy) => new CoreDataSeedingStrategy(context, loggerFactory.CreateLogger<CoreDataSeedingStrategy>(), config),
                    nameof(ReferenceDataSeedingStrategy) => new ReferenceDataSeedingStrategy(context, fileReader, loggerFactory.CreateLogger<ReferenceDataSeedingStrategy>(), loggerFactory, config, seedFilesPath),
                    nameof(TestDataSeedingStrategy) => new TestDataSeedingStrategy(context, fileReader, loggerFactory.CreateLogger<TestDataSeedingStrategy>(), loggerFactory, config, seedFilesPath),
                    _ => throw new ArgumentException($"Unknown seeding strategy: {typeof(T).Name}")
                };
            });

            return services;
        }
        /// <summary>
        /// Execute database seeding during application startup
        /// </summary>
        public static async Task<IHost> ExecuteSeedingAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var seedingManager = scope.ServiceProvider.GetRequiredService<IEnterpriseSeedingManager>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DatabaseSeeding");

            try
            {
                logger.LogInformation("Starting enterprise database seeding...");

                var result = await seedingManager.ExecuteSeedingAsync();

                if (result.IsSuccess)
                {
                    logger.LogInformation("Database seeding completed successfully. Total records: {RecordsSeeded}", result.RecordsSeeded);
                }
                else
                {
                    logger.LogError("Database seeding failed with errors: {Errors}", string.Join("; ", result.Errors));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database seeding failed with unexpected error");
            }

            return host;
        }

        /// <summary>
        /// Execute database seeding only if enabled in configuration
        /// </summary>
        public static async Task<IHost> ExecuteSeedingIfEnabledAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IOptions<SeedingConfiguration>>().Value;

            if (!config.Enabled)
            {
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("DatabaseSeeding");
                logger.LogInformation("Database seeding is disabled in configuration");
                return host;
            }

            return await host.ExecuteSeedingAsync();
        }
    }
}
