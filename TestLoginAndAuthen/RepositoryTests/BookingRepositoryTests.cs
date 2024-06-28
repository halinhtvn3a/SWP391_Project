//using BusinessObjects;
//using DAOs;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace UnitTests.RepositoryTests
//{
//    public class BookingRepositoryTests
//    {
//        private readonly Mock<DbSet<Booking>> mockSet;
//        private readonly Mock<CourtCallerDbContext> mockContext;
//        private readonly List<Booking> bookingList;

//        public BookingRepositoryTests()
//        {
//            // Initialize mock set and context
//            mockSet = new Mock<DbSet<Booking>>();
//            mockContext = new Mock<CourtCallerDbContext>();
//            bookingList = new List<Booking>
//            {
//                new Booking { BookingId = "B00001", UserId = "U00001", CourtId = "C00001", BookingDate = DateTime.Now, StartTime = DateTime.Now, EndTime = DateTime.Now, Status = "Active"  },
//                new Booking { BookingId = "B00002", UserId = "U00002", CourtId = "C00002", BookingDate = DateTime.Now, StartTime = DateTime.Now, EndTime = DateTime.Now, Status = "Active"  },
//                new Booking { BookingId = "B00003", UserId = "U00003", CourtId = "C00003", BookingDate = DateTime.Now, StartTime = DateTime.Now, EndTime = DateTime.Now, Status = "Active"  },
//                new Booking { BookingId = "B00004", UserId = "U00004", CourtId = "C00004", BookingDate = DateTime.Now, StartTime = DateTime.Now, EndTime = DateTime.Now, Status = "Active"  },
//            };

//            var data = bookingList.AsQueryable();

//            mockSet.As<IQueryable<Booking>>().Setup(m => m.Provider).Returns(data.Provider);
//            mockSet.As<IQueryable<Booking>>().Setup(m => m.Expression).Returns(data.Expression);
//            mockSet.As<IQueryable<Booking>>().Setup(m => m.ElementType).Returns(data.ElementType);
//            mockSet.As<IQueryable<Booking>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

//            mockContext.Setup(c => c.Bookings).Returns(mockSet.Object);
//        }

//        [Theory]
//        [InlineData("B00001")]
//        [InlineData("B00002")]
//        public void GetBooking_ReturnsBooking(string bookingId)
//        {
//            var repository = new BookingRepository(mockContext.Object);
//            var booking = repository.GetBooking(bookingId);
//            Assert.Equal(booking.BookingId, bookingId);
//        }

//        [Theory]
//        [InlineData("B00009")]
//        [InlineData("B00010")]
//        public void GetBooking_ReturnsNull(string booking)
//    }
//}
