using BusinessObjects;
using Repositories;

using HelperResult = DAOs.Helper;

namespace Services
{
    public class ReviewService
    {
        private readonly ReviewRepository ReviewRepository = null;
        public ReviewService()
        {
            if (ReviewRepository == null)
            {
                ReviewRepository = new ReviewRepository();
            }
        }
        public Review AddReview(Review Review) => ReviewRepository.AddReview(Review);
        public void DeleteReview(string id) => ReviewRepository.DeleteReview(id);
        public Review GetReview(string id) => ReviewRepository.GetReview(id);
       
        public Review UpdateReview(string id, Review Review) => ReviewRepository.UpdateReview(id, Review);
        public async Task<List<Review>> GetReview(HelperResult.PageResult pageResult) => await ReviewRepository.GetReview(pageResult);

        public List<Review> GetReviewsByCourt(string id) => ReviewRepository.GetReviewsByCourt(id);
        public List<Review> SearchByUser(string id) => ReviewRepository.SearchByUser(id);
        public List<Review> SearchByDate(DateTime start, DateTime end) => ReviewRepository.SearchByDate(start, end);

        public List<Review> SearchByRating(int rating) => ReviewRepository.SearchByRating(rating);


    }
}
