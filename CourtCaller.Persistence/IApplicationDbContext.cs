using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace CourtCaller.Persistence
{
	public interface IApplicationDbContext
	{
		DbSet<Review> Reviews { get; }
		DbSet<UserDetail> UserDetails { get; }
		DbSet<Branch> Branches { get; }
		DbSet<Court> Courts { get; }
		DbSet<TimeSlot> TimeSlots { get; }
		DbSet<Booking> Bookings { get; }
		DbSet<Payment> Payments { get; }
		DbSet<Price> Prices { get; }
		DbSet<News> News { get; }
		DbSet<RegistrationRequest> RegistrationRequests { get; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}