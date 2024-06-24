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
        //public async Task LockSlot(SlotModel slotModel)
        //{

        //    await Clients.All.SendAsync("LockingSlot",slotModel);
        //}

        //public async Task ReleaseSlot(SlotModel slotModel)
        //{
        //    await Clients.All.SendAsync("ReleaseSlot", slotModel);
        //}
    }    
}
