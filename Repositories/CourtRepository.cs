using BusinessObjects;
using DAOs.Models;
using DAOs;
using DAOs.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CourtRepository
    {
        private readonly CourtDAO _courtDao = null;
        public CourtRepository()
        {
            if (_courtDao == null)
            {
                _courtDao = new CourtDAO();
            }
        }
        public Court AddCourt(CourtModel courtModel) => _courtDao.AddCourt(courtModel);

        public void DeleteCourt(string id) => _courtDao.DeleteCourt(id);

        public Court GetCourt(string id) => _courtDao.GetCourt(id);

        public async Task<List<Court>> GetCourts(PageResult pageResult, string searchQuery = null) => await _courtDao.GetCourts(pageResult,searchQuery);

        public Court UpdateCourt(string id, CourtModel courtModel) => _courtDao.UpdateCourt(id, courtModel);

        public List<Court> GetActiveCourts() => _courtDao.GetActiveCourts();

        public async Task<List<Court>> SortCourt(string? sortBy, bool isAsc, PageResult pageResult) => await _courtDao.SortCourt(sortBy, isAsc, pageResult);
    }
}
