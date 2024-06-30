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
            Console.WriteLine("Received DisableSlot call");
            Console.WriteLine($"BranchId: {slotCheckModel.BranchId}, SlotDate: {slotCheckModel.SlotDate}, StartTime: {slotCheckModel.TimeSlot.SlotStartTime}, EndTime: {slotCheckModel.TimeSlot.SlotEndTime}");

            int count = _timeSlotService.CountTimeSlot(slotCheckModel);
            if (count <= 0)
            {
                // Nếu thỏa mãn điều kiện, gửi lại dữ liệu cho tất cả các client
                await Clients.All.SendAsync("DisableSlot", slotCheckModel);
                Console.WriteLine("Slot disabled and sent to all clients: " + slotCheckModel);
            }
            else
            {
                Console.WriteLine("Slot not disabled: " + slotCheckModel);
            }
        }
    }    
}
