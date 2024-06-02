using BusinessObjects;
using DAOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class BookingRepository
    {
        private readonly BookingDAO BookingDAO = null;
        public BookingRepository()
        {
            if (BookingDAO == null)
            {
                BookingDAO = new BookingDAO();
            }
        }
        public Booking AddBooking(Booking Booking) => BookingDAO.AddBooking(Booking);

        public void DeleteBooking(string id) => BookingDAO.DeleteBooking(id);

        public Booking GetBooking(string id) => BookingDAO.GetBooking(id);

        public List<Booking> GetBookings() => BookingDAO.GetBookings();

        //public Booking UpdateBooking(string id, Booking Booking) => BookingDAO.UpdateBooking(id, Booking);

        public List<Booking> GetBookingsByStatus(bool status) => BookingDAO.GetBookingsByStatus(status);

        public List<Booking> SearchBookings(DateTime start, DateTime end) => BookingDAO.SearchBookings(start, end);

        public List<Booking> SearchBookingsByUser(string userId) => BookingDAO.SearchBookingsByUser(userId);

        public List<Booking> SortByPrice() => BookingDAO.SortByPrice();
    }
}
