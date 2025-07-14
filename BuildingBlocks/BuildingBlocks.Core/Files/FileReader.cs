using BuildingBlocks.Abstractions.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BuildingBlocks.Core.Files
{
    public class FileReader : IFileReader
    {
        public async Task<string> ReadFileAsync(string filePath)
        {
            try
            {
                // Check if file exists before proceeding
                var isExist = File.Exists(filePath);
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File not found. {filePath}", filePath);
                }

                using var fileStream = new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read
                );
                using var readerStream = new StreamReader(fileStream, Encoding.UTF8);

                return await readerStream.ReadToEndAsync();
            }
            catch (FileNotFoundException ex)
            {
                // Handle file not found exception specifically
                throw new Exception(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle permission-related issues
                throw new Exception($"Access to the file at {filePath} is denied.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while reading the file.", ex);
            }
        }
    }
}
