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
        private readonly CourtDAO CourtDAO = null;
        public CourtRepository()
        {
            if (CourtDAO == null)
            {
                CourtDAO = new CourtDAO();
            }
        }
        public Court AddCourt(Court Court) => CourtDAO.AddCourt(Court);

        public void DeleteCourt(string id) => CourtDAO.DeleteCourt(id);

        public Court GetCourt(string id) => CourtDAO.GetCourt(id);

        public async Task<List<Court>> GetCourts(PageResult pageResult) => await CourtDAO.GetCourts(pageResult);

        public Court UpdateCourt(string id, Court Court) => CourtDAO.UpdateCourt(id, Court);
    }
}
