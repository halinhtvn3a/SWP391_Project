using DAOs;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication2.Data;
using DAOs.Helper;
using Microsoft.AspNetCore.Identity;

namespace DAOs
{
    public class ReviewDAO
    {
        private readonly CourtCallerDbContext dbContext = null;

        public ReviewDAO()
        {
            if (dbContext == null)
            {
                dbContext = new CourtCallerDbContext();
            }
        }

        public async Task<List<Review>> GetReview(PageResult pageResult)
        {
            var query = dbContext.Reviews.AsQueryable();
            Pagination pagination = new Pagination(dbContext);
            List<Review> reviews = await pagination.GetListAsync<Review>(query, pageResult);
            return reviews;
        }

        public Review GetReview(string id)
        {
            return dbContext.Reviews.FirstOrDefault(m => m.ReviewId.Equals(id));
        }

        public Review AddReview(Review Review)
        {
            dbContext.Reviews.Add(Review);
            dbContext.SaveChanges();
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
                dbContext.Update(oReview);
                dbContext.SaveChanges();
            }
            return oReview;
        }

        public void DeleteReview(string id)
        {
            Review oReview = GetReview(id);
            if (oReview != null)
            {
                dbContext.Remove(oReview);
                dbContext.SaveChanges();
            }
        }

        public List<Review> SearchByDate(DateTime start, DateTime end) => dbContext.Reviews.Where(m => m.ReviewDate >= start && m.ReviewDate <= end).ToList();


        public List<Review> SearchByRating(int rating) => dbContext.Reviews.Where(m => m.Rating == rating).ToList();

        public List<Review> SearchByUser(string id) => dbContext.Reviews.Where(m => m.Id == id).ToList();


        public List<Review> GetReviewsByCourt(string id) => dbContext.Reviews.Where(m => m.CourtId == id).ToList();


    }
}
