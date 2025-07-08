using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using DAOs.Models;
using Services;
using Microsoft.AspNetCore.Authorization;
using QRCoder;
using Page = DAOs.Helper;
using Repositories;
using Newtonsoft.Json;
using Qr = API.Helper;
using API.Helper;
using Microsoft.AspNetCore.Identity;
using Services.Interface;
using System.IdentityModel.Tokens.Jwt;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = new BookingService();
        }

        [HttpGet("current-time")]
        public IActionResult GetCurrentTime()
        {
            var currentUtcTime = DateTime.UtcNow;
            var currentLocalTime = DateTime.Now; // Thời gian cục bộ trên máy chủ

            return Ok(new
            {
                UtcTime = currentUtcTime,
                LocalTime = currentLocalTime,
                TimeZone = TimeZoneInfo.Local.StandardName
            });
        }


        [HttpGet("CheckAuthor")]
        [Authorize(Roles = "Staff")]
        public IActionResult GetProtectedData()
        {
            var claims = User.Claims;
            var expiryClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

            // Logging expiration claim
            Console.WriteLine($"Token Expiry: {expiryClaim}");

            return Ok("This is protected data.");
        }
        // GET: api/Bookings
        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<PagingResponse<Booking>>> GetBookings([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
           [FromQuery] string searchQuery = null)
        {

            var pageResult = new Page.PageResult
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
            };
            var (bookings, total) = await _bookingService.GetBookings(pageResult, searchQuery);

            var response = new PagingResponse<Booking>
            {
                Data = bookings,
                Total = total
            };
            return Ok(response);
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Booking>> GetBooking(string id)
        {
            var booking = _bookingService.GetBooking(id);

            if (booking == null)
            {
                return NotFound();
            }

            return await booking;
        }

        [HttpGet("userId/{userId}")]
        [Authorize]
        public async Task<ActionResult<Booking>> GetBookingByUserId(string userId)
        {
            var bookings = _bookingService.GetBookingsByUserId(userId);

            if (bookings == null)
            {
                return NotFound();
            }

            return Ok(bookings);
        }

        // PUT: api/Bookings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutBooking(string id, Booking booking)
        //{
        //    if (id != booking.BookingId)
        //    {
        //        return BadRequest();
        //    }

        //    bookingService.Entry(booking).State = EntityState.Modified;

        //    return NoContent();
        //}

        // POST: api/Bookings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754




        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBooking(string id)
        {
            var booking = _bookingService.GetBooking(id);
            if (booking == null)
            {
                return NotFound();
            }

            _bookingService.DeleteBooking(id);

            return NoContent();
        }

        //private bool BookingExists(string id)
        //{
        //    return bookingService.Bookings.Any(e => e.BookingId == id);
        //}

        [HttpGet("status/{status}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByStatus(string status)
        {
            return _bookingService.GetBookingsByStatus(status).ToList();
        }

        [HttpGet("search/{start}/{end}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Booking>>> SearchBookingsByTime(DateTime start, DateTime end)
        {
            return _bookingService.SearchBookingsByTime(start, end).ToList();
        }

        [HttpGet("search/{userId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByUser(string userId,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pageResult = new Page.PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            List<Booking> bookings = await _bookingService.GetBookingsByUserId(userId, pageResult);

            return Ok(bookings);
        }


        //[HttpPost("reserve")]
        //public async Task<IActionResult> ReserveSlot(string[] slotId, string userId)
        //{
        //    return await bookingService.PessimistLockAsync(slotId, userId);
        //}

        [HttpPost("reserve-slot")]
        [Authorize]
        public async Task<IActionResult> ReserveSlotV2(SlotModel[] slotModels, string userId)
        {
            var response = await _bookingService.ReserveSlotResponse(slotModels, userId);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        [HttpDelete("cancel/{bookingId}")]
        public async Task<IActionResult> CancelBooking(string bookingId)
        {
            var response = await _bookingService.CancelBookingResponse(bookingId);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        [HttpDelete("delete/{bookingId}")]
        [Authorize]
        public async Task<IActionResult> DeleteBookingAndSetTimeSlot(string bookingId)
        {
            var response = await _bookingService.DeleteBookingResponse(bookingId);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        //post booking type flex
        [HttpPost("flex")]
        [Authorize]
        public async Task<IActionResult> PostBookingTypeFlex(string userId, int numberOfSlot, string branchId)
        {
            var response = await _bookingService.AddBookingTypeFlexResponse(userId, numberOfSlot, branchId);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("fix-slot")]
        [Authorize]
        public async Task<IActionResult> PostBookingTypeFix([FromQuery] int numberOfMonths,
            [FromQuery] string[] dayOfWeek, [FromQuery] DateOnly startDate, [FromBody] TimeSlotModel[] timeSlotModel,
            [FromQuery] string userId, string branchId)
        {
            var response = await _bookingService.AddBookingTypeFixResponse(numberOfMonths, dayOfWeek, startDate, timeSlotModel, userId, branchId);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("sortBooking/{sortBy}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Booking>>> SortBookings(string sortBy, bool isAsc, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pageResult = new Page.PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            List<Booking> bookings = await _bookingService.SortBookings(sortBy, isAsc, pageResult);

            return Ok(bookings);
        }

        //[HttpPost("api/checkin/qr")]
        //public async Task<IActionResult> CheckInWithQR([FromBody] QRCheckInModel request)
        //{
        //    var qrData = DecryptQRCode(request.QRCodeData); // Implement DecryptQRCode to parse QR code data

        //    var booking = await _bookingRepository.GetBooking(qrData.BookingId);
        //    if (booking != null && booking.Status == "complete" && booking.CustomerId == qrData.CustomerId)
        //    {
        //        booking.Status = "checked-in";
        //        await _bookingService.SaveChangesAsync();

        //        return Ok("Check-in successful.");
        //    }
        //    return BadRequest("Invalid QR code or booking.");
        //}

        [HttpGet("checkbookingtypeflex")]
        [Authorize]
        public ActionResult<CheckBookingTypeFlexModel> CheckAvaiableSlotsFromBookingTypeFlex(string userId, string branchId)
        {
            var bookingFlexModel = _bookingService.NumberOfSlotsAvailable(userId, branchId);
            var result = new CheckBookingTypeFlexModel
            {
                bookingId = bookingFlexModel.Item1,
                numberOfSlot = bookingFlexModel.Item2
            };

            return Ok(result);
        }

        // public int NumberOfSlotsAvailable(string userId,string bookingId)

        [HttpGet("qrcode/{bookingId}")]
        public async Task<IActionResult> GetQRCode(string bookingId)
        {
            var response = await _bookingService.GetQRCodeResponse(bookingId);
            if (response.Status == "Success")
            {
                // Generate QR code from the JSON string returned by service
                string qrCodeBase64 = Qr.QrCode.GenerateQRCode(response.Message);
                return Ok(new ResponseModel { Status = "Success", Message = qrCodeBase64 });
            }
            return NotFound(response);
        }

        [HttpGet("daily-bookings")]
        [Authorize]
        public async Task<IActionResult> GetDailyBookings(string? branchId)
        {
            var response = await _bookingService.GetDailyBookingsResponse(branchId);
            if (response.Status == "Success")
                return Ok(response);
            return StatusCode(500, response);
        }


        [HttpGet("weekly-bookings")]
        [Authorize]
        public async Task<IActionResult> GetWeeklyBookings(string? branchId)
        {
            var response = await _bookingService.GetWeeklyBookingsResponse(branchId);
            if (response.Status == "Success")
                return Ok(response);
            return StatusCode(500, response);
        }

        [HttpGet("monthly-bookings")]
        [Authorize]
        public async Task<IActionResult> GetMonthlyBookings(string? branchId)
        {
            var response = await _bookingService.GetMonthlyBookingsResponse(branchId);
            if (response.Status == "Success")
                return Ok(response);
            return StatusCode(500, response);
        }

        [HttpGet("bookings-from-start-of-week")]
        [Authorize]
        public async Task<IActionResult> GetBookingsFromStartOfWeek(string? branchId)
        {
            try
            {
                var result = await _bookingService.GetBookingsFromStartOfWeek(branchId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("weekly-bookings-from-start-of-month")]

        public async Task<IActionResult> GetWeeklyBookingsFromStartOfMonth(string? branchId)
        {
            try
            {
                var result = await _bookingService.GetWeeklyBookingsFromStartOfMonth(branchId);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("monthly-bookings-from-start-of-year")]
        [Authorize]
        public async Task<IActionResult> GetMonthlyBookingsFromStartOfYear(string? branchId)
        {
            try
            {
                var result = await _bookingService.GetMonthlyBookingsFromStartOfYear(branchId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("daily-revenue")]
        [Authorize]
        public async Task<IActionResult> GetDailyRevenue(string? branchId)
        {
            var response = await _bookingService.GetDailyRevenueResponse(branchId);
            if (response.Status == "Success")
                return Ok(response);
            return StatusCode(500, response);
        }

        [HttpGet("weekly-revenue")]
        [Authorize]
        public async Task<IActionResult> GetWeeklyRevenue(string? branchId)
        {
            var response = await _bookingService.GetWeeklyRevenueResponse(branchId);
            if (response.Status == "Success")
                return Ok(response);
            return StatusCode(500, response);
        }

        [HttpGet("monthly-revenue")]
        [Authorize]
        public async Task<IActionResult> GetMonthlyRevenue(string? branchId)
        {
            var response = await _bookingService.GetMonthlyRevenueResponse(branchId);
            if (response.Status == "Success")
                return Ok(response);
            return StatusCode(500, response);
        }

        [HttpGet("revenue-from-start-of-week")]
        [Authorize]
        public async Task<IActionResult> GetRevenueFromStartOfWeek(string? branchId)
        {
            try
            {
                var result = await _bookingService.GetRevenueFromStartOfWeek(branchId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("weekly-revenue-from-start-of-month")]
        [Authorize]
        public async Task<IActionResult> GetWeeklyRevenueFromStartOfMonth(string? branchId)
        {
            try
            {
                var result = await _bookingService.GetWeeklyRevenueFromStartOfMonth(branchId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("monthly-revenue-from-start-of-year")]
        [Authorize]
        public async Task<IActionResult> GetMonthlyRevenueFromStartOfYear(string? branchId)
        {
            try
            {
                var result = await _bookingService.GetMonthlyRevenueFromStartOfYear(branchId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
