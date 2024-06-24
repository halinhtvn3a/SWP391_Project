using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using DAOs.Models;

namespace Services.SignalRHub
{
    public class TimeSlotHub : Hub
    {
        private readonly TimeSlotService _timeSlotService;

        public TimeSlotHub(TimeSlotService timeSlotService)
        {
            _timeSlotService = timeSlotService;
        }
        public async Task LockSlot(SlotModel slotModel)
        {

            bool isBooked = _timeSlotService.IsSlotBookedInBranch(new SlotModel
            {
                CourtId = slotModel.CourtId,
                BranchId = slotModel.BranchId,
                SlotDate = slotModel.SlotDate,
                TimeSlot = new TimeSlotModel
                {
                    SlotStartTime = slotModel.TimeSlot.SlotStartTime,
                    SlotEndTime = slotModel.TimeSlot.SlotEndTime
                }
            });

            if (!isBooked)
            {
                await Clients.All.SendAsync("LockingSlot", slotModel);
            }
            else
            {
                throw new HubException("Slot is already booked.");
            }
        }

        public async Task ReleaseSlot(SlotModel slotModel)
        {
            await Clients.All.SendAsync("ReleaseSlot", slotModel);
        }
    }    
}
