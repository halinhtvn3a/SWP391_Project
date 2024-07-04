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

        public CourtDAO(CourtCallerDbContext courtCallerDbContext)
        {
            _courtCallerDbContext = courtCallerDbContext;
        }

       public async Task<List<Court>> GetCourts (PageResult pageResult, string searchQuery = null)
        {
            var query = _courtCallerDbContext.Courts.AsQueryable();


            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(court => court.CourtId.Contains(searchQuery) ||
                                              court.BranchId.Contains(searchQuery) ||
                                              court.CourtName.Contains(searchQuery) ||
                                              court.Status.Contains(searchQuery));
                                              
            }

            Pagination pagination = new Pagination(_courtCallerDbContext);
            List<Court> courts = await pagination.GetListAsync<Court>(query, pageResult);
            return courts;

        }

        public List<Court> GetCourts()
        {
            return _courtCallerDbContext.Courts.ToList();
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

        public int GetNumberOfCourtsByBranchId(string branchId) => _courtCallerDbContext.Courts.Where(m => m.BranchId.Equals(branchId)).Count();


        public List<Court> GetCourtsByStatus(string status) => _courtCallerDbContext.Courts.Where(m => m.Status.Equals(status)).ToList();

        public async Task<List<Court>> SortCourt(string? sortBy, bool isAsc, PageResult pageResult)
        {
            IQueryable<Court> query = _courtCallerDbContext.Courts;

            switch (sortBy?.ToLower())
            {
                case "branchid":
                    query = isAsc ? query.OrderBy(b => b.BranchId) : query.OrderByDescending(b => b.BranchId);
                    break;
                case "courtid":
                    query = isAsc ? query.OrderBy(b => b.CourtId) : query.OrderByDescending(b => b.CourtId);
                    break;
                case "courtname":
                    query = isAsc ? query.OrderBy(b => b.CourtName) : query.OrderByDescending(b => b.CourtName);
                    break;
                case "courtpicture":
                    query = isAsc ? query.OrderBy(b => b.CourtPicture) : query.OrderByDescending(b => b.CourtPicture);
                    break;
                case "status":
                    query = isAsc ? query.OrderBy(b => b.Status) : query.OrderByDescending(b => b.Status);
                    break;
                default:
                    break;
            }
            Pagination pagination = new Pagination(_courtCallerDbContext);
            List<Court> courts = await pagination.GetListAsync<Court>(query, pageResult);
            return courts;
        }

        public void MaintainCourt(string courtId)
        {
            Court oCourt = GetCourt(courtId);
            if (oCourt != null)
            {
                oCourt.Status = "Maintaining";
                _courtCallerDbContext.Update(oCourt);
                _courtCallerDbContext.SaveChanges();
            }
        }
    }
}
