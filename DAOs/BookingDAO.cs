using BusinessObjects;
using DAOs;
using DAOs.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PageResult = DAOs.Helper.PageResult;
using System.ComponentModel.DataAnnotations;
using DAOs.Models;

namespace DAOs
{
    public class BookingDAO
    {

        private readonly CourtCallerDbContext _courtCallerDbContext = null;

        public BookingDAO()
        {
            if (_courtCallerDbContext == null)
            {
                _courtCallerDbContext = new CourtCallerDbContext();
            }
        }
        public BookingDAO(CourtCallerDbContext context)
        {
            _courtCallerDbContext = context;
        }

        public async Task<(List<Booking>, int total)> GetBookings(PageResult pageResult, string searchQuery = null)
        {
            var query = _courtCallerDbContext.Bookings
                .Include(b => b.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(b =>
                    b.BookingId.ToString().Contains(searchQuery) ||
                    b.User.Id.ToString().Contains(searchQuery) ||
                    b.BookingDate.ToString().Contains(searchQuery) ||
                    b.Status.Contains(searchQuery) ||
                    b.TotalPrice.ToString().Contains(searchQuery) ||
                    b.BookingType.Contains(searchQuery) ||
                    b.NumberOfSlot.ToString().Contains(searchQuery)
                );
            }

           
            var total = await query.CountAsync();

            var pagedQuery = query
                .Skip((pageResult.PageNumber - 1) * pageResult.PageSize)
                .Take(pageResult.PageSize)
                .Select(b => new Booking
                {
                    BookingId = b.BookingId,
                    Id = b.User.Id,
                    BookingDate = b.BookingDate,
                    BranchId = b.BranchId,
                    Status = b.Status,
                    TotalPrice = b.TotalPrice,
                    BookingType = b.BookingType,
                    NumberOfSlot = b.NumberOfSlot
                });

            var bookings = await pagedQuery.ToListAsync();

            return (bookings, total);
        }




        public async Task<Booking> GetBooking(string id)
        {
            var booking = await _courtCallerDbContext.Bookings.FirstOrDefaultAsync(m => m.BookingId.Equals(id));
            
            return booking;
        }

        public async Task<TimeSlot> AddBookingTransaction(string slotId)
        {
            var test = await _courtCallerDbContext.TimeSlots
                 .FromSqlRaw($"SELECT * FROM TimeSlots WITH (UPDLOCK) WHERE SlotId = '{slotId}' AND IsAvailable = 1")
                 .FirstOrDefaultAsync();

            return test;
        }


