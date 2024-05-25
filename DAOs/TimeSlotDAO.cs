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
        private readonly CourtCallerDbContext dbContext = null;

        public TimeSlotDAO()
        {
            if (dbContext == null)
            {
                dbContext = new CourtCallerDbContext();
            }
        }

        public List<TimeSlot> GetTimeSlots()
        {
            return dbContext.TimeSlots.ToList();
        }

        public TimeSlot GetTimeSlot(string id)
        {
            return dbContext.TimeSlots.FirstOrDefault(m => m.SlotId.Equals(id));
        }

        public TimeSlot AddTimeSlot(TimeSlot TimeSlot)
        {
            dbContext.TimeSlots.Add(TimeSlot);
            dbContext.SaveChanges();
            return TimeSlot;
        }

        public TimeSlot UpdateTimeSlot(string id, TimeSlot TimeSlot)
        {
            TimeSlot oTimeSlot = GetTimeSlot(id);
            if (oTimeSlot != null)
            {
                oTimeSlot.IsAvailable = false;
                dbContext.Update(oTimeSlot);
                dbContext.SaveChanges();
            }
            return oTimeSlot;
        }

        //public void DeleteTimeSlot(int id)
        //{
        //    TimeSlot oTimeSlot = GetTimeSlot(id);
        //    if (oTimeSlot != null)
        //    {
        //        dbContext.Remove(oTimeSlot);
        //        dbContext.SaveChanges();
        //    }
        //}
    }
}
