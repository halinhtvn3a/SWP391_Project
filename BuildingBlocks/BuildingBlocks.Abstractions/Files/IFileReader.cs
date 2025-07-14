using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Abstractions.Files
{
    public interface IFileReader
    {
        Task<string> ReadFileAsync(string filePath);
    }
}
