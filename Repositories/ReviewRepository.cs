using BusinessObjects;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Helper;
using DAOs.Models;

namespace Repositories
{
    public class ReviewRepository
    {
        private readonly ReviewDAO _reviewDao = null;
        public ReviewRepository()
        {
            if (_reviewDao == null)
            {
                _reviewDao = new ReviewDAO();
            }
        }
        public Review AddReview(ReviewModel reviewModel) => _reviewDao.AddReview(reviewModel);

        public void DeleteReview(string id) => _reviewDao.DeleteReview(id);

        public Review GetReview(string id) => _reviewDao.GetReview(id);

        public async Task<List<Review>> GetReview(PageResult pageResult, string searchQuery = null) => await _reviewDao.GetReview(pageResult,searchQuery);

        public Review UpdateReview(string id, ReviewModel reviewModel) => _reviewDao.UpdateReview(id, reviewModel);

        public List<Review> GetReviewsByBranch(string id) => _reviewDao.GetReviewsByBranch(id);
        public List<Review> SearchByUser(string id) => _reviewDao.SearchByUser(id);

        public List<Review> SearchByDate(DateTime start, DateTime end) => _reviewDao.SearchByDate(start, end);

        public List<Review> SearchByRating(int rating) => _reviewDao.SearchByRating(rating);

        public async Task<List<Review>> SortReview(string? sortBy, bool isAsc, PageResult pageResult) => await _reviewDao.SortReview(sortBy, isAsc, pageResult);
    }
}
