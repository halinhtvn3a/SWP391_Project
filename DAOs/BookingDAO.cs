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
using WebApplication2.Data;

namespace DAOs
{
    public class BookingDAO
    {   private int counter = 3;
        private readonly CourtCallerDbContext dbContext = null;

        public BookingDAO()
        {
            if (dbContext == null)
            {
                dbContext = new CourtCallerDbContext();
            }
        }


        public async Task<List<Booking>> GetBookings(PageResult pageResult)
        {
            var query = dbContext.Bookings.Include(b => b.User).Select(b => new Booking {  BookingId = b.BookingId, Id = b.User.Id,BookingDate= b.BookingDate, Check = b.Check, PaymentAmount = b.PaymentAmount
                
            });
            
            Pagination pagination = new Pagination(dbContext);
            List<Booking> bookings = await pagination.GetListAsync<Booking>(query,pageResult);
            return bookings;
        }

       
        public Booking GetBooking(string id)
        {
            return dbContext.Bookings.FirstOrDefault(m => m.BookingId.Equals(id));
        }

        public async Task<TimeSlot> AddBookingTransaction(string slotId)
        {
            return await dbContext.TimeSlots
                .FromSqlRaw("SELECT * FROM TimeSlots WITH (UPDLOCK) WHERE SlotId = {0} AND IsAvailable = 1", slotId)
                .FirstOrDefaultAsync();
        }


        public void AddBooking(Booking booking)
        {
            dbContext.Bookings.Add(booking);
        }
        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await dbContext.Database.CommitTransactionAsync();
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
                await dbContext.Database.RollbackTransactionAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public string GenerateID()
        {
            return "B" + counter.ToString("D3");
        }


        //public Booking UpdateBooking(int id, Booking Booking)
        //{
        //    Booking oBooking = GetBooking(id);
        //    if (oBooking != null)
        //    {
        //        oBooking.BookingName = Booking.BookingName;
        //        oBooking.IsNatural = Booking.IsNatural;
        //        dbContext.Update(oBooking);
        //        dbContext.SaveChanges();
        //    }
        //    return oBooking;
        //}

        public void DeleteBooking(string id)
        {
            Booking oBooking = GetBooking(id);
            if (oBooking != null)
            {
                oBooking.Check = false;
                dbContext.Update(oBooking);
                dbContext.SaveChanges();
            }
        }

        public List<Booking> GetBookingsByStatus(bool status)
        {
            return dbContext.Bookings.Where(m => m.Check.Equals(status)).ToList();
        }

        public List<Booking> SearchBookings(DateTime start, DateTime end)
        {
            return dbContext.Bookings.Where(m => m.BookingDate >= start && m.BookingDate <= end).ToList();
        }

        public List<Booking> SearchBookingsByUser(string userId)
        {
            return dbContext.Bookings.Where(m => m.Id.Equals(userId)).ToList();
        }


        public async Task BeginTransactionAsync()
        {
            try
            {
                await dbContext.Database.BeginTransactionAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
