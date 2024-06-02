using BusinessObjects;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Helper;

namespace Repositories
{
    public class ReviewRepository
    {
        private readonly ReviewDAO ReviewDAO = null;
        public ReviewRepository()
        {
            if (ReviewDAO == null)
            {
                ReviewDAO = new ReviewDAO();
            }
        }
        public Review AddReview(Review Review) => ReviewDAO.AddReview(Review);

        public void DeleteReview(string id) => ReviewDAO.DeleteReview(id);

        public Review GetReview(string id) => ReviewDAO.GetReview(id);

        public async Task<List<Review>> GetReview(PageResult pageResult) => await ReviewDAO.GetReview(pageResult);

        public Review UpdateReview(string id, Review Review) => ReviewDAO.UpdateReview(id, Review);
    }
}
