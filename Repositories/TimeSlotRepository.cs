using BusinessObjects;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

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
        public TimeSlot AddTimeSlot(TimeSlot TimeSlot) => _timeSlotDao.AddTimeSlot(TimeSlot);

        //public void DeleteTimeSlot(string id) => TimeSlotDAO.DeleteTimeSlot(id);

        public TimeSlot GetTimeSlot(string id) => _timeSlotDao.GetTimeSlot(id);

        public List<TimeSlot> GetTimeSlots() => _timeSlotDao.GetTimeSlots();

        public TimeSlot UpdateTimeSlot(string id, TimeSlot TimeSlot) => _timeSlotDao.UpdateTimeSlot(id, TimeSlot);

        public bool IsSlotBookedInBranch(SlotModel slotModel)
        {
            bool isAvailable;
            if (slotModel.BranchId == null)
            {
                isAvailable = _timeSlotDao.GetTimeSlots().Any(t =>
                    t.CourtId == slotModel.CourtId &&
                    t.SlotDate == slotModel.SlotDate &&
                    t.SlotStartTime == slotModel.TimeSlot.SlotStartTime &&
                    t.SlotEndTime == slotModel.TimeSlot.SlotEndTime);
            }
            else
            {
                isAvailable = !_timeSlotDao.GetTimeSlots()
                                  .Any(t => _courtDao.GetCourt(t.CourtId).BranchId == slotModel.BranchId &&
                              t.SlotDate == slotModel.SlotDate &&
                              t.SlotStartTime == slotModel.TimeSlot.SlotStartTime &&
                              t.SlotEndTime == slotModel.TimeSlot.SlotEndTime);

            }
            return isAvailable;
        }
    }
}
