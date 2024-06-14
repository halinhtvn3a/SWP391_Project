using BusinessObjects;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Models;
using DAOs.Helper;
using Microsoft.EntityFrameworkCore;
using DAOs.Helper;

namespace Repositories
{
    public class BookingRepository
    {
        private readonly BookingDAO _bookingDao = null;
        private readonly TimeSlotDAO _timeSlotDao = null;
        private readonly PriceDAO _priceDao = null;
        private readonly TimeSlotRepository _timeSlotRepository = null;

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

            if (_priceDao == null)
            {
                _priceDao = new PriceDAO();
            }

            if (_timeSlotRepository == null)
            {
                _timeSlotRepository = new TimeSlotRepository();
            }
        }


        public BookingRepository(BookingDAO bookingDAO, TimeSlotDAO timeSlotDAO)
        {
            _bookingDao = bookingDAO;
            _timeSlotDao = timeSlotDAO;
        }

        //public Booking AddBooking(Booking Booking) => BookingDAO.AddBooking(Booking);
        public List<Booking> GetBookingsByUserId(string userId) => _bookingDao.GetBookingsByUserId(userId);

        public void DeleteBooking(string id) => _bookingDao.DeleteBooking(id);

        public async Task<Booking> GetBooking(string id) => await _bookingDao.GetBooking(id);


        public async Task<List<Booking>> GetBookings(PageResult pageResult) =>
            await _bookingDao.GetBookings(pageResult);

        public List<Booking> GetBookingsByStatus(string status) => _bookingDao.GetBookingsByStatus(status);

