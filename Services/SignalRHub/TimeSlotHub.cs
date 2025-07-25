﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Models;
using Microsoft.AspNetCore.SignalR;
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
            _logger.LogInformation(
                $"BranchId: {slotCheckModel.BranchId}, SlotDate: {slotCheckModel.SlotDate}, StartTime: {slotCheckModel.TimeSlot.SlotStartTime}, EndTime: {slotCheckModel.TimeSlot.SlotEndTime}"
            );

            try
            {
                _logger.LogInformation("Sending DisableSlot to all clients");

                _logger.LogInformation("DisableSlot sent successfully");

                //await Clients.All.SendAsync("RefreshCourt");

                int count = _timeSlotService.CountTimeSlot(slotCheckModel);
                if (count <= 0)
                {
                    await Clients.All.SendAsync("DisableSlot", slotCheckModel);
                    _logger.LogInformation(
                        "Slot disabled and sent to all clients: " + slotCheckModel
                    );
                }
                else
                {
                    _logger.LogInformation("Slot not disabled: " + slotCheckModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending DisableSlot: {ex.Message}");
            }
        }

        [HubMethodName("RefreshCourt")]
        public async Task RefreshCourt(CourtAvailableCheckModel slotCheckModel)
        {
            _logger.LogInformation("Received DisableSlot call");
            _logger.LogInformation(
                $"BranchId: {slotCheckModel.BranchId}, SlotDate: {slotCheckModel.SlotDate}, StartTime: {slotCheckModel.TimeSlot.SlotStartTime}, EndTime: {slotCheckModel.TimeSlot.SlotEndTime}"
            );

            try
            {
                _logger.LogInformation("Sending DisableSlot to all clients");

                _logger.LogInformation("DisableSlot sent successfully");

                await Clients.All.SendAsync("RefreshCourt");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending DisableSlot: {ex.Message}");
            }
        }
    }
}
