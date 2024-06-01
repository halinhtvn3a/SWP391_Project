using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService reviewService;

        public ReviewsController()
        {
            reviewService = new ReviewService();
        }

        // GET: api/Reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
        {
            return reviewService.GetReviews();
        }

        // GET: api/Reviews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(string id)
        {
            var review = reviewService.GetReview(id);

            if (review == null)
            {
                return NotFound();
            }

            return review;
        }

        // PUT: api/Reviews/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(string id, Review review)
        {
            if (id != review.ReviewId)
            {
                return BadRequest();
            }

            reviewService.UpdateReview(id, review);

            return NoContent();
        }

        // POST: api/Reviews
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Review>> PostReview(Review review)
        {
            reviewService.AddReview(review);

            return CreatedAtAction("GetReview", new { id = review.ReviewId }, review);
        }

        // DELETE: api/Reviews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(string id)
        {
            var review = reviewService.GetReview(id);
            
            reviewService.DeleteReview(id);

            return NoContent();
        }

        private bool ReviewExists(string id)
        {
            return reviewService.GetReviews().Any(e => e.ReviewId == id);
        }

        [HttpGet("GetReviewsByCourt/{id}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByCourt(string id)
        {
            return reviewService.GetReviewsByCourt(id);
        }

        [HttpGet("SearchByUser/{id}")]
        public async Task<ActionResult<IEnumerable<Review>>> SearchByUser(string id)
        {
            return reviewService.SearchByUser(id);
        }

        [HttpGet("SearchByDate/{start}/{end}")]
        public async Task<ActionResult<IEnumerable<Review>>> SearchByDate(DateTime start, DateTime end)
        {
            return reviewService.SearchByDate(start, end);
        }

        [HttpGet("SearchByRating/{rating}")]
        public async Task<ActionResult<IEnumerable<Review>>> SearchByRating(int rating)
        {
            return reviewService.SearchByRating(rating);
        }


    }
}
