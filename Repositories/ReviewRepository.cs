using BusinessObjects;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<Review> GetReviews() => ReviewDAO.GetReviews();

        public Review UpdateReview(string id, Review Review) => ReviewDAO.UpdateReview(id, Review);

        public List<Review> GetReviewsByCourt(string id) => ReviewDAO.GetReviewsByCourt(id);
        public List<Review> SearchByUser(string id) => ReviewDAO.SearchByUser(id);

        public List<Review> SearchByDate(DateTime start, DateTime end) => ReviewDAO.SearchByDate(start, end);

        public List<Review> SearchByRating(int rating) => ReviewDAO.SearchByRating(rating);
    }
}
