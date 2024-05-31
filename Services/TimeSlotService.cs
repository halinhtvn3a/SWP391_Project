using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class TimeSlotService
    {
        private readonly TimeSlotRepository TimeSlotRepository = null;
        public TimeSlotService()
        {
            if (TimeSlotRepository == null)
            {
                TimeSlotRepository = new TimeSlotRepository();
            }
        }
        public TimeSlot AddTimeSlot(TimeSlot TimeSlot) => TimeSlotRepository.AddTimeSlot(TimeSlot);
        //public void DeleteTimeSlot(string id) => TimeSlotRepository.DeleteTimeSlot(id);
        public TimeSlot GetTimeSlot(string id) => TimeSlotRepository.GetTimeSlot(id);
        public List<TimeSlot> GetTimeSlots() => TimeSlotRepository.GetTimeSlots();
        public TimeSlot UpdateTimeSlot(string id, TimeSlot TimeSlot) => TimeSlotRepository.UpdateTimeSlot(id, TimeSlot);
    }
}
