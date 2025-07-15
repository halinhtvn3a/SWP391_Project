using BuildingBlocks.Abstractions.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace CourtCaller.Persistence.Seeding
{
	public class EnhancedJsonDataSeeder<T> where T : class
	{
		private readonly IFileReader _fileReader;
		private readonly DbContext _dbContext;
		private readonly ILogger<EnhancedJsonDataSeeder<T>> _logger;
		private readonly SeedingConfiguration _config;
		private string _absoluteFilePathJson = string.Empty;

		public EnhancedJsonDataSeeder(
			IFileReader fileReader,
			DbContext dbContext,
			ILogger<EnhancedJsonDataSeeder<T>> logger,
			SeedingConfiguration config)
		{
			_fileReader = fileReader;
			_dbContext = dbContext;
			_logger = logger;
			_config = config;
		}

		public EnhancedJsonDataSeeder<T> AddRelativeFilePath(string basePath, string relativeFilePath)
		{
			_absoluteFilePathJson = Path.Combine(basePath, relativeFilePath);
			return this;
		}

		public async Task<SeedingResult> SeedAsync(bool skipIfExists = true, CancellationToken cancellationToken = default)
		{
			var result = new SeedingResult();
			var stopwatch = Stopwatch.StartNew();
			var entityName = typeof(T).Name;

			try
			{
				_logger.LogInformation("Starting seeding for {EntityName}...", entityName);

				// Validate file path
				if (string.IsNullOrEmpty(_absoluteFilePathJson))
				{
					result.Errors.Add("File path not provided");
					return result;
				}

				if (!File.Exists(_absoluteFilePathJson))
				{
					result.Errors.Add($"Seed file not found: {_absoluteFilePathJson}");
					return result;
				}

				// Check database connection
				if (!await _dbContext.Database.CanConnectAsync(cancellationToken))
				{
					result.Errors.Add("Cannot connect to database");
					return result;
				}

				// Check if data already exists
				var existingCount = await _dbContext.Set<T>().CountAsync(cancellationToken);
				if (skipIfExists && existingCount > 0 && !_config.ForceReseed)
				{
					result.IsSuccess = true;
					result.Message = $"Skipping {entityName}: {existingCount} records already exist";
					_logger.LogInformation("Skipping {EntityName}: {Count} records already exist", entityName, existingCount);
					return result;
				}

				// Parse and validate data
				var data = await ParseAndValidateJsonAsync(cancellationToken);
				if (!data.Any())
				{
					result.IsSuccess = true;
					result.Message = $"No data to seed for {entityName}";
					return result;
				}

				// If force reseed and data exists, clear existing data
				if (_config.ForceReseed && existingCount > 0)
				{
					_logger.LogWarning("Force reseed enabled. Removing {Count} existing {EntityName} records", existingCount, entityName);
					_dbContext.Set<T>().RemoveRange(_dbContext.Set<T>());
					await _dbContext.SaveChangesAsync(cancellationToken);
				}

				// Seed data in batches
				var batchSize = _config.GetEnvironmentSettings(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development").BatchSize;
				var totalRecords = data.Count();
				var batches = data.Chunk(batchSize).ToList();

				_logger.LogInformation("Seeding {TotalRecords} {EntityName} records in {BatchCount} batches of {BatchSize}",
					totalRecords, entityName, batches.Count, batchSize);

				using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

				try
				{
					foreach (var (batch, index) in batches.Select((batch, index) => (batch, index)))
					{
						_logger.LogDebug("Processing batch {BatchIndex}/{TotalBatches} for {EntityName}",
							index + 1, batches.Count, entityName);

						await _dbContext.Set<T>().AddRangeAsync(batch, cancellationToken);
						await _dbContext.SaveChangesAsync(cancellationToken);

						result.RecordsSeeded += batch.Length;
					}

					await transaction.CommitAsync(cancellationToken);

					result.IsSuccess = true;
					result.SeededEntities.Add(entityName);
					result.Message = $"Successfully seeded {result.RecordsSeeded} {entityName} records";

					_logger.LogInformation("Successfully seeded {RecordsSeeded} {EntityName} records", result.RecordsSeeded, entityName);
				}
				catch (Exception)
				{
					await transaction.RollbackAsync(cancellationToken);
					throw;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to seed {EntityName}", entityName);
				result.Errors.Add($"Seeding failed for {entityName}: {ex.Message}");
				result.IsSuccess = false;
			}
			finally
			{
				stopwatch.Stop();
				result.Duration = stopwatch.Elapsed;
			}

			return result;
		}

		private async Task<IEnumerable<T>> ParseAndValidateJsonAsync(CancellationToken cancellationToken)
		{
			try
			{
				var json = await _fileReader.ReadFileAsync(_absoluteFilePathJson);

				if (string.IsNullOrWhiteSpace(json))
				{
					_logger.LogWarning("Empty JSON file: {FilePath}", _absoluteFilePathJson);
					return Enumerable.Empty<T>();
				}

				var settings = new JsonSerializerSettings()
				{
					NullValueHandling = NullValueHandling.Ignore,
					MissingMemberHandling = MissingMemberHandling.Ignore,
					DateFormatHandling = DateFormatHandling.IsoDateFormat,
					Error = (sender, args) =>
					{
						_logger.LogError("JSON deserialization error: {Error}", args.ErrorContext.Error.Message);
						args.ErrorContext.Handled = true;
					}
				};

				var data = JsonConvert.DeserializeObject<IEnumerable<T>>(json, settings) ?? Enumerable.Empty<T>();

				// Validate each entity
				var validatedData = new List<T>();
				var validationErrors = new List<string>();

				foreach (var item in data)
				{
					var validationResults = new List<ValidationResult>();
					var validationContext = new ValidationContext(item);

					if (Validator.TryValidateObject(item, validationContext, validationResults, true))
					{
						validatedData.Add(item);
					}
					else
					{
						var errors = string.Join(", ", validationResults.Select(r => r.ErrorMessage));
						validationErrors.Add($"Validation failed for {typeof(T).Name}: {errors}");
					}
				}

				if (validationErrors.Any())
				{
					_logger.LogWarning("Validation errors found: {Errors}", string.Join("; ", validationErrors));
				}

				_logger.LogInformation("Parsed {ValidCount}/{TotalCount} valid {EntityName} records",
					validatedData.Count, data.Count(), typeof(T).Name);

				return validatedData;
			}
			catch (JsonException ex)
			{
				throw new InvalidOperationException($"Invalid JSON format in {_absoluteFilePathJson}: {ex.Message}", ex);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Failed to parse JSON from {_absoluteFilePathJson}: {ex.Message}", ex);
			}
		}
	}
}