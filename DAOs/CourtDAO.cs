using DAOs;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs
{
    public class CourtDAO
    {
        private readonly CourtCallerDbContext dbContext = null;

        public CourtDAO()
        {
            if (dbContext == null)
            {
                dbContext = new CourtCallerDbContext();
            }
        }

        public List<Court> GetCourts()
        {
            return dbContext.Courts.ToList();
        }

        public Court GetCourt(string id)
        {
            return dbContext.Courts.FirstOrDefault(m => m.CourtId.Equals(id));
        }

        public Court AddCourt(Court Court)
        {
            dbContext.Courts.Add(Court);
            dbContext.SaveChanges();
            return Court;
        }

        public Court UpdateCourt(string id, Court Court)
        {
            Court oCourt = GetCourt(id);
            if (oCourt != null)
            {
                oCourt.CourtName = Court.CourtName;
                oCourt.Status = Court.Status;
                dbContext.Update(oCourt);
                dbContext.SaveChanges();
            }
            return oCourt;
        }

        public void DeleteCourt(string id)
        {
            Court oCourt = GetCourt(id);
            if (oCourt != null)
            {
                oCourt.Status = false;
                dbContext.Update(oCourt);
                dbContext.SaveChanges();
            }
        }
    }
}
