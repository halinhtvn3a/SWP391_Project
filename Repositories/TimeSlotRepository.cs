using BusinessObjects;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class TimeSlotRepository
    {
        private readonly TimeSlotDAO _timeSlotDao = null;
        public TimeSlotRepository()
        {
            if (_timeSlotDao == null)
            {
                _timeSlotDao = new TimeSlotDAO();
            }
        }
        public TimeSlot AddTimeSlot(TimeSlot TimeSlot) => _timeSlotDao.AddTimeSlot(TimeSlot);

        //public void DeleteTimeSlot(string id) => TimeSlotDAO.DeleteTimeSlot(id);

        public TimeSlot GetTimeSlot(string id) => _timeSlotDao.GetTimeSlot(id);

        public List<TimeSlot> GetTimeSlots() => _timeSlotDao.GetTimeSlots();

        public TimeSlot UpdateTimeSlot(string id, TimeSlot TimeSlot) => _timeSlotDao.UpdateTimeSlot(id, TimeSlot);
    }
}
