﻿using BusinessObjects;
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
        Task<(int todayCount, double changePercentage)> GetDailyBookings();
        Task<(int weeklyCount, double changePercentage)> GetWeeklyBookingsAsync();
        Task<int[]> GetBookingsFromStartOfWeek();
        Task<int[]> GetWeeklyBookingsFromStartOfMonth();


    }
}