//using BusinessObjects;
//using DAOs;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace UnitTests.DAOsTests
//{
//    public class BookingDAOTests
//    {
//        private readonly Mock<DbSet<Booking>> mockSet;
//        private readonly Mock<CourtCallerDbContext> mockContext;
//        private readonly List<Booking> bookingList;

//        public BookingDAOTests()
//        {
//            // Initialize mock set and context
//            mockSet = new Mock<DbSet<Booking>>();
//            mockContext = new Mock<CourtCallerDbContext>();
//            bookingList = new List<Booking>
//            {
//                new Booking { BookingId = "B00001", BookingDate = DateTime.Now, Status = "Active", BookingType = "One-time", NumberOfSlot = 5 },
//                new Booking { BookingId = "B00002", BookingDate = DateTime.Now, Status = "Active", BookingType = "Fix", NumberOfSlot = 50 },
//                new Booking { BookingId = "B00003", BookingDate = DateTime.Now, Status = "Inactive", BookingType = "Flex", NumberOfSlot = 3 }
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
//        public async void GetBooking_ReturnsBooking(string bookingId)
//        {
//            var dao = new BookingDAO(mockContext.Object);

//            Booking result = await dao.GetBooking(bookingId);

//            Assert.NotNull(result);
//            Assert.Equal(bookingId, result.BookingId);
//        }
//    }
//}
