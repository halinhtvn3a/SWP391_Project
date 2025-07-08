using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAOs.Models;
using DAOs.Helper;
using Services.Interface;
using DAOs;
using Newtonsoft.Json;

namespace Services
{
    public class BookingService : IBookingService
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

        public async Task<(List<Booking>, int total)> GetBookings(PageResult pageResult, string searchQuery = null) => await _bookingRepository.GetBookings(pageResult, searchQuery);

        public List<Booking> GetBookingsByStatus(string status) => _bookingRepository.GetBookingsByStatus(status);
        public List<Booking> SearchBookingsByTime(DateTime start, DateTime end) => _bookingRepository.SearchBookingsByTime(start, end);
        public async Task<List<Booking>> GetBookingsByUserId(string userId, PageResult pageResult) => await _bookingRepository.GetBookingsByUserId(userId, pageResult);

        public Booking ReserveSlotAsyncV2(SlotModel[] slotModels, string userId)
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

        public async Task<Booking> AddBookingTypeFix(int numberOfMonths, string[] dayOfWeek, DateOnly startDate, TimeSlotModel[] timeSlotModel, string userId, string branchId)
        {
            try
            {
                var booking = await _bookingRepository.AddBookingTypeFix(numberOfMonths, dayOfWeek, startDate, timeSlotModel, userId, branchId);

                if (booking is null)
                {
                    return null;
                }

                return booking;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task DeleteBookingAndSetTimeSlotAsync(string bookingId) => await _bookingRepository.DeleteBookingAndSetTimeSlotAsync(bookingId);

        public async Task CancelBooking(string bookingId)
        {
            await _bookingRepository.CancelBooking(bookingId);
        }

        public async Task<List<Booking>> SortBookings(string? sortBy, bool isAsc, PageResult pageResult) => await _bookingRepository.SortBookings(sortBy, isAsc, pageResult);

        public (string, int) NumberOfSlotsAvailable(string userId, string branchId) => _bookingRepository.NumberOfSlotsAvailable(userId, branchId);

        public async Task<(int todayCount, double changePercentage)> GetDailyBookings(string? branchId) => await _bookingRepository.GetDailyBookings(branchId);

        public async Task<ResponseModel> GetDailyBookingsResponse(string? branchId)
        {
            try
            {
                var (todayCount, changePercentage) = await _bookingRepository.GetDailyBookings(branchId);
                var response = new DailyBookingResponse
                {
                    TodayCount = todayCount,
                    ChangePercentage = changePercentage
                };
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(response) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public async Task<(int weeklyCount, double changePercentage)> GetWeeklyBookingsAsync(string? branchId) => await _bookingRepository.GetWeeklyBookingsAsync(branchId);
        public async Task<(int weeklyCount, double changePercentage)> GetMonthlyBookingsAsync(string? branchId) => await _bookingRepository.GetMonthlyBookingsAsync(branchId);
        public async Task<int[]> GetBookingsFromStartOfWeek(string? branchId) => await _bookingRepository.GetBookingsFromStartOfWeek(branchId);
        public async Task<int[]> GetWeeklyBookingsFromStartOfMonth(string? branchId) => await _bookingRepository.GetWeeklyBookingsFromStartOfMonth(branchId);
        public async Task<int[]> GetMonthlyBookingsFromStartOfYear(string? branchId) => await _bookingRepository.GetMonthlyBookingsFromStartOfYear(branchId);

        public async Task<(decimal todayRevenue, decimal changePercentage)> GetDailyRevenue(string? branchId) => await _bookingRepository.GetDailyRevenue(branchId);
        public async Task<(decimal todayRevenue, decimal changePercentage)> GetWeeklyRevenueAsync(string? branchId) => await _bookingRepository.GetWeeklyRevenueAsync(branchId);
        public async Task<(decimal todayRevenue, decimal changePercentage)> GetMonthlyRevenueAsync(string? branchId) => await _bookingRepository.GetMonthlyRevenueAsync(branchId);
        public async Task<decimal[]> GetRevenueFromStartOfWeek(string? branchId) => await _bookingRepository.GetRevenueFromStartOfWeek(branchId);
        public async Task<decimal[]> GetWeeklyRevenueFromStartOfMonth(string? branchId) => await _bookingRepository.GetWeeklyRevenueFromStartOfMonth(branchId);
        public async Task<decimal[]> GetMonthlyRevenueFromStartOfYear(string? branchId) => await _bookingRepository.GetMonthlyRevenueFromStartOfYear(branchId);

        public async Task<ResponseModel> GetQRCodeResponse(string bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetBooking(bookingId);
                if (booking == null)
                {
                    return new ResponseModel { Status = "Error", Message = "Booking not found." };
                }
                var qrData = new { BookingId = booking.BookingId };
                string qrString = JsonConvert.SerializeObject(qrData);

                // We'll handle QR generation in the controller for now to avoid dependency issues
                return new ResponseModel { Status = "Success", Message = qrString };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        // Reserve slot endpoint
        public async Task<ResponseModel> ReserveSlotResponse(SlotModel[] slotModels, string userId)
        {
            try
            {
                if (slotModels == null || slotModels.Length == 0)
                {
                    return new ResponseModel { Status = "Error", Message = "Invalid slot data." };
                }

                if (string.IsNullOrEmpty(userId))
                {
                    return new ResponseModel { Status = "Error", Message = "User ID is required." };
                }

                var booking = _bookingRepository.ReserveSlotAsyncV2(slotModels, userId);
                if (booking != null)
                {
                    return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(booking) };
                }
                return new ResponseModel { Status = "Error", Message = "Failed to reserve slot." };
            }
            catch (ArgumentException ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = "An error occurred while reserving the slot." };
            }
        }

        // Flex booking endpoint
        public async Task<ResponseModel> AddBookingTypeFlexResponse(string userId, int numberOfSlot, string branchId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(branchId))
                {
                    return new ResponseModel { Status = "Error", Message = "User ID and Branch ID are required." };
                }

                if (numberOfSlot <= 0)
                {
                    return new ResponseModel { Status = "Error", Message = "Number of slots must be greater than 0." };
                }

                var booking = _bookingRepository.AddBookingTypeFlex(userId, numberOfSlot, branchId);
                if (booking != null)
                {
                    return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(booking) };
                }
                return new ResponseModel { Status = "Error", Message = "Failed to create flexible booking." };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        // Fix booking endpoint
        public async Task<ResponseModel> AddBookingTypeFixResponse(int numberOfMonths, string[] dayOfWeek, DateOnly startDate, TimeSlotModel[] timeSlotModel, string userId, string branchId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(branchId))
                {
                    return new ResponseModel { Status = "Error", Message = "User ID and Branch ID are required." };
                }

                if (numberOfMonths <= 0)
                {
                    return new ResponseModel { Status = "Error", Message = "Number of months must be greater than 0." };
                }

                if (dayOfWeek == null || dayOfWeek.Length == 0)
                {
                    return new ResponseModel { Status = "Error", Message = "Day of week is required." };
                }

                if (timeSlotModel == null || timeSlotModel.Length == 0)
                {
                    return new ResponseModel { Status = "Error", Message = "Time slots are required." };
                }

                var booking = await _bookingRepository.AddBookingTypeFix(numberOfMonths, dayOfWeek, startDate, timeSlotModel, userId, branchId);
                if (booking != null)
                {
                    return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(booking) };
                }
                return new ResponseModel { Status = "Error", Message = "Failed to create fixed booking." };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        // Cancel booking endpoint
        public async Task<ResponseModel> CancelBookingResponse(string bookingId)
        {
            try
            {
                if (string.IsNullOrEmpty(bookingId))
                {
                    return new ResponseModel { Status = "Error", Message = "Booking ID is required." };
                }

                await _bookingRepository.CancelBooking(bookingId);
                return new ResponseModel { Status = "Success", Message = "Booking cancelled successfully." };
            }
            catch (InvalidOperationException ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = "An error occurred while cancelling the booking." };
            }
        }

