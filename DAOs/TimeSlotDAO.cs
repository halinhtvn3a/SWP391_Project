using BusinessObjects;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs
{
    public class TimeSlotDAO
    {
        private readonly CourtCallerDbContext DbContext = null;

        public TimeSlotDAO()
        {
            if (DbContext == null)
            {
                DbContext = new CourtCallerDbContext();
            }
        }

        public List<TimeSlot> GetTimeSlots()
        {
            return DbContext.TimeSlots.ToList();
        }

        public TimeSlot GetTimeSlot(string id)
        {
            return DbContext.TimeSlots.FirstOrDefault(m => m.SlotId.Equals(id));
        }

        public TimeSlot AddTimeSlot(TimeSlot TimeSlot)
        {
            DbContext.TimeSlots.Add(TimeSlot);
            DbContext.SaveChanges();
            return TimeSlot;
        }

        public TimeSlot UpdateTimeSlot(string id, TimeSlot TimeSlot)
        {
            TimeSlot oTimeSlot = GetTimeSlot(id);
            if (oTimeSlot != null)
            {
                oTimeSlot.IsAvailable = false;
                oTimeSlot.Price = TimeSlot.Price;
                DbContext.Update(oTimeSlot);
                DbContext.SaveChanges();
            }
            return oTimeSlot;
        }

        public void UpdateSlot(TimeSlot slot)
        {
            DbContext.TimeSlots.Update(slot);
        }

        //public void DeleteTimeSlot(int id)
        //{
        //    TimeSlot oTimeSlot = GetTimeSlot(id);
        //    if (oTimeSlot != null)
        //    {
        //        DbContext.Remove(oTimeSlot);
        //        DbContext.SaveChanges();
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
                    DbContext.Update(oTimeSlot);
                    DbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