        public void AddBooking(Booking booking)
        {
            _courtCallerDbContext.Bookings.Add(booking);
            _courtCallerDbContext.SaveChanges();
        }
        public async Task SaveChangesAsync()
        {
            await _courtCallerDbContext.SaveChangesAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _courtCallerDbContext.Database.CommitTransactionAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _courtCallerDbContext.Database.RollbackTransactionAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task UpdateBooking(Booking booking)
        {
            _courtCallerDbContext.Bookings.Update(booking);
           await _courtCallerDbContext.SaveChangesAsync();
        }

        public async Task<Booking> UpdateBooking(string id, decimal price)
        {
            Booking oBooking = await GetBooking(id);
            if (oBooking != null)
            {
                oBooking.TotalPrice = price;
                _courtCallerDbContext.Update(oBooking);
                _courtCallerDbContext.SaveChanges();
            }
            return oBooking;
        }

        public async void DeleteBooking(string id)
        {
            Booking oBooking = await GetBooking(id);
            if (oBooking != null)
            {
                oBooking.Status = "Canceled";
                _courtCallerDbContext.Update(oBooking);
                _courtCallerDbContext.SaveChanges();
            }
        }

        public List<Booking> GetBookingsByStatus(string status)
        {
            return _courtCallerDbContext.Bookings.Where(m => m.Status.Equals(status)).ToList();
        }

        public List<Booking> SearchBookingsByTime(DateTime start, DateTime end)
        {
            return _courtCallerDbContext.Bookings.Where(m => m.BookingDate >= start && m.BookingDate <= end).ToList();
        }

        public async Task<List<Booking>> GetBookingsByUserId(string userId, PageResult pageResult)
        {
            var query = _courtCallerDbContext.Bookings.AsQueryable();

            Pagination pagination = new Pagination(_courtCallerDbContext);
            List<Booking> bookings = await pagination.GetListAsync<Booking>(query, pageResult);
            return bookings;
        }


        public async Task BeginTransactionAsync()
        {
            try
            {
                await _courtCallerDbContext.Database.BeginTransactionAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<Booking> GetBookingTypeFlex(string userId)
        {
            return _courtCallerDbContext.Bookings.Where(m => m.BookingType.Equals("Flex") && m.Id.Equals(userId)).ToList();
        }

        public List<Booking> GetBookingsByUserId(string userId)
        {
            return _courtCallerDbContext.Bookings.Where(m => m.Id.Equals(userId)).ToList();
        }

        public async Task<List<Booking>> SortBookings(string? sortBy, bool isAsc, PageResult pageResult)
        {
            IQueryable<Booking> query = _courtCallerDbContext.Bookings;

            switch (sortBy?.ToLower())
            {
                case "bookingid":
                    query = isAsc ? query.OrderBy(b => b.BookingId) : query.OrderByDescending(b => b.BookingId);
                    break;
                case "id":
                    query = isAsc ? query.OrderBy(b => b.Id) : query.OrderByDescending(b => b.Id);
                    break;
                case "bookingdate":
                    query = isAsc ? query.OrderBy(b => b.BookingDate) : query.OrderByDescending(b => b.BookingDate);
                    break;
                case "bookingtype":
                    query = isAsc ? query.OrderBy(b => b.BookingType) : query.OrderByDescending(b => b.BookingType);
                    break;
                case "numberofslot":
                    query = isAsc ? query.OrderBy(b => b.NumberOfSlot) : query.OrderByDescending(b => b.NumberOfSlot);
                    break;
                case "status":
                    query = isAsc ? query.OrderBy(b => b.Status) : query.OrderByDescending(b => b.Status);
                    break;
                case "totalprice":
                    query = isAsc ? query.OrderBy(b => b.TotalPrice) : query.OrderByDescending(b => b.TotalPrice);
                    break;
                default:
                    break;
            }
            Pagination pagination = new Pagination(_courtCallerDbContext);
            List<Booking> bookings = await pagination.GetListAsync<Booking>(query, pageResult);
            return bookings;
        }

        public async Task<(int todayCount, double changePercentage)> GetDailyBookings()
        {
            var today = DateTime.UtcNow.Date.AddDays(1);
            var tomorrow = today.AddDays(1);
            var yesterday = today.AddDays(-1);

            
            var todayBookingCount = await _courtCallerDbContext.Bookings
                .Where(m => m.BookingDate >= today && m.BookingDate < tomorrow)
                .CountAsync();

           
            var yesterdayBookingCount = await _courtCallerDbContext.Bookings
                .Where(m => m.BookingDate >= yesterday && m.BookingDate < today)
                .CountAsync();
            double changePercentage = 0;
            if (yesterdayBookingCount > 0)
            {
                changePercentage = ((double)(todayBookingCount - yesterdayBookingCount) / yesterdayBookingCount) * 100;
            }
            else if (todayBookingCount > 0)
            {
                changePercentage = 100;
            }
            return (todayBookingCount, changePercentage);
        }


        public async Task<(int weeklyCount, double changePercentage)> GetWeeklyBookingsAsync()
        {
            var today = DateTime.UtcNow.Date.AddDays(1);
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek).AddDays(1);
            var endOfWeek = startOfWeek.AddDays(6);
            var startOfLastWeek = startOfWeek.AddDays(-7);
            var endOfLastWeek = startOfWeek;

            
            var currentWeekBookings = await _courtCallerDbContext.Bookings
                .Where(m => m.BookingDate >= startOfWeek && m.BookingDate < endOfWeek)
                .ToListAsync();

            
            var lastWeekBookings = await _courtCallerDbContext.Bookings
                .Where(m => m.BookingDate >= startOfLastWeek && m.BookingDate < endOfLastWeek)
                .ToListAsync();

            int currentWeekCount = currentWeekBookings.Count();
            int lastWeekCount = lastWeekBookings.Count();

            double changePercentage = 0;
            if (lastWeekCount > 0)
            {
                changePercentage = ((double)(currentWeekCount - lastWeekCount) / lastWeekCount) * 100;
            }
            else if (currentWeekCount > 0)
            {
                changePercentage = 100; 
            }

            return (currentWeekCount, changePercentage);
        }



        public async Task<List<Booking>> GetBookingsForLastWeekAsync()
        {
            var lastWeek = DateTime.Today.AddDays(-7);
            return await _courtCallerDbContext.Bookings
                .Where(b => b.BookingDate >= lastWeek)
                .Include(b => b.TimeSlots)
                .ToListAsync();
        }

        public async Task<int[]> GetBookingsFromStartOfWeek()
        {
            var today = DateTime.UtcNow.Date.AddDays(1);
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek).AddDays(1);

            var bookingCounts = new List<int>();

            for (var date = startOfWeek; date <= today; date = date.AddDays(1))
            {
                var nextDate = date.AddDays(1);
                var count = await _courtCallerDbContext.Bookings
                    .Where(m => m.BookingDate >= date && m.BookingDate < nextDate)
                    .CountAsync();

                bookingCounts.Add(count);
            }

            return bookingCounts.ToArray();
        }
        public async Task<int[]> GetWeeklyBookingsFromStartOfMonth()
        {
            var today = DateTime.UtcNow.Date.AddDays(1);
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfCurrentWeek = today.AddDays(-(int)today.DayOfWeek).AddDays(1);

            var bookingCounts = new List<int>();

            for (var startOfWeek = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek).AddDays(1);
                 startOfWeek <= startOfCurrentWeek;
                 startOfWeek = startOfWeek.AddDays(7))
            {
                var endOfWeek = startOfWeek.AddDays(6);

                var count = await _courtCallerDbContext.Bookings
                    .Where(m => m.BookingDate >= startOfWeek && m.BookingDate < endOfWeek)
                    .CountAsync();

                bookingCounts.Add(count);
            }

            return bookingCounts.ToArray();
        }

    }
}