        // Delete booking endpoint
        public async Task<ResponseModel> DeleteBookingResponse(string bookingId)
        {
            try
            {
                if (string.IsNullOrEmpty(bookingId))
                {
                    return new ResponseModel { Status = "Error", Message = "Booking ID is required." };
                }

                await _bookingRepository.DeleteBookingAndSetTimeSlotAsync(bookingId);
                return new ResponseModel { Status = "Success", Message = "Booking deleted successfully." };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        // Revenue endpoints
        public async Task<ResponseModel> GetDailyRevenueResponse(string? branchId)
        {
            try
            {
                var (todayRevenue, changePercentage) = await _bookingRepository.GetDailyRevenue(branchId);
                var response = new RevenueResponse
                {
                    Revenue = todayRevenue,
                    ChangePercentage = changePercentage
                };
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(response) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public async Task<ResponseModel> GetWeeklyRevenueResponse(string? branchId)
        {
            try
            {
                var (weeklyRevenue, changePercentage) = await _bookingRepository.GetWeeklyRevenueAsync(branchId);
                var response = new RevenueResponse
                {
                    Revenue = weeklyRevenue,
                    ChangePercentage = changePercentage
                };
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(response) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public async Task<ResponseModel> GetMonthlyRevenueResponse(string? branchId)
        {
            try
            {
                var (monthlyRevenue, changePercentage) = await _bookingRepository.GetMonthlyRevenueAsync(branchId);
                var response = new RevenueResponse
                {
                    Revenue = monthlyRevenue,
                    ChangePercentage = changePercentage
                };
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(response) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        // Booking statistics endpoints
        public async Task<ResponseModel> GetWeeklyBookingsResponse(string? branchId)
        {
            try
            {
                var (weeklyCount, changePercentage) = await _bookingRepository.GetWeeklyBookingsAsync(branchId);
                var response = new DailyBookingResponse
                {
                    TodayCount = weeklyCount,
                    ChangePercentage = changePercentage
                };
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(response) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }

        public async Task<ResponseModel> GetMonthlyBookingsResponse(string? branchId)
        {
            try
            {
                var (monthlyCount, changePercentage) = await _bookingRepository.GetMonthlyBookingsAsync(branchId);
                var response = new DailyBookingResponse
                {
                    TodayCount = monthlyCount,
                    ChangePercentage = changePercentage
                };
                return new ResponseModel { Status = "Success", Message = JsonConvert.SerializeObject(response) };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = "Error", Message = ex.Message };
            }
        }
    }
}
