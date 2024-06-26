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
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;
using Microsoft.AspNetCore.SignalR;
using Services.SignalRHub;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class TimeSlotService
    {
        private readonly TimeSlotRepository _timeSlotRepository = null;
        private readonly IHubContext<TimeSlotHub> _hubContext;
        public TimeSlotService(TimeSlotRepository timeSlotRepository, IHubContext<TimeSlotHub> hubContext)
        {
            _timeSlotRepository = timeSlotRepository;
            _hubContext = hubContext;
        }
        public TimeSlot AddTimeSlot(TimeSlot timeSlot) => _timeSlotRepository.AddTimeSlot(timeSlot);
        //public void DeleteTimeSlot(string id) => TimeSlotRepository.DeleteTimeSlot(id);
        
        public async Task<List<TimeSlot>> GetTimeSlots(PageResult pageResult, string searchQuery = null) => await _timeSlotRepository.GetTimeSlots(pageResult,searchQuery);
        public async Task<List<TimeSlot>> GetTimeSlotsByUserId(string userId, PageResult pageResult) => await _timeSlotRepository.GetTimeSlotsByUserId(userId, pageResult);
        public async Task<TimeSlot> GetTimeSlot(string id) => await _timeSlotRepository.GetTimeSlot(id);
        public List<TimeSlot> GetTimeSlots() => _timeSlotRepository.GetTimeSlots();
        public async Task< TimeSlot> UpdateTimeSlot(string id, SlotModel slotModel) => await _timeSlotRepository.UpdateTimeSlot(id, slotModel);

        public async Task UpdateTimeSlotWithObject(TimeSlot timeSlot) => await  _timeSlotRepository.UpdateTimeSlotWithObject(timeSlot);
        public List<TimeSlot> GetTimeSlotsByBookingId(string bookingId) => _timeSlotRepository.GetTimeSlotsByBookingId(bookingId);

        public bool IsSlotBookedInBranch(SlotModel slotModel) => _timeSlotRepository.IsSlotBookedInBranch(slotModel);

        public bool IsSlotBookedInBranchV2(SlotModel slotModel) => _timeSlotRepository.IsSlotBookedInBranchV2(slotModel);
        public TimeSlot ChangeSlot(SlotModel slotModel, string slotId) => _timeSlotRepository.ChangeSlot(slotModel, slotId);

        public List<TimeSlot> GetTimeSlotsByDate(DateOnly dateOnly) => _timeSlotRepository.GetTimeSlotsByDate(dateOnly);
        public async Task<List<TimeSlot>> SortTimeSlot(string? sortBy, bool isAsc, PageResult pageResult) => await _timeSlotRepository.SortTimeSlot(sortBy, isAsc, pageResult);

        public string GenerateQRCode(string qrData)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            using (var ms = new MemoryStream())
            {
                qrCodeImage.Save(ms, ImageFormat.Png);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        //public async Task<List<SlotModel>> GetLockedSlots()
        //{
        //    var lockedTimeSlots = await _timeSlotRepository.GetLockedTimeSlots();
        //    return lockedTimeSlots.Select(t => new SlotModel
        //    {
        //        CourtId = t.CourtId,
        //        BranchId = t,
        //        SlotDate = t.SlotDate,
        //        TimeSlot = new TimeSlotModel
        //        {
        //            SlotStartTime = t.SlotStartTime,
        //            SlotEndTime = t.SlotEndTime
        //        }
        //    }).ToList();
        //}

        public List<TimeSlot> UnavailableSlot(DateOnly date, string branchId) => _timeSlotRepository.UnavailableSlot(date, branchId);

        public int CountTimeSlot(SlotCheckModel slotCheckModel) => _timeSlotRepository.CountTimeSlot(slotCheckModel);
        public async Task ConfirmBooking(SlotCheckModel slotCheckModel)
        {
            // Kiểm tra và cập nhật TimeSlots
            var count = _timeSlotRepository.CountTimeSlot(slotCheckModel);
            Console.WriteLine($"Count for slot {slotCheckModel.TimeSlot.SlotStartTime} to {slotCheckModel.TimeSlot.SlotEndTime} on {slotCheckModel.SlotDate}: {count}");

            if (count == 0)
            {
                Console.WriteLine("Slot is no longer available. Sending notification to clients.");

               
                await _hubContext.Clients.All.SendAsync("DisableSlot", slotCheckModel);
            }
        }

        public TimeSlot AddSlotToBooking(SlotModel slotModel, string bookingId) => _timeSlotRepository.AddSlotToBooking(slotModel, bookingId);

    }

}
