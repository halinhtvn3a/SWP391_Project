using System.Threading;
using System.Threading.Tasks;

namespace CourtCaller.Persistence.Seeding
{
    public interface ISeedingStrategy
    {
        Task<SeedingResult> SeedAsync(CancellationToken cancellationToken = default);
        bool ShouldSeed(string environment);
        int Priority { get; }
        string Name { get; }
    }

    public class SeedingResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> SeededEntities { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public TimeSpan Duration { get; set; }
        public int RecordsSeeded { get; set; }
    }
}
