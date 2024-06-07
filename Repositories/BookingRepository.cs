using BusinessObjects;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using DAOs.Helper;
using Microsoft.EntityFrameworkCore;
using DAOs.Helper;

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

        public async Task<bool> ReserveSlotAsync(string[] slotId, string userId)
        {
            await _bookingDao.BeginTransactionAsync();

            try
            {
                foreach (var s in slotId)
                {
                    var slot = await AddBookingTransaction(s);

                    if (slot == null)
                    {
                        return false;
                    }

                    
                }

                decimal paymentAmount = 0;

                foreach (var s in slotId)
                {
                    var slot = await AddBookingTransaction(s);
                    slot.Status = "false";
                    _timeSlotDao.UpdateSlot(slot);
                    paymentAmount += slot.Price;
                }

                var generateBookingId = GenerateId.GenerateShortBookingId();

               
                

                // Tạo booking mới
                var booking = new Booking
                {
                    BookingId = generateBookingId,
                    Id = userId,
                    BookingDate = DateTime.Now,
                    Status = "True",
                    TotalPrice = paymentAmount
                };

                _bookingDao.AddBooking(booking);
                await _bookingDao.SaveChangesAsync();
                await _bookingDao.CommitTransactionAsync();

                foreach (var s in slotId)
                {
                    _timeSlotDao.UpdateBookinginSlot(s, generateBookingId);
                }

                return true;
                // var slot = await AddBookingTransaction(slotId);

                // if (slot == null)
                // {
                //     return false;
                // }

                // var generateBookingId = GenerateId.GenerateShortBookingId();

                // // Cập nhật trạng thái slot
                // slot.IsAvailable = false;
                //_timeSlotDao.UpdateSlot(slot);




                // // Tạo booking mới
                // var booking = new Booking
                // {
                //     BookingId = generateBookingId,
                //     Id = userId,
                //     BookingDate = DateTime.Now,
                //     Status = "True",
                //     TotalPrice = paymentAmount
                // };

                // _bookingDao.AddBooking(booking);
                // await _bookingDao.SaveChangesAsync();
                // await _bookingDao.CommitTransactionAsync();
                // _timeSlotDao.UpdateBookinginSlot(slotId, generateBookingId);

                // return true;
            }
            catch (DbUpdateException ex)
            {

                await _bookingDao.RollbackTransactionAsync();
                throw;
            }
            catch (Exception ex)
            {

                await _bookingDao.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<bool> ReserveSlotAsyncV2(SlotModel[] slotModels, string userId)
        {
            //await _bookingDao.BeginTransactionAsync();

            try
            {
                foreach (var s in slotModels)
                {
                    if (!_timeSlotDao.IsSlotAvailable(s))
                    {
                        return false;
                    }
                }

                decimal paymentAmount = 0;

                var generateBookingId = GenerateId.GenerateShortBookingId();

                var booking = new Booking
                {
                    BookingId = generateBookingId,
                    Id = userId,
                    BookingDate = DateTime.Now,
                    Status = "True",
                };
                _bookingDao.AddBooking(booking);
                await _bookingDao.SaveChangesAsync();


                foreach (var s in slotModels)
                {
                    TimeSlot timeSlot = _timeSlotDao.AddSlotToBooking(s, generateBookingId);
                    paymentAmount += timeSlot.Price;
                }

                _bookingDao.UpdateBooking(generateBookingId, paymentAmount);

                
                //await _bookingDao.CommitTransactionAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {

                await _bookingDao.RollbackTransactionAsync();
                throw;
            }
            catch (Exception ex)
            {

                await _bookingDao.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteBookingAndSetTimeSlotAsync(string bookingId) => await _timeSlotDao.DeleteBookingAndSetTimeSlotAsync(bookingId);
    }
}
