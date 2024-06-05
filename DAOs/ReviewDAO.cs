using DAOs;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOs.Helper;
using Microsoft.AspNetCore.Identity;

namespace DAOs
{
    public class ReviewDAO
    {
        private readonly CourtCallerDbContext DbContext = null;

        public ReviewDAO()
        {
            if (DbContext == null)
            {
                DbContext = new CourtCallerDbContext();
            }
        }

        public async Task<List<Review>> GetReview(PageResult pageResult)
        {
            var query = DbContext.Reviews.AsQueryable();
            Pagination pagination = new Pagination(DbContext);
            List<Review> reviews = await pagination.GetListAsync<Review>(query, pageResult);
            return reviews;
        }

        public Review GetReview(string id)
        {
            return DbContext.Reviews.FirstOrDefault(m => m.ReviewId.Equals(id));
        }

        public Review AddReview(Review Review)
        {
            DbContext.Reviews.Add(Review);
            DbContext.SaveChanges();
            return Review;
        }

        public Review UpdateReview(string id, Review Review)
        {
            Review oReview = GetReview(id);
            if (oReview != null)
            {
                oReview.ReviewText = Review.ReviewText;
                oReview.Rating = Review.Rating;
                oReview.ReviewDate = DateTime.Now;
                DbContext.Update(oReview);
                DbContext.SaveChanges();
            }
            return oReview;
        }

        public void DeleteReview(string id)
        {
            Review oReview = GetReview(id);
            if (oReview != null)
            {
                DbContext.Remove(oReview);
                DbContext.SaveChanges();
            }
        }

        public List<Review> SearchByDate(DateTime start, DateTime end) => DbContext.Reviews.Where(m => m.ReviewDate >= start && m.ReviewDate <= end).ToList();


        public List<Review> SearchByRating(int rating) => DbContext.Reviews.Where(m => m.Rating == rating).ToList();

        public List<Review> SearchByUser(string id) => DbContext.Reviews.Where(m => m.Id == id).ToList();


        public List<Review> GetReviewsByBranch(string id) => DbContext.Reviews.Where(m => m.BranchId == id).ToList();


    }
}
