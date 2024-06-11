using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using DAOs.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DAOs;
using Microsoft.EntityFrameworkCore;

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
        public void DeleteBooking(string id) => _bookingRepository.DeleteBooking(id);
        public Booking GetBooking(string id) => _bookingRepository.GetBooking(id);
        //public List<Booking> GetBookings() => BookingRepository.GetBookings();
        //public Booking UpdateBooking(string id, Booking Booking) => BookingRepository.UpdateBooking(id, Booking);
        public async Task<List<Booking>> GetBookings(PageResult pageResult) => await _bookingRepository.GetBookings(pageResult);

        public List<Booking> GetBookingsByStatus(string status) => _bookingRepository.GetBookingsByStatus(status);
        public List<Booking> SearchBookings(DateTime start, DateTime end) => _bookingRepository.SearchBookings(start, end);
        public List<Booking> SearchBookingsByUser(string userId) => _bookingRepository.SearchBookingsByUser(userId);

        public async Task<IActionResult> PessimistLockAsync(string[] slotId, string userId)
        {
            try
            {
                var success = await _bookingRepository.ReserveSlotAsync(slotId, userId);

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

        public async Task<IActionResult> PessimistLockAsyncV2(SlotModel[] slotModels, string userId)
        {
            try
            {
                var success = await _bookingRepository.ReserveSlotAsyncV2(slotModels, userId);

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
        
        public async Task<IActionResult> AddBookingTypeFix(int numberOfMonths, string[] dayOfWeek, DateOnly startDate, SlotModel slotModel, string userId)
        {
            try
            {
                var success =
                    await _bookingRepository.AddBookingTypeFix(numberOfMonths, dayOfWeek, startDate, slotModel, userId);

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

        public async Task DeleteBookingAndSetTimeSlotAsync(string bookingId) => await _bookingRepository.DeleteBookingAndSetTimeSlotAsync(bookingId);

    }
}
