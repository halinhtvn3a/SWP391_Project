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
    public class BookingRepositoryTests
    {
        private readonly Mock<DbSet<Booking>> mockSet;
        private readonly Mock<CourtCallerDbContext> mockContext;
        private readonly List<Booking> bookingList;
        private readonly BranchDAO branchDAO;
        private readonly BookingDAO bookingDAO;
        private readonly TimeSlotDAO timeSlotDAO;
        private readonly CourtDAO courtDAO;
        private readonly UserDAO userDAO;
        private readonly UserDetailDAO userDetailDAO;
        private readonly BookingRepository bookingRepository;
        private readonly TimeSlotRepository timeSlotRepository;

        public BookingRepositoryTests()
        {
            // Initialize mock set and context
            mockSet = new Mock<DbSet<Booking>>();
            mockContext = new Mock<CourtCallerDbContext>();
            bookingList = new List<Booking>
            {
                new Booking {
                    BookingId = "B00001",
                    Id = "U00001",
                    BranchId = "B00001",
                    BookingDate = DateTime.Now,
                    BookingType = "By day",
                    NumberOfSlot = 5,
                    Status = "Complete",
                    TotalPrice = 100,
                    TimeSlots = new List<TimeSlot>{
                        new TimeSlot()
                        {
                            SlotId = "S00001",
                            SlotDate = DateOnly.FromDateTime(DateTime.Now),
                            Status = "Reserved",
                            CourtId = "C00001",
                            BookingId = "B00001",
                            SlotEndTime = new TimeOnly(8, 0),
                            SlotStartTime = new TimeOnly(7, 0),
                            Court = new Court
                            {
                                CourtId = "C00001",
                                CourtName = "Court 1",
                                BranchId = "B00001",
                                Branch = new Branch
                                {
                                    BranchId = "B00001",
                                }
                            }
                        },
                        new TimeSlot
                        {
                            SlotId = "S00002",
                            SlotDate = DateOnly.FromDateTime(DateTime.Now),
                            Status = "Reserved",
                            CourtId = "C00001",
                            BookingId = "B00001",
                            SlotEndTime = new TimeOnly(9, 0),
                            SlotStartTime = new TimeOnly(8, 0),
                            Court = new Court
                            {
                                CourtId = "C00001",
                                CourtName = "Court 1",
                                BranchId = "B00001",
                                Branch = new Branch
                                {
                                    BranchId = "B00001",
                                }
                            }
                        },
                        new TimeSlot
                        {
                            SlotId = "S00003",
                            SlotDate = DateOnly.FromDateTime(DateTime.Now),
                            Status = "Reserved",
                            CourtId = "C00001",
                            BookingId = "B00001",
                            SlotEndTime = new TimeOnly(10, 0),
                            SlotStartTime = new TimeOnly(9, 0),
                            Court = new Court
                            {
                                CourtId = "C00001",
                                CourtName = "Court 1",
                                BranchId = "B00001",
                                Branch = new Branch
                                {
                                    BranchId = "B00001",
                                }
                            }
                        },
                    }
                }
            };

            var data = bookingList.AsQueryable();

            mockSet.As<IQueryable<Booking>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Booking>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Booking>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Booking>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            mockContext.Setup(c => c.Bookings).Returns(mockSet.Object);

        }

       
    }
}
