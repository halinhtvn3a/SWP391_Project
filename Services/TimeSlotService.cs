using BusinessObjects;
using DAOs;
using DAOs.Models;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Helper;

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
        
        public async Task<List<TimeSlot>> GetTimeSlots(PageResult pageResult) => await _timeSlotRepository.GetTimeSlots(pageResult);
        public async Task<List<TimeSlot>> GetTimeSlotsByUserId(string userId, PageResult pageResult) => await _timeSlotRepository.GetTimeSlotsByUserId(userId, pageResult);
        public TimeSlot GetTimeSlot(string id) => _timeSlotRepository.GetTimeSlot(id);
        public List<TimeSlot> GetTimeSlots() => _timeSlotRepository.GetTimeSlots();
        public TimeSlot UpdateTimeSlot(string id, SlotModel slotModel) => _timeSlotRepository.UpdateTimeSlot(id, slotModel);

        public List<TimeSlot> GetTimeSlotsByBookingId(string bookingId) => _timeSlotRepository.GetTimeSlotsByBookingId(bookingId);

        public bool IsSlotBookedInBranch(SlotModel slotModel) => _timeSlotRepository.IsSlotBookedInBranch(slotModel);

        public TimeSlot ChangeSlot(SlotModel slotModel, string slotId) => _timeSlotRepository.ChangeSlot(slotModel, slotId);

        public async Task<List<TimeSlot>> SortTimeSlot(string? sortBy, bool isAsc, PageResult pageResult) => await _timeSlotRepository.SortTimeSlot(sortBy, isAsc, pageResult);
    }
}
