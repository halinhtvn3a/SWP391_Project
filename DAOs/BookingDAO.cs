using BusinessObjects;
using DAOs;
using DAOs.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs
{
    public class BookingDAO
    {
        
        private readonly CourtCallerDbContext _courtCallerDbContext = null;

        public BookingDAO()
        {
            if (_courtCallerDbContext == null)
            {
                _courtCallerDbContext = new CourtCallerDbContext();
            }
        }


        public async Task<List<Booking>> GetBookings(PageResult pageResult)
        {
            var query = _courtCallerDbContext.Bookings.Include(b => b.User).Select(b => new Booking
            {
                BookingId = b.BookingId,
                Id = b.User.Id,
                BookingDate = b.BookingDate,
                Status = b.Status,
                TotalPrice = b.TotalPrice,
                BookingType = b.BookingType,
                NumberOfSlot = b.NumberOfSlot
            });

            Pagination pagination = new Pagination(_courtCallerDbContext);
            List<Booking> bookings = await pagination.GetListAsync<Booking>(query, pageResult);
            return bookings;
        }


        public Booking GetBooking(string id)
        {
            return _courtCallerDbContext.Bookings.FirstOrDefault(m => m.BookingId.Equals(id));
        }

        public async Task<TimeSlot> AddBookingTransaction(string slotId)
        {
            var test = await _courtCallerDbContext.TimeSlots
                 .FromSqlRaw($"SELECT * FROM TimeSlots WITH (UPDLOCK) WHERE SlotId = '{slotId}' AND IsAvailable = 1")
                 .FirstOrDefaultAsync();

            return test;
        }


        public void AddBooking(Booking booking)
        {
            _courtCallerDbContext.Bookings.Add(booking);
            _courtCallerDbContext.SaveChanges();
        }
        public async Task SaveChangesAsync()
        {
            await _courtCallerDbContext.SaveChangesAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _courtCallerDbContext.Database.CommitTransactionAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _courtCallerDbContext.Database.RollbackTransactionAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        public Booking UpdateBooking(string id, decimal price)
        {
            Booking oBooking = GetBooking(id);
            if (oBooking != null)
            {
                oBooking.TotalPrice = price;
                _courtCallerDbContext.Update(oBooking);
                _courtCallerDbContext.SaveChanges();
            }
            return oBooking;
        }

        public void DeleteBooking(string id)
        {
            Booking oBooking = GetBooking(id);
            if (oBooking != null)
            {
                oBooking.Status = "Cancel";
                _courtCallerDbContext.Update(oBooking);
                _courtCallerDbContext.SaveChanges();
            }
        }

        public List<Booking> GetBookingsByStatus(string status)
        {
            return _courtCallerDbContext.Bookings.Where(m => m.Status.Equals(status)).ToList();
        }

        public List<Booking> SearchBookings(DateTime start, DateTime end)
        {
            return _courtCallerDbContext.Bookings.Where(m => m.BookingDate >= start && m.BookingDate <= end).ToList();
        }

        public List<Booking> SearchBookingsByUser(string userId)
        {
            return _courtCallerDbContext.Bookings.Where(m => m.Id.Equals(userId)).ToList();
        }


        public async Task BeginTransactionAsync()
        {
            try
            {
                await _courtCallerDbContext.Database.BeginTransactionAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<Booking> CheckBookingTypeFlex()
        {
            return _courtCallerDbContext.Bookings
                .FromSqlRaw($"SELECT * FROM Bookings WITH (UPDLOCK) WHERE BookingType='Flex'").ToList();
        }


    }
}
