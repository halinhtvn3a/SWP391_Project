using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using DAOs.Models;
using Microsoft.Extensions.Logging;

namespace Services.SignalRHub
{
    public class TimeSlotHub : Hub
    {
        private readonly TimeSlotService _timeSlotService;
        private readonly ILogger<TimeSlotHub> _logger;
        public TimeSlotHub(TimeSlotService timeSlotService, ILogger<TimeSlotHub> logger)
        {
            _timeSlotService = timeSlotService;
            _logger = logger;
        }

        [HubMethodName("DisableSlot")]
        public async Task DisableSlot(SlotCheckModel slotCheckModel)
        {
            _logger.LogInformation("Received DisableSlot call");
            _logger.LogInformation($"BranchId: {slotCheckModel.BranchId}, SlotDate: {slotCheckModel.SlotDate}, StartTime: {slotCheckModel.TimeSlot.SlotStartTime}, EndTime: {slotCheckModel.TimeSlot.SlotEndTime}");
            await Clients.All.SendAsync("SenDisableSlot", slotCheckModel);

            //int count = _timeSlotService.CountTimeSlot(slotCheckModel);
            //if (count <= 0)
            //{
            //    // Nếu thỏa mãn điều kiện, gửi lại dữ liệu cho tất cả các client
              
            //    _logger.LogInformation("Slot disabled and sent to all clients: " + slotCheckModel);
            //}
            //else
            //{
            //    _logger.LogInformation("Slot not disabled: " + slotCheckModel);
            //}
        }
    }    
}
