using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Abstractions.Files;
using BuildingBlocks.Core.Data;
using BusinessObjects;
using CourtCaller.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CourtCaller.Persistence
{
    public class CourtCallerDbContextSeed(IFileReader fileReader, IApplicationDbContext dbContext)
    {
        private readonly IFileReader _fileReader = fileReader;
        private readonly IApplicationDbContext _dbContext = dbContext;

        public async Task SeedAsync(string rootPath, CancellationToken cancellationToken = default)
        {
            await new JsonDataSeeder<Branch, DbContext>(_fileReader, (DbContext)_dbContext)
                .AddRelativeFilePath(rootPath, AppCts.SeederRelativePath.BranchPath)
                .SeedAsync();

            await new JsonDataSeeder<Court, DbContext>(_fileReader, (DbContext)_dbContext)
                .AddRelativeFilePath(rootPath, AppCts.SeederRelativePath.CourtPath)
                .SeedAsync();

            await new JsonDataSeeder<TimeSlot, DbContext>(_fileReader, (DbContext)_dbContext)
                .AddRelativeFilePath(rootPath, AppCts.SeederRelativePath.TimeSlotPath)
                .SeedAsync();

            await new JsonDataSeeder<Booking, DbContext>(_fileReader, (DbContext)_dbContext)
                .AddRelativeFilePath(rootPath, AppCts.SeederRelativePath.BookingPath)
                .SeedAsync();

            await new JsonDataSeeder<Payment, DbContext>(_fileReader, (DbContext)_dbContext)
                .AddRelativeFilePath(rootPath, AppCts.SeederRelativePath.PaymentPath)
                .SeedAsync();

            await new JsonDataSeeder<Review, DbContext>(_fileReader, (DbContext)_dbContext)
                .AddRelativeFilePath(rootPath, AppCts.SeederRelativePath.ReviewPath)
                .SeedAsync();

            await new JsonDataSeeder<UserDetail, DbContext>(_fileReader, (DbContext)_dbContext)
                .AddRelativeFilePath(rootPath, AppCts.SeederRelativePath.UserDetailPath)
                .SeedAsync();

            await new JsonDataSeeder<Price, DbContext>(_fileReader, (DbContext)_dbContext)
                .AddRelativeFilePath(rootPath, AppCts.SeederRelativePath.PricePath)
                .SeedAsync();

            await new JsonDataSeeder<News, DbContext>(_fileReader, (DbContext)_dbContext)
                .AddRelativeFilePath(rootPath, AppCts.SeederRelativePath.NewsPath)
                .SeedAsync();

            await new JsonDataSeeder<RegistrationRequest, DbContext>(
                _fileReader,
                (DbContext)_dbContext
            )
                .AddRelativeFilePath(rootPath, AppCts.SeederRelativePath.RegistrationRequestPath)
                .SeedAsync();
        }
    }
}
