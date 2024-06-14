using DAOs;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Helper;
using Microsoft.AspNetCore.Identity;
using DAOs.Models;

namespace DAOs
{
    public class ReviewDAO
    {
        private readonly CourtCallerDbContext _courtCallerDbContext = null;

        public ReviewDAO()
        {
            if (_courtCallerDbContext == null)
            {
                _courtCallerDbContext = new CourtCallerDbContext();
            }
        }

        public async Task<List<Review>> GetReview(PageResult pageResult)
        {
            var query = _courtCallerDbContext.Reviews.AsQueryable();
            Pagination pagination = new Pagination(_courtCallerDbContext);
            List<Review> reviews = await pagination.GetListAsync<Review>(query, pageResult);
            return reviews;
        }

        public Review GetReview(string id)
        {
            return _courtCallerDbContext.Reviews.FirstOrDefault(m => m.ReviewId.Equals(id));
        }

        public Review AddReview(ReviewModel reviewModel)
        {
            Review review = new Review()
            {
                ReviewId = "R" + (_courtCallerDbContext.Reviews.Count() + 1).ToString("D5"),
                ReviewText = reviewModel.ReviewText,
                Rating = reviewModel.Rating,
                ReviewDate = DateTime.Now,
                BranchId = reviewModel.BranchId,
                Id = reviewModel.UserId
            };
            _courtCallerDbContext.Reviews.Add(review);
            _courtCallerDbContext.SaveChanges();
            return review;
        }

        public Review UpdateReview(string id, ReviewModel reviewModel)
        {
            Review oReview = GetReview(id);
            if (oReview != null)
            {
                oReview.ReviewText = reviewModel.ReviewText;
                oReview.Rating = reviewModel.Rating;
                oReview.ReviewDate = DateTime.Now;
                _courtCallerDbContext.Update(oReview);
                _courtCallerDbContext.SaveChanges();
            }
            return oReview;
        }

        public void DeleteReview(string id)
        {
            Review oReview = GetReview(id);
            if (oReview != null)
            {
                _courtCallerDbContext.Remove(oReview);
                _courtCallerDbContext.SaveChanges();
            }
        }

        public List<Review> SearchByDate(DateTime start, DateTime end) => _courtCallerDbContext.Reviews.Where(m => m.ReviewDate >= start && m.ReviewDate <= end).ToList();


        public List<Review> SearchByRating(int rating) => _courtCallerDbContext.Reviews.Where(m => m.Rating == rating).ToList();

        public List<Review> SearchByUser(string id) => _courtCallerDbContext.Reviews.Where(m => m.Id == id).ToList();


        public List<Review> GetReviewsByBranch(string id) => _courtCallerDbContext.Reviews.Where(m => m.BranchId == id).ToList();


    }
}
