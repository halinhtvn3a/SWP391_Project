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
        private readonly BookingDAO _bookingDao = null;
        private readonly TimeSlotDAO _timeSlotDao = null;
        public BookingRepository()
        {
            if (_bookingDao == null)
            {
                _bookingDao = new BookingDAO();
            }
            if (_timeSlotDao == null)
            {
                _timeSlotDao = new TimeSlotDAO();
            }
        }
       

        public BookingRepository(BookingDAO bookingDAO, TimeSlotDAO timeSlotDAO)
        {
            _bookingDao = bookingDAO;
            _timeSlotDao = timeSlotDAO;
        }

        //public Booking AddBooking(Booking Booking) => BookingDAO.AddBooking(Booking);

        public void DeleteBooking(string id) => _bookingDao.DeleteBooking(id);

        public Booking GetBooking(string id) => _bookingDao.GetBooking(id);


        public async Task<List<Booking>> GetBookings(PageResult pageResult) => await _bookingDao.GetBookings(pageResult);

        public List<Booking> GetBookingsByStatus(string status) => _bookingDao.GetBookingsByStatus(status);

        public List<Booking> SearchBookings(DateTime start, DateTime end) => _bookingDao.SearchBookings(start, end);

        public List<Booking> SearchBookingsByUser(string userId) => _bookingDao.SearchBookingsByUser(userId);


        public async Task<TimeSlot> AddBookingTransaction(string slotId)
        {
            return await _bookingDao.AddBookingTransaction(slotId);
        }

        //public async Task<bool> ReserveSlotAsync(string slotId, string userId, decimal paymentAmount)
        //{
        //    await BookingDAO.BeginTransactionAsync();

        //    try
        //    {
        //        var slot = await AddBookingTransaction(slotId);

        //        if (slot == null)
        //        {
        //            return false;
        //        }

        //        // Cập nhật trạng thái slot
        //        slot.IsAvailable = false;
        //       TimeSlotDAO.UpdateSlot(slot);
                

        //        // Tạo booking mới
        //        var booking = new Booking
        //        {
        //            BookingId = Guid.NewGuid().ToString(),
        //            Id = BookingDAO.GenerateID(),
        //            SlotId = slotId,
        //            BookingDate = DateTime.Now,
        //            Check = false,
        //            PaymentAmount = paymentAmount
        //        };

        //        BookingDAO.AddBooking(booking);
        //        await BookingDAO.SaveChangesAsync();
        //        await BookingDAO.CommitTransactionAsync();

        //        return true;
        //    }
        //    catch (DbUpdateException ex)
        //    {
                
        //        await BookingDAO.RollbackTransactionAsync();
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
                
        //        await BookingDAO.RollbackTransactionAsync();
        //        throw;
        //    }
        //}
    }
}
