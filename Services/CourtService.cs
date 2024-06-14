using BusinessObjects;
using DAOs.Models;
using DAOs.Helper;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CourtService
    {
        private readonly CourtRepository _courtRepository = null;
        public CourtService()
        {
            if (_courtRepository == null)
            {
                _courtRepository = new CourtRepository();
            }
        }
        public Court AddCourt(CourtModel courtModel) => _courtRepository.AddCourt(courtModel);
        public void DeleteCourt(string id) => _courtRepository.DeleteCourt(id);
        public Court GetCourt(string id) => _courtRepository.GetCourt(id);
        public async Task<List<Court>> GetCourts(PageResult pageResult) => await _courtRepository.GetCourts(pageResult);
        public Court UpdateCourt(string id, CourtModel courtModel) => _courtRepository.UpdateCourt(id, courtModel);
        public List<Court> GetActiveCourts() => _courtRepository.GetActiveCourts();

    }
}
