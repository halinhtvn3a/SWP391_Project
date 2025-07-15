using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Abstractions.Data;
using BuildingBlocks.Abstractions.Files;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BuildingBlocks.Core.Data
{
    public class JsonDataSeeder<T, TContext> : IDataSeeder
        where T : class
        where TContext : DbContext
    {
        private readonly IFileReader _fileReader;
        private string _absoluteFilePathJson = default!;
        private readonly TContext _dbContext;

        public JsonDataSeeder(IFileReader fileReader, TContext dbContext)
        {
            _fileReader = fileReader;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Add file before parse to json object
        /// </summary>
        /// <param name="relativeFilePath"></param>
        /// <returns></returns>
        public JsonDataSeeder<T, TContext> AddRelativeFilePath(
            string basePath,
            string relativeFilePath
        )
        {
            _absoluteFilePathJson = Path.Combine(basePath, relativeFilePath);
            return this;
        }

        /// <summary>
        /// Parse Json string To particula object
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<IEnumerable<T>> ParseJsonToObject()
        {
            try
            {
                var json = await _fileReader.ReadFileAsync(_absoluteFilePathJson);
                var settings = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                };

                var data =
                    JsonConvert.DeserializeObject<IEnumerable<T>>(json, settings)
                    ?? Enumerable.Empty<T>();
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Seed particular 1 entity from json file
        /// </summary>
        /// <returns></returns>
        public async Task SeedAsync()
        {
            try
            {
                // check file exists first
                if (string.IsNullOrEmpty(_absoluteFilePathJson))
                    throw new FileNotFoundException("Does not have file");

                // Seed data based on entity
                if (await _dbContext.Database.CanConnectAsync())
                {
                    if (!await _dbContext.Set<T>().AnyAsync())
                    {
                        await _dbContext.Set<T>().AddRangeAsync(await ParseJsonToObject());
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"An error occurred while seeding data: {ex.Message}",
                    ex
                );
            }
        }
    }
}
