using DAOs;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Helper;
using DAOs.Models;

namespace DAOs
{
    public class CourtDAO
    {
        private readonly CourtCallerDbContext _courtCallerDbContext = null;

        public CourtDAO()
        {
            if (_courtCallerDbContext == null)
            {
                _courtCallerDbContext = new CourtCallerDbContext();
            }
        }

       public async Task<List<Court>> GetCourts (PageResult pageResult)
        {
            var query = _courtCallerDbContext.Courts.AsQueryable();
            Pagination pagination = new Pagination(_courtCallerDbContext);
            List<Court> courts = await pagination.GetListAsync<Court>(query, pageResult);
            return courts;

        }


        public Court GetCourt(string id)
        {
            return _courtCallerDbContext.Courts.FirstOrDefault(m => m.CourtId.Equals(id));
        }

        public Court AddCourt(CourtModel courtModel)
        {
            Court Court = new Court()
            {
                CourtId = "C" + (_courtCallerDbContext.Courts.Count() + 1).ToString("D5"),
                CourtName = courtModel.CourtName,
                BranchId = courtModel.BranchId,
                CourtPicture = courtModel.CourtPicture,
                Status = courtModel.Status
            };

            _courtCallerDbContext.Courts.Add(Court);
            _courtCallerDbContext.SaveChanges();
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
                _courtCallerDbContext.Update(oCourt);
                _courtCallerDbContext.SaveChanges();
            }
            return oCourt;
        }

        public void DeleteCourt(string id)
        {
            Court oCourt = GetCourt(id);
            if (oCourt != null)
            {
                oCourt.Status = "Inactive";
                _courtCallerDbContext.Update(oCourt);
                _courtCallerDbContext.SaveChanges();
            }
        }

        public List<Court> GetCourtsByBranchId(string branchId) => _courtCallerDbContext.Courts.Where(m => m.BranchId.Equals(branchId)).ToList();
        public List<Court> GetActiveCourts() => _courtCallerDbContext.Courts.Where(m => m.Status.Equals("Active")).ToList();
    }
}
