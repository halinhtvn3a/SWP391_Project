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
        
        public async Task DisableSlot(SlotCheckModel slotCheckModel)
        {
            await Clients.All.SendAsync("DisableSlot", slotCheckModel);
        }
    }    
}
