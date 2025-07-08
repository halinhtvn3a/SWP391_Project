using BusinessObjects;
using DAOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Helper;

namespace Services.Interface
{
    public interface IBookingService
    {
        Booking AddBookingTypeFlex(string userId, int numberOfSlot, string branchId);
        List<Booking> GetBookingsByUserId(string userId);
        void DeleteBooking(string id);
        Task<Booking> GetBooking(string id);
        Task<(List<Booking>, int total)> GetBookings(PageResult pageResult, string searchQuery = null);
        List<Booking> GetBookingsByStatus(string status);
        List<Booking> SearchBookingsByTime(DateTime start, DateTime end);
        Task<List<Booking>> GetBookingsByUserId(string userId, PageResult pageResult);
        Booking ReserveSlotAsyncV2(SlotModel[] slotModels, string userId);
        Task<Booking> AddBookingTypeFix(int numberOfMonths, string[] dayOfWeek, DateOnly startDate, TimeSlotModel[] timeSlotModel, string userId, string branchId);
        Task DeleteBookingAndSetTimeSlotAsync(string bookingId);
        Task CancelBooking(string bookingId);
        Task<List<Booking>> SortBookings(string? sortBy, bool isAsc, PageResult pageResult);
        (string, int) NumberOfSlotsAvailable(string userId, string branchId);
        Task<(int todayCount, double changePercentage)> GetDailyBookings(string? branchId);
        Task<(int weeklyCount, double changePercentage)> GetWeeklyBookingsAsync(string? branchId);
        Task<(int weeklyCount, double changePercentage)> GetMonthlyBookingsAsync(string? branchId);

        Task<int[]> GetBookingsFromStartOfWeek(string? branchId);
        Task<int[]> GetWeeklyBookingsFromStartOfMonth(string? branchId);
        Task<int[]> GetMonthlyBookingsFromStartOfYear(string? branchId);

        Task<(decimal todayRevenue, decimal changePercentage)> GetDailyRevenue(string? branchId);
        Task<(decimal todayRevenue, decimal changePercentage)> GetMonthlyRevenueAsync(string? branchId);
        Task<(decimal todayRevenue, decimal changePercentage)> GetWeeklyRevenueAsync(string? branchId);
        Task<decimal[]> GetRevenueFromStartOfWeek(string? branchId);

        Task<decimal[]> GetWeeklyRevenueFromStartOfMonth(string? branchId);

        Task<decimal[]> GetMonthlyRevenueFromStartOfYear(string? branchId);

        Task<ResponseModel> GetDailyBookingsResponse(string? branchId);
        Task<ResponseModel> GetQRCodeResponse(string bookingId);
        Task<ResponseModel> ReserveSlotResponse(SlotModel[] slotModels, string userId);
        Task<ResponseModel> AddBookingTypeFlexResponse(string userId, int numberOfSlot, string branchId);
        Task<ResponseModel> AddBookingTypeFixResponse(int numberOfMonths, string[] dayOfWeek, DateOnly startDate, TimeSlotModel[] timeSlotModel, string userId, string branchId);
        Task<ResponseModel> CancelBookingResponse(string bookingId);
        Task<ResponseModel> DeleteBookingResponse(string bookingId);
        Task<ResponseModel> GetDailyRevenueResponse(string? branchId);
        Task<ResponseModel> GetWeeklyRevenueResponse(string? branchId);
        Task<ResponseModel> GetMonthlyRevenueResponse(string? branchId);
        Task<ResponseModel> GetWeeklyBookingsResponse(string? branchId);
        Task<ResponseModel> GetMonthlyBookingsResponse(string? branchId);
    }
}
