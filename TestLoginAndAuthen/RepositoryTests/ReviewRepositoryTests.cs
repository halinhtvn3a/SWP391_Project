using BusinessObjects;
using DAOs;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.RepositoryTests
{
    public class ReviewRepositoryTests
    {
        private readonly Mock<DbSet<Review>> mockSet;
        private readonly Mock<CourtCallerDbContext> mockContext;
        private readonly List<Review> reviewList;
        private readonly ReviewDAO reviewDAO;
        private readonly ReviewRepository reviewRepository;

        public ReviewRepositoryTests()
        {
            // Initialize mock set and context
            mockSet = new Mock<DbSet<Review>>();
            mockContext = new Mock<CourtCallerDbContext>();
            reviewList = new List<Review>
            {
                new Review { ReviewId = "R00001", ReviewText = "Test Review 1", Rating = 5, ReviewDate = DateTime.Now, BranchId = "B00001" },
                new Review { ReviewId = "R00002", ReviewText = "Test Review 2", Rating = 1, ReviewDate = DateTime.Now, BranchId = "B00001" },
                new Review { ReviewId = "R00003", ReviewText = "Test Review 3", Rating = 2, ReviewDate = DateTime.Now, BranchId = "B00001" },
                new Review { ReviewId = "R00004", ReviewText = "Test Review 4", Rating = 3, ReviewDate = DateTime.Now, BranchId = "B00001" },
                new Review { ReviewId = "R00005", ReviewText = "Test Review 5", Rating = 4, ReviewDate = DateTime.Now, BranchId = "B00001" },
                new Review { ReviewId = "R00006", ReviewText = "Test Review 6", Rating = 5, ReviewDate = DateTime.Now, BranchId = "B00001" },
            };

            var data = reviewList.AsQueryable();

            mockSet.As<IQueryable<Review>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Review>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Review>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Review>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            mockContext.Setup(c => c.Reviews).Returns(mockSet.Object);

            reviewDAO = new ReviewDAO(mockContext.Object);
            reviewRepository = new ReviewRepository(reviewDAO);
        }

        [Theory]
        [InlineData(5, 33, "B00001")]
        [InlineData(1, 17, "B00001")]
        public void GetRatingPercentageOfABranch_ReturnsWell(int rating, decimal expected, string branchId)
        {
            decimal actual = Math.Round(reviewRepository.GetRatingPercentageOfABranch(rating, branchId));

            Assert.Equal(expected, actual);
        }
    }
}
