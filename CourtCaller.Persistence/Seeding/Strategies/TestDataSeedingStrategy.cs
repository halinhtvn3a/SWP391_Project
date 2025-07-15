using BuildingBlocks.Abstractions.Files;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CourtCaller.Persistence.Seeding.Strategies
{
	public class TestDataSeedingStrategy : ISeedingStrategy
	{
		private readonly CourtCallerDbContext _context;
		private readonly IFileReader _fileReader;
		private readonly ILogger<TestDataSeedingStrategy> _logger;
		private readonly ILoggerFactory _loggerFactory;
		private readonly SeedingConfiguration _config;
		private readonly string _rootPath;

		public string Name => "Test Data";
		public int Priority => 3;

		public TestDataSeedingStrategy(
	CourtCallerDbContext context,
	IFileReader fileReader,
	ILogger<TestDataSeedingStrategy> logger,
	ILoggerFactory loggerFactory,
	SeedingConfiguration config,
	string rootPath)
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
			return settings.SeedTestData && !settings.ExcludedEntities.Contains("TestData");
		}

		public async Task<SeedingResult> SeedAsync(CancellationToken cancellationToken = default)
		{
			var result = new SeedingResult();
			var stopwatch = Stopwatch.StartNew();

			try
			{
				_logger.LogInformation("Starting test data seeding...");

				// Check if we have the required reference data first
				var branchesExist = await _context.Branches.AnyAsync(cancellationToken);
				var courtsExist = await _context.Courts.AnyAsync(cancellationToken);

				if (!branchesExist || !courtsExist)
				{
					result.Errors.Add("Cannot seed test data: Required reference data (Branches/Courts) not found");
					return result;
				}

				// Define seeding order for test data
				var seedingTasks = new (string EntityName, Func<Task> SeedingTask)[]
				{
					("TimeSlots", () => SeedEntityAsync<TimeSlot>(AppCts.SeederRelativePath.TimeSlotPath, result, cancellationToken)),
					("UserDetails", () => SeedEntityAsync<UserDetail>(AppCts.SeederRelativePath.UserDetailPath, result, cancellationToken)),
					("Bookings", () => SeedEntityAsync<Booking>(AppCts.SeederRelativePath.BookingPath, result, cancellationToken)),
					("Payments", () => SeedEntityAsync<Payment>(AppCts.SeederRelativePath.PaymentPath, result, cancellationToken)),
					("Reviews", () => SeedEntityAsync<Review>(AppCts.SeederRelativePath.ReviewPath, result, cancellationToken)),
					("RegistrationRequests", () => SeedEntityAsync<RegistrationRequest>(AppCts.SeederRelativePath.RegistrationRequestPath, result, cancellationToken))
				};

				foreach (var (entityName, seedingTask) in seedingTasks)
				{
					var settings = _config.GetEnvironmentSettings(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development");

					if (settings.ExcludedEntities.Contains(entityName))
					{
						_logger.LogInformation("Skipping {EntityName} - excluded in environment settings", entityName);
						continue;
					}

					_logger.LogInformation("Seeding test {EntityName}...", entityName);
					await seedingTask();
				}

				result.IsSuccess = result.Errors.Count == 0;
				result.Message = result.IsSuccess
					? "Test data seeding completed successfully"
					: $"Test data seeding completed with {result.Errors.Count} errors";

				_logger.LogInformation("Test data seeding completed in {Duration}ms. Success: {Success}",
					stopwatch.ElapsedMilliseconds, result.IsSuccess);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Test data seeding failed");
				result.Errors.Add($"Test data seeding failed: {ex.Message}");
				result.IsSuccess = false;
			}
			finally
			{
				stopwatch.Stop();
				result.Duration = stopwatch.Elapsed;
			}

			return result;
		}

		private async Task SeedEntityAsync<T>(string relativePath, SeedingResult parentResult, CancellationToken cancellationToken)
			where T : class
		{
			try
			{
				var seeder = new EnhancedJsonDataSeeder<T>(
	_fileReader,
	_context,
	_loggerFactory.CreateLogger<EnhancedJsonDataSeeder<T>>(),
	_config);

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