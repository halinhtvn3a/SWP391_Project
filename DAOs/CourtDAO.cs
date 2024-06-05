using DAOs;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Helper;

namespace DAOs
{
    public class CourtDAO
    {
        private readonly CourtCallerDbContext DbContext = null;

        public CourtDAO()
        {
            if (DbContext == null)
            {
                DbContext = new CourtCallerDbContext();
            }
        }

       public async Task<List<Court>> GetCourts (PageResult pageResult)
        {
            var query = DbContext.Courts.AsQueryable();
            Pagination pagination = new Pagination(DbContext);
            List<Court> courts = await pagination.GetListAsync<Court>(query, pageResult);
            return courts;

        }


        public Court GetCourt(string id)
        {
            return DbContext.Courts.FirstOrDefault(m => m.CourtId.Equals(id));
        }

        public Court AddCourt(Court Court)
        {
            DbContext.Courts.Add(Court);
            DbContext.SaveChanges();
            return Court;
        }

        public Court UpdateCourt(string id, Court Court)
        {
            Court oCourt = GetCourt(id);
            if (oCourt != null)
            {
                oCourt.CourtName = Court.CourtName;
                oCourt.Status = Court.Status;
                DbContext.Update(oCourt);
                DbContext.SaveChanges();
            }
            return oCourt;
        }

        public void DeleteCourt(string id)
        {
            Court oCourt = GetCourt(id);
            if (oCourt != null)
            {
                oCourt.Status = "Deleted";
                DbContext.Update(oCourt);
                DbContext.SaveChanges();
            }
        }

        public List<Court> GetActiveCourts() => DbContext.Courts.Where(m => m.Status.Equals(true)).ToList();
    }
}
