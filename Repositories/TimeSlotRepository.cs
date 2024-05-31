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
        private readonly TimeSlotDAO TimeSlotDAO = null;
        public TimeSlotRepository()
        {
            if (TimeSlotDAO == null)
            {
                TimeSlotDAO = new TimeSlotDAO();
            }
        }
        public TimeSlot AddTimeSlot(TimeSlot TimeSlot) => TimeSlotDAO.AddTimeSlot(TimeSlot);

        //public void DeleteTimeSlot(string id) => TimeSlotDAO.DeleteTimeSlot(id);

        public TimeSlot GetTimeSlot(string id) => TimeSlotDAO.GetTimeSlot(id);

        public List<TimeSlot> GetTimeSlots() => TimeSlotDAO.GetTimeSlots();

        public TimeSlot UpdateTimeSlot(string id, TimeSlot TimeSlot) => TimeSlotDAO.UpdateTimeSlot(id, TimeSlot);
    }
}
