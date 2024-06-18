using BusinessObjects;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Helper;
using DAOs.Models;

namespace Repositories
{
    public class TimeSlotRepository
    {
        private readonly TimeSlotDAO _timeSlotDao = null;
        private readonly CourtDAO _courtDao = null;
        public TimeSlotRepository()
        {
            if (_timeSlotDao == null)
            {
                _timeSlotDao = new TimeSlotDAO();
            }

            if (_courtDao == null)
            {
                _courtDao = new CourtDAO();
            }
        }
        public TimeSlot AddTimeSlot(TimeSlot timeSlot) => _timeSlotDao.AddTimeSlot(timeSlot);

        public async Task<List<TimeSlot>> GetTimeSlots(PageResult pageResult) => await _timeSlotDao.GetTimeSlots(pageResult);


        public void DeleteTimeSlot(string id) => _timeSlotDao.DeleteTimeSlot(id);

        public TimeSlot GetTimeSlot(string id) => _timeSlotDao.GetTimeSlot(id);

        public List<TimeSlot> GetTimeSlots() => _timeSlotDao.GetTimeSlots();

        public TimeSlot UpdateTimeSlot(string id, SlotModel slotModel) => _timeSlotDao.UpdateTimeSlot(id, slotModel);

        public bool IsSlotBookedInBranch(SlotModel slotModel)
        {
            bool isBooked;
            if (slotModel.BranchId == null)
            {
                isBooked = _timeSlotDao.GetTimeSlots().Any(t =>
                    t.CourtId == slotModel.CourtId &&
                    t.SlotDate == slotModel.SlotDate &&
                    t.SlotStartTime == slotModel.TimeSlot.SlotStartTime &&
                    t.SlotEndTime == slotModel.TimeSlot.SlotEndTime);
            }
            else
            {
                isBooked = !_timeSlotDao.GetTimeSlots()
                                  .Any(t => _courtDao.GetCourt(t.CourtId).BranchId == slotModel.BranchId &&
                              t.SlotDate == slotModel.SlotDate &&
                              t.SlotStartTime == slotModel.TimeSlot.SlotStartTime &&
                              t.SlotEndTime == slotModel.TimeSlot.SlotEndTime);

            }
            return isBooked;
        }

        public List<TimeSlot> GetTimeSlotsByBookingId(string bookingId) => _timeSlotDao.GetTimeSlotsByBookingId(bookingId);

        public TimeSlot ChangeSlot(SlotModel slotModel, string slotId)
        {

            if (IsSlotBookedInBranch(slotModel))
            {
                return null;
            }

            var timeSlot = _timeSlotDao.GetTimeSlots().FirstOrDefault(t =>
                t.SlotId == slotId);

            if (timeSlot != null)
            {
                DeleteTimeSlot(slotId);
                TimeSlot newTimeSlot = _timeSlotDao.AddSlotToBooking(slotModel, timeSlot.BookingId);
                return newTimeSlot;
            }
            return null;
        }
    }
}
