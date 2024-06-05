using BusinessObjects;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Helper;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class BookingRepository
    {
        private readonly BookingDAO BookingDAO = null;
        private readonly TimeSlotDAO TimeSlotDAO = null;
        public BookingRepository()
        {
            if (BookingDAO == null)
            {
                BookingDAO = new BookingDAO();
            }
            if (TimeSlotDAO == null)
            {
                TimeSlotDAO = new TimeSlotDAO();
            }
        }
       

        public BookingRepository(BookingDAO bookingDAO, TimeSlotDAO timeSlotDAO)
        {
            BookingDAO = bookingDAO;
            TimeSlotDAO = timeSlotDAO;
        }

        //public Booking AddBooking(Booking Booking) => BookingDAO.AddBooking(Booking);

        public void DeleteBooking(string id) => BookingDAO.DeleteBooking(id);

        public Booking GetBooking(string id) => BookingDAO.GetBooking(id);


        public async Task<List<Booking>> GetBookings(PageResult pageResult) => await BookingDAO.GetBookings(pageResult);

        public List<Booking> GetBookingsByStatus(bool status) => BookingDAO.GetBookingsByStatus(status);

        public List<Booking> SearchBookings(DateTime start, DateTime end) => BookingDAO.SearchBookings(start, end);

        public List<Booking> SearchBookingsByUser(string userId) => BookingDAO.SearchBookingsByUser(userId);


        public async Task<TimeSlot> AddBookingTransaction(string slotId)
        {
            return await BookingDAO.AddBookingTransaction(slotId);
        }

        public async Task<bool> ReserveSlotAsync(string slotId, string userId, decimal paymentAmount)
        {
            await BookingDAO.BeginTransactionAsync();

            try
            {
                var slot = await AddBookingTransaction(slotId);

                if (slot == null)
                {
                    return false;
                }

                // Cập nhật trạng thái slot
                slot.IsAvailable = false;
               TimeSlotDAO.UpdateSlot(slot);
                

                // Tạo booking mới
                var booking = new Booking
                {
                    BookingId = Guid.NewGuid().ToString(),
                    Id = BookingDAO.GenerateID(),
                    SlotId = slotId,
                    BookingDate = DateTime.Now,
                    Check = false,
                    PaymentAmount = paymentAmount
                };

                BookingDAO.AddBooking(booking);
                await BookingDAO.SaveChangesAsync();
                await BookingDAO.CommitTransactionAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                
                await BookingDAO.RollbackTransactionAsync();
                throw;
            }
            catch (Exception ex)
            {
                
                await BookingDAO.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
