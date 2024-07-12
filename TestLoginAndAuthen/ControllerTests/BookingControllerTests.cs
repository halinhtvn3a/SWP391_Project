using API.Controllers;
using BusinessObjects;
using DAOs.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.Interface;

namespace UnitTests.ControllerTests {
    public class BookingsControllerTests
    {
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly BookingsController _controller;

        public BookingsControllerTests()
        {
            _mockBookingService = new Mock<IBookingService>();
            _controller = new BookingsController(_mockBookingService.Object);
        }
        [Fact]
        public async Task ReserveSlotV2_ReturnsOk_WhenBookingIsSuccessful()
        {
            // Arrange
            var slotModels = new SlotModel[]
            {
            new SlotModel
            {
                BranchId = "B00002",
                SlotDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                CourtId = "C00002",
                TimeSlot = new TimeSlotModel
                {
                    SlotStartTime = new TimeOnly(10, 0),
                    SlotEndTime = new TimeOnly(11, 0)
                }
            }
            };
            string userId = "U00002";
            var booking = new Booking
            {
                BookingId = "B12345",
                Id = userId,
                BranchId = "B00002",
                BookingDate = DateTime.Now,
                Status = "Pending",
                TotalPrice = 100,
                BookingType = "By Day",
                NumberOfSlot = slotModels.Length
            };

            _mockBookingService.Setup(service => service.ReserveSlotAsyncV2(It.IsAny<SlotModel[]>(), It.IsAny<string>()))
                .ReturnsAsync(booking);

            // Act
            var result = await _controller.ReserveSlotV2(slotModels, userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBooking = Assert.IsType<Booking>(okResult.Value);
            Assert.Equal(booking.BookingId, returnedBooking.BookingId);
        }

        [Fact]
        public async Task ReserveSlotV2_ReturnsBadRequest_WhenBookingFails()
        {
            // Arrange
            var slotModels = new SlotModel[]
            {
            new SlotModel
            {
                BranchId = "B00001",
                SlotDate = DateOnly.FromDateTime(DateTime.Now),
                CourtId = "C00001",
                TimeSlot = new TimeSlotModel
                {
                    SlotStartTime = new TimeOnly(9, 0),
                    SlotEndTime = new TimeOnly(10, 0)
                }
            }
            };
            string userId = "U00001";

            _mockBookingService.Setup(service => service.ReserveSlotAsyncV2(It.IsAny<SlotModel[]>(), It.IsAny<string>()))
                .ReturnsAsync((Booking)null);

            // Act
            var result = await _controller.ReserveSlotV2(slotModels, userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to reserve slot.", badRequestResult.Value);
        }
    }
}
