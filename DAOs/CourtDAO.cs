using DAOs;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Helper;
using BusinessObjects.Models;

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

        public Court AddCourt(CourtModel courtModel)
        {
            Court Court = new Court()
            {
                CourtId = "C" + (DbContext.Courts.Count() + 1).ToString("D5"),
                CourtName = courtModel.CourtName,
                BranchId = courtModel.BranchId,
                CourtPicture = courtModel.CourtPicture,
                Status = courtModel.Status
            };

            DbContext.Courts.Add(Court);
            DbContext.SaveChanges();
            return Court;
        }

        public Court UpdateCourt(string id, CourtModel courtModel)
        {
            Court oCourt = GetCourt(id);
            if (oCourt != null)
            {
                oCourt.CourtName = courtModel.CourtName;
                oCourt.BranchId = courtModel.BranchId;
                oCourt.CourtPicture = courtModel.CourtPicture;
                oCourt.Status = courtModel.Status;
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
                oCourt.Status = "Inactive";
                DbContext.Update(oCourt);
                DbContext.SaveChanges();
            }
        }

        public List<Court> GetActiveCourts() => DbContext.Courts.Where(m => m.Status.Equals("Active")).ToList();
    }
}
