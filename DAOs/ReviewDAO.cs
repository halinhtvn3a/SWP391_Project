using DAOs;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<Review> GetReviews()
        {
            return dbContext.Reviews.ToList();
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
    }
}
