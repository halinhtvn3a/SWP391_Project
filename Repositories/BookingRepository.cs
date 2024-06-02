using BusinessObjects;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Helper;

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

        

        public async Task<List<Booking>> GetBookings(PageResult pageResult) => await BookingDAO.GetBookings(pageResult);
    }
}
