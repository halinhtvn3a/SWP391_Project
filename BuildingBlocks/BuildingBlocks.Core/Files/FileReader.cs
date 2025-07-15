using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Abstractions.Files;

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
            catch (FileNotFoundException)
            {
                // Preserve original FileNotFoundException details
                throw;
            }
            catch (UnauthorizedAccessException)
            {
                // Preserve original UnauthorizedAccessException details
                throw;
            }
            catch (Exception ex)
            {
                throw new IOException("An error occurred while reading the file.", ex);
            }
        }
    }
}
