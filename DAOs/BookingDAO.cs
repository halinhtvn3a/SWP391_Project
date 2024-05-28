using BusinessObjects;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication2.Data;

namespace DAOs
{
    public class BookingDAO
    {
        private readonly CourtCallerDbContext dbContext = null;

        public BookingDAO()
        {
            if (dbContext == null)
            {
                dbContext = new CourtCallerDbContext();
            }
        }

        public List<Booking> GetBookings()
        {
            return dbContext.Bookings.ToList();
        }

        public Booking GetBooking(string id)
        {
            return dbContext.Bookings.FirstOrDefault(m => m.BookingId.Equals(id));
        }

        public Booking AddBooking(Booking Booking)
        {
            dbContext.Bookings.Add(Booking);
            dbContext.SaveChanges();
            return Booking;
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
    }
}
