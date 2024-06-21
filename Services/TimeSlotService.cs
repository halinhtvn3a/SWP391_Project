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
        
        public async Task<List<TimeSlot>> GetTimeSlots(PageResult pageResult, string searchQuery = null) => await _timeSlotRepository.GetTimeSlots(pageResult,searchQuery);
        public async Task<List<TimeSlot>> GetTimeSlotsByUserId(string userId, PageResult pageResult) => await _timeSlotRepository.GetTimeSlotsByUserId(userId, pageResult);
        public async Task<TimeSlot> GetTimeSlot(string id) => await _timeSlotRepository.GetTimeSlot(id);
        public List<TimeSlot> GetTimeSlots() => _timeSlotRepository.GetTimeSlots();
        public async Task< TimeSlot> UpdateTimeSlot(string id, SlotModel slotModel) => await _timeSlotRepository.UpdateTimeSlot(id, slotModel);

        public async Task UpdateTimeSlotWithObject(TimeSlot timeSlot) => await  _timeSlotRepository.UpdateTimeSlotWithObject(timeSlot);
        public List<TimeSlot> GetTimeSlotsByBookingId(string bookingId) => _timeSlotRepository.GetTimeSlotsByBookingId(bookingId);

        public bool IsSlotBookedInBranch(SlotModel slotModel) => _timeSlotRepository.IsSlotBookedInBranch(slotModel);

        public TimeSlot ChangeSlot(SlotModel slotModel, string slotId) => _timeSlotRepository.ChangeSlot(slotModel, slotId);

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
    }

}
