using BusinessObjects;
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
        public Court AddCourt(Court Court) => _courtDao.AddCourt(Court);

        public void DeleteCourt(string id) => _courtDao.DeleteCourt(id);

        public Court GetCourt(string id) => _courtDao.GetCourt(id);

        public async Task<List<Court>> GetCourts(PageResult pageResult) => await _courtDao.GetCourts(pageResult);

        public Court UpdateCourt(string id, Court Court) => _courtDao.UpdateCourt(id, Court);

        public List<Court> GetActiveCourts() => _courtDao.GetActiveCourts();

    }
}
