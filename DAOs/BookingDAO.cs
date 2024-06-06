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
        private int counter = 3;
        private readonly CourtCallerDbContext DbContext = null;

        public BookingDAO()
        {
            if (DbContext == null)
            {
                DbContext = new CourtCallerDbContext();
            }
        }


        public async Task<List<Booking>> GetBookings(PageResult pageResult)
        {
            var query = DbContext.Bookings.Include(b => b.User).Select(b => new Booking
            {
                BookingId = b.BookingId,
                Id = b.User.Id,
                BookingDate = b.BookingDate,
                Status = b.Status,
                TotalPrice = b.TotalPrice

            });

            Pagination pagination = new Pagination(DbContext);
            List<Booking> bookings = await pagination.GetListAsync<Booking>(query, pageResult);
            return bookings;
        }


        public Booking GetBooking(string id)
        {
            return DbContext.Bookings.FirstOrDefault(m => m.BookingId.Equals(id));
        }

        public async Task<TimeSlot> AddBookingTransaction(string slotId)
        {
            var test = await DbContext.TimeSlots
                 .FromSqlRaw($"SELECT * FROM TimeSlots WITH (UPDLOCK) WHERE SlotId = '{slotId}' AND IsAvailable = 1")
                 .FirstOrDefaultAsync();

            return test;
        }


        public void AddBooking(Booking booking)
        {
            DbContext.Bookings.Add(booking);
        }
        public async Task SaveChangesAsync()
        {
            await DbContext.SaveChangesAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await DbContext.Database.CommitTransactionAsync();
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
                await DbContext.Database.RollbackTransactionAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public string GenerateID()
        {
            return "B" + counter.ToString("D5");
        }


        //public Booking UpdateBooking(int id, Booking Booking)
        //{
        //    Booking oBooking = GetBooking(id);
        //    if (oBooking != null)
        //    {
        //        oBooking.BookingName = Booking.BookingName;
        //        oBooking.IsNatural = Booking.IsNatural;
        //        DbContext.Update(oBooking);
        //        DbContext.SaveChanges();
        //    }
        //    return oBooking;
        //}

        public void DeleteBooking(string id)
        {
            Booking oBooking = GetBooking(id);
            if (oBooking != null)
            {
                oBooking.Status = "Cancel";
                DbContext.Update(oBooking);
                DbContext.SaveChanges();
            }
        }

        public List<Booking> GetBookingsByStatus(string status)
        {
            return DbContext.Bookings.Where(m => m.Status.Equals(status)).ToList();
        }

        public List<Booking> SearchBookings(DateTime start, DateTime end)
        {
            return DbContext.Bookings.Where(m => m.BookingDate >= start && m.BookingDate <= end).ToList();
        }

        public List<Booking> SearchBookingsByUser(string userId)
        {
            return DbContext.Bookings.Where(m => m.Id.Equals(userId)).ToList();
        }


        public async Task BeginTransactionAsync()
        {
            try
            {
                await DbContext.Database.BeginTransactionAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