        public async Task SaveChangesAsync() => await _bookingDao.SaveChangesAsync();
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
                    Status = "False",
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
                    if (_timeSlotRepository.IsSlotBookedInBranch(s))
                    {
                        return false;
                    }
                }


                if (CheckAvaiableSlotsFromBookingTypeFlex(userId) != null)
                {
                    Booking booking = CheckAvaiableSlotsFromBookingTypeFlex(userId);
                    foreach (var s in slotModels)
                    {
                        TimeSlot timeSlot = _timeSlotDao.AddSlotToBooking(s, booking.BookingId);
                    }
                }
                else
                {
                    decimal paymentAmount = 0;

                    var generateBookingId = GenerateId.GenerateShortBookingId();

                    var booking = new Booking
                    {
                        BookingId = generateBookingId,
                        Id = userId,
                        BookingDate = DateTime.Now,
                        Status = "False",
                        TotalPrice = 0,
                        BookingType = "Normal",
                        NumberOfSlot = slotModels.Length
                    };
                    _bookingDao.AddBooking(booking);
                    await _bookingDao.SaveChangesAsync();
                    foreach (var s in slotModels)
                    {
                        TimeSlot timeSlot = _timeSlotDao.AddSlotToBooking(s, generateBookingId);
                        paymentAmount += timeSlot.Price;
                    }

                    _bookingDao.UpdateBooking(generateBookingId, paymentAmount);
                }





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

        public async Task DeleteBookingAndSetTimeSlotAsync(string bookingId) =>
            await _timeSlotDao.DeleteBookingAndSetTimeSlotAsync(bookingId);

        //public async Task<bool> FlexibleBooking(string userId, int numberOfSlot)
        //{
        //    await _bookingDao.BeginTransactionAsync();
        //    Booking booking = new Booking
        //    {
        //        BookingId = "B" + GenerateId.GenerateShortBookingId(),
        //        Id = userId,
        //        BookingDate = DateTime.Now,
        //        Status = "True",
        //        TotalPrice = 50 * numberOfSlot,
        //        BookingType = "Flex",
        //        TimeSlots = new TimeSlot[numberOfSlot],
        //    };
        //    return true;
        //}

        public Booking CheckAvaiableSlotsFromBookingTypeFlex(string userId)
        {
            var bookings = _bookingDao.GetBookingTypeFlex(userId);
            foreach (var booking in bookings)
            {
                if (booking.NumberOfSlot > _timeSlotDao.NumberOfSlotsInBooking(booking.BookingId))
                {
                    return booking;
                }
            }
            return null;
        }

        public Booking AddBookingTypeFlex(string userId, int numberOfSlot, string branchId)
        {
            Booking booking = new Booking
            {
                BookingId = "B" + GenerateId.GenerateShortBookingId(),
                Id = userId,
                BookingDate = DateTime.Now,
                Status = "False",
                TotalPrice = _priceDao.GetPriceByBranchAndWeekend(branchId, false).SlotPrice * numberOfSlot * 9 / 10,
                BookingType = "Flex",
                NumberOfSlot = numberOfSlot,
            };
            _bookingDao.AddBooking(booking);

            return booking;
        }

        //public async Task<bool> AddBookingTypeFix(int numberOfMonths, string[] dayOfWeek, DateOnly startDate, SlotModel slotModel, string userId)
        //{
        //    DateOnly endDate = startDate.AddMonths(numberOfMonths);

        //    for (var date = startDate; date <= endDate; date = date.AddDays(1))
        //    {
        //        foreach (var day in dayOfWeek)
        //        {
        //            if (date.DayOfWeek.ToString().Equals(day))
        //            {
        //                if (_timeSlotRepository.IsSlotBookedInBranch(slotModel))
        //                {
        //                    return false;
        //                }
        //            }
        //        }
        //    }

        //    string bookingId = GenerateId.GenerateShortBookingId();
        //    int numberOfslots = 0;

        //    Booking booking = new Booking
        //    {
        //        BookingId = bookingId,
        //        Id = userId,
        //        BookingDate = DateTime.Now,
        //        Status = "True",
        //        TotalPrice = _priceDao.GetPriceByBranchAndWeekend(slotModel.BranchId, false).SlotPrice * numberOfMonths * 9 / 10,
        //        BookingType = "Fix"

        //    };
        //    _bookingDao.AddBooking(booking);
        //    for (var date = startDate; date <= endDate; date = date.AddDays(1))
        //    {
        //        foreach (var day in dayOfWeek)
        //        {
        //            if (date.DayOfWeek.ToString().Equals(day))
        //            {
        //                TimeSlot timeSlot = _timeSlotDao.AddSlotToBooking(slotModel, booking.BookingId);
        //                numberOfslots++;
        //            }
        //        }
        //    }
        //    booking.NumberOfSlot = numberOfslots;
        //    _bookingDao.UpdateBooking(bookingId, booking.TotalPrice);

        //    return true;
        //}
        public async Task<bool> AddBookingTypeFix(int numberOfMonths, string[] dayOfWeek, DateOnly startDate, TimeSlotModel timeSlotModel, string userId, string branchId)
        {
            DateOnly endDate = startDate.AddMonths(numberOfMonths);
            List<DateOnly> validDates = new List<DateOnly>();

            // Collect all valid dates first
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (dayOfWeek.Contains(date.DayOfWeek.ToString()))
                {
                    validDates.Add(date);
                }
            }

            // Check if any slot is already booked
            if (validDates.Any(date => _timeSlotRepository.IsSlotBookedInBranch(new SlotModel()
                {
                    BranchId = branchId,
                    TimeSlot = timeSlotModel
                })))
            {
                return false;
            }

            string bookingId = GenerateId.GenerateShortBookingId();
            int numberOfSlots = validDates.Count; // Assuming one slot per valid date

            decimal totalPrice = _priceDao.GetPriceByBranchAndWeekend(branchId, false).SlotPrice * numberOfSlots * 9 / 10;

            Booking booking = new Booking
            {
                BookingId = bookingId,
                Id = userId,
                BookingDate = DateTime.Now,
                Status = "True",
                TotalPrice = totalPrice,
                BookingType = "Fix",
                NumberOfSlot = numberOfSlots
            };

            _bookingDao.AddBooking(booking);

            // Add slots to booking for each valid date
            foreach (var date in validDates)
            {
                _timeSlotDao.AddSlotToBooking(new SlotModel()
                {
                    BranchId = branchId,
                    SlotDate = date,
                    TimeSlot = timeSlotModel
                }, booking.BookingId);
            }

            await _bookingDao.SaveChangesAsync();

            return true;
        }

    }
}
