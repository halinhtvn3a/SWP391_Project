using BusinessObjects;
using BusinessObjects.Models;
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
        private readonly CourtRepository CourtRepository = null;
        public CourtService()
        {
            if (CourtRepository == null)
            {
                CourtRepository = new CourtRepository();
            }
        }
        public Court AddCourt(CourtModel courtModel) => CourtRepository.AddCourt(courtModel);
        public void DeleteCourt(string id) => CourtRepository.DeleteCourt(id);
        public Court GetCourt(string id) => CourtRepository.GetCourt(id);
        public async Task<List<Court>> GetCourts(PageResult pageResult) => await CourtRepository.GetCourts(pageResult);
        public Court UpdateCourt(string id, CourtModel courtModel) => CourtRepository.UpdateCourt(id, courtModel);
        public List<Court> GetActiveCourts() => CourtRepository.GetActiveCourts();

    }
}
