using QRCoder;
using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Models;
using DAOs.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Services
{
    public class BookingService
    {
        private readonly BookingRepository _bookingRepository = null;
        private readonly TimeSlotRepository _timeSlotRepository = null;
        public BookingService()
        {
            if (_bookingRepository == null)
            {
                _bookingRepository = new BookingRepository();
            }
            if (_timeSlotRepository == null)
            {
                _timeSlotRepository = new TimeSlotRepository();
            }
        }
        public Booking AddBookingTypeFlex(string userId, int numberOfSlot, string branchId) => _bookingRepository.AddBookingTypeFlex(userId, numberOfSlot, branchId);

        public List<Booking> GetBookingsByUserId(string userId) => _bookingRepository.GetBookingsByUserId(userId);
        public void DeleteBooking(string id) => _bookingRepository.DeleteBooking(id);
        public async Task<Booking> GetBooking(string id) => await _bookingRepository.GetBooking(id);
        //public List<Booking> GetBookings() => BookingRepository.GetBookings();
        
        public async Task<List<Booking>> GetBookings(PageResult pageResult, string searchQuery = null) => await _bookingRepository.GetBookings(pageResult,searchQuery);

        public List<Booking> GetBookingsByStatus(string status) => _bookingRepository.GetBookingsByStatus(status);
        public List<Booking> SearchBookingsByTime(DateTime start, DateTime end) => _bookingRepository.SearchBookingsByTime(start, end);
        public async Task<List<Booking>> GetBookingsByUserId(string userId, PageResult pageResult) => await _bookingRepository.GetBookingsByUserId(userId, pageResult);

        //public async Task<IActionResult> PessimistLockAsync(string[] slotId, string userId)
        //{
        //    try
        //    {
        //        var success = await _bookingRepository.ReserveSlotAsync(slotId, userId);

        //        if (!success)
        //        {
        //            return new ConflictObjectResult("Slot is already reserved.");
        //        }

        //        return new OkObjectResult("Slot reserved successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        //    }
        //}

        public Booking PessimistLockAsyncV2(SlotModel[] slotModels, string userId)
        {
            try
            {
                var booking = _bookingRepository.ReserveSlotAsyncV2(slotModels, userId);
                return booking;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<Booking> AddBookingTypeFix(int numberOfMonths, string[] dayOfWeek, DateOnly startDate, TimeSlotModel timeSlotModel, string userId, string branchId)
        {
            try
            {
                var booking =
                    await _bookingRepository.AddBookingTypeFix(numberOfMonths, dayOfWeek, startDate, timeSlotModel, userId, branchId);

                if (booking is null)
                {
                    return null;
                   // return new ConflictObjectResult("Slot is already reserved.");
                }

                return booking;
            }
            catch (Exception ex)
            {
                return null;
                // new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        public async Task DeleteBookingAndSetTimeSlotAsync(string bookingId) => await _bookingRepository.DeleteBookingAndSetTimeSlotAsync(bookingId);

        public void CancelBooking(string bookingId) => _bookingRepository.CancelBooking(bookingId);

        public async Task<List<Booking>> SortBookings(string? sortBy, bool isAsc, PageResult pageResult) => await _bookingRepository.SortBookings(sortBy, isAsc, pageResult);

       

    }
}
