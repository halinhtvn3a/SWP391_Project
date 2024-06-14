using BusinessObjects;
using DAOs.Models;
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
        private readonly TimeSlotRepository _timeSlotRepository = null;
        public TimeSlotService()
        {
            if (_timeSlotRepository == null)
            {
                _timeSlotRepository = new TimeSlotRepository();
            }
        }
        public TimeSlot AddTimeSlot(TimeSlot timeSlot) => _timeSlotRepository.AddTimeSlot(timeSlot);
        //public void DeleteTimeSlot(string id) => TimeSlotRepository.DeleteTimeSlot(id);
        public TimeSlot GetTimeSlot(string id) => _timeSlotRepository.GetTimeSlot(id);
        public List<TimeSlot> GetTimeSlots() => _timeSlotRepository.GetTimeSlots();
        public TimeSlot UpdateTimeSlot(string id, TimeSlot timeSlot) => _timeSlotRepository.UpdateTimeSlot(id, timeSlot);

        public List<TimeSlot> GetTimeSlotsByBookingId(string bookingId) => _timeSlotRepository.GetTimeSlotsByBookingId(bookingId);

        public bool IsSlotBookedInBranch(SlotModel slotModel) => _timeSlotRepository.IsSlotBookedInBranch(slotModel);
    }
}
