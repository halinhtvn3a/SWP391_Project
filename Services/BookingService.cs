using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Services
{
    public class BookingService
    {
        private readonly BookingRepository BookingRepository = null;
        public BookingService()
        {
            if (BookingRepository == null)
            {
                BookingRepository = new BookingRepository();
            }
        }
        //public Booking AddBooking(Booking Booking) => BookingRepository.AddBooking(Booking);
        public void DeleteBooking(string id) => BookingRepository.DeleteBooking(id);
        public Booking GetBooking(string id) => BookingRepository.GetBooking(id);
        //public List<Booking> GetBookings() => BookingRepository.GetBookings();
        //public Booking UpdateBooking(string id, Booking Booking) => BookingRepository.UpdateBooking(id, Booking);
        public async Task<List<Booking>> GetBookings(PageResult pageResult) => await BookingRepository.GetBookings(pageResult);

        public List<Booking> GetBookingsByStatus(string status) => BookingRepository.GetBookingsByStatus(status);
        public List<Booking> SearchBookings(DateTime start, DateTime end) => BookingRepository.SearchBookings(start, end);
        public List<Booking> SearchBookingsByUser(string userId) => BookingRepository.SearchBookingsByUser(userId);

        public async Task<IActionResult> PessimistLockAsync(string slotId, string userId, decimal paymentAmount)
        {
            try
            {
                var success = await BookingRepository.ReserveSlotAsync(slotId, userId, paymentAmount);

                if (!success)
                {
                    return new ConflictObjectResult("Slot is already reserved.");
                }

                return new OkObjectResult("Slot reserved successfully.");
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
