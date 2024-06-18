using BusinessObjects;
using DAOs.Models;
using DAOs;
using DAOs.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class BranchRepository
    {
        private readonly BranchDAO _branchDao = null;
        private readonly BookingDAO _bookingDao = null;
        private readonly TimeSlotDAO _timeSlotDao = null;
        private readonly CourtDAO _courtDao = null;
        public BranchRepository()
        {
            if (_branchDao == null)
            {
                _branchDao = new BranchDAO();
            }

            if (_bookingDao == null)
            {
                _bookingDao = new BookingDAO();
            }

            if (_timeSlotDao == null)
            {
                _timeSlotDao = new TimeSlotDAO();
            }

            if (_courtDao == null)
            {
                _courtDao = new CourtDAO();
            }
        }
        public Branch AddBranch(BranchModel branchModel) => _branchDao.AddBranch(branchModel);

        public void DeleteBranch(string id) => _branchDao.DeleteBranch(id);

        public Branch GetBranch(string id) => _branchDao.GetBranch(id);

        public async Task<List<Branch>> GetBranches(PageResult pageResult, string searchQuery = null) => await _branchDao.GetBranches(pageResult,searchQuery);

        public Branch UpdateBranch(string id, BranchModel branchModel) => _branchDao.UpdateBranch(id, branchModel);

        public List<Branch> GetBranchesByStatus(string status) => _branchDao.GetBranchesByStatus(status);

        public List<Branch> GetBranchesByCourtId(string courtId) => _branchDao.GetBranchesByCourtId(courtId);

        public List<Branch> GetBranchByPrice(decimal minPrice, decimal maxPrice) => _branchDao.GetBranchByPrice(minPrice, maxPrice);

        public Branch GetLastBranch(string userId)
        {
            List<Booking> bookings = _bookingDao.GetBookingsByUserId(userId);
            if (bookings.Count == 0)
            {
                return null;
            }
            Booking lastBooking = bookings.OrderByDescending(b => b.BookingDate).First();
            // Corrected to use lastBooking.BookingId instead of userId
            TimeSlot timeSlot = _timeSlotDao.GetTimeSlotsByBookingId(lastBooking.BookingId).First();
            Court court = _courtDao.GetCourt(timeSlot.CourtId);
            return _branchDao.GetBranch(court.BranchId);
        }
        public async Task<List<Branch>> SortBranch(string? sortBy, bool isAsc, PageResult pageResult) => await _branchDao.SortBranch(sortBy, isAsc, pageResult);
    }
}
