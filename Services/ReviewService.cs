using BusinessObjects;
using DAOs.Models;
using Repositories;

using HelperResult = DAOs.Helper;

namespace Services
{
    public class ReviewService
    {
        private readonly ReviewRepository _reviewRepository = null;
        public ReviewService()
        {
            if (_reviewRepository == null)
            {
                _reviewRepository = new ReviewRepository();
            }
        }
        public Review AddReview(ReviewModel reviewModel) => _reviewRepository.AddReview(reviewModel);
        public void DeleteReview(string id) => _reviewRepository.DeleteReview(id);
        public Review GetReview(string id) => _reviewRepository.GetReview(id);

        public Review UpdateReview(string id, ReviewModel reviewModel) => _reviewRepository.UpdateReview(id, reviewModel);
        public async Task<List<Review>> GetReview(HelperResult.PageResult pageResult) => await _reviewRepository.GetReview(pageResult);

        public List<Review> GetReviewsByBranch(string id) => _reviewRepository.GetReviewsByBranch(id);
        public List<Review> SearchByUser(string id) => _reviewRepository.SearchByUser(id);
        public List<Review> SearchByDate(DateTime start, DateTime end) => _reviewRepository.SearchByDate(start, end);

        public List<Review> SearchByRating(int rating) => _reviewRepository.SearchByRating(rating);

        public async Task<List<Review>> SortReview(string? sortBy, bool isAsc, HelperResult.PageResult pageResult) => await _reviewRepository.SortReview(sortBy, isAsc, pageResult);
    }
}
