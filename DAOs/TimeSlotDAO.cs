using BusinessObjects;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using DAOs.Helper;

namespace DAOs
{
    public class TimeSlotDAO
    {
        private readonly CourtCallerDbContext _dbContext = null;

        public TimeSlotDAO()
        {
            if (_dbContext == null)
            {
                _dbContext = new CourtCallerDbContext();
            }
        }

        public List<TimeSlot> GetTimeSlots()
        {
            return _dbContext.TimeSlots.ToList();
        }

        public TimeSlot GetTimeSlot(string id)
        {
            return _dbContext.TimeSlots.FirstOrDefault(m => m.SlotId.Equals(id));
        }

        public TimeSlot AddTimeSlot(TimeSlot TimeSlot)
        {
            _dbContext.TimeSlots.Add(TimeSlot);
            _dbContext.SaveChanges();
            return TimeSlot;
        }

        public TimeSlot UpdateTimeSlot(string id, TimeSlot TimeSlot)
        {
            TimeSlot oTimeSlot = GetTimeSlot(id);
            if (oTimeSlot != null)
            {
                oTimeSlot.IsAvailable = false;
                oTimeSlot.Price = TimeSlot.Price;
                _dbContext.Update(oTimeSlot);
                _dbContext.SaveChanges();
            }
            return oTimeSlot;
        }

        public void UpdateSlot(TimeSlot slot)
        {
            _dbContext.TimeSlots.Update(slot);
        }

        //public void DeleteTimeSlot(int id)
        //{
        //    TimeSlot oTimeSlot = GetTimeSlot(id);
        //    if (oTimeSlot != null)
        //    {
        //        _dbContext.Remove(oTimeSlot);
        //        _dbContext.SaveChanges();
        //    }
        //}

        public void UpdateBookinginSlot(string slotId, string bookingId)
        {
            try
            {
                TimeSlot oTimeSlot = GetTimeSlot(slotId);
                if (oTimeSlot != null)
                {
                    oTimeSlot.BookingId = bookingId;
                    _dbContext.Update(oTimeSlot);
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetDayOfWeek(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
            {
                var dayOfWeek = parsedDate.DayOfWeek;
                return dayOfWeek.ToString();
            }
            else
            {
                return null;
            }
        }


        //V2
        public List<TimeSlot> GetTimeSlotsByDate(DateOnly dateOnly)
        {
            return _dbContext.TimeSlots.Where(t => t.SlotDate.Equals(dateOnly)).ToList();
        }

        public bool IsSlotAvailable(SlotModel slotModel)
        {
            List<TimeSlot> timeSlots = GetTimeSlotsByDate(slotModel.SlotDate);

            if (!timeSlots.Any())
            {
                return true;
            }

            foreach (var slot in timeSlots)
            {
                if (slot.SlotStartTime.Equals(slotModel.SlotStartTime) && slot.SlotEndTime.Equals(slotModel.SlotEndTime))
                {
                    return false;
                }
            }

            
            return true;
        }

        public TimeSlot AddSlotToBooking(SlotModel slotModel, string bookingId)
        {
            if (slotModel == null || bookingId == null)
            {
                throw new ArgumentNullException("slotModel or bookingId is null");
            }

            TimeSlot timeSlot = new TimeSlot
            {
                SlotId = "S" + GenerateId.GenerateShortBookingId(),
                CourtId = slotModel.CourtId,
                BookingId = bookingId,
                SlotDate = slotModel.SlotDate,
                Price = (slotModel.SlotDate.DayOfWeek == DayOfWeek.Sunday || slotModel.SlotDate.DayOfWeek == DayOfWeek.Saturday) ? 100 : 50,
                SlotStartTime = slotModel.SlotStartTime,
                SlotEndTime = slotModel.SlotEndTime,
                IsAvailable = false
            };

            try
            {
                _dbContext.TimeSlots.Add(timeSlot);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                // Handle exception
                throw;
            }

            return timeSlot;
        }

    }
}
