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

using Page = DAOs.Helper;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class BookingsController : ControllerBase
    {
        private readonly BookingService _bookingService;

        public BookingsController()
        {
            _bookingService = new BookingService();
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
           [FromQuery] string searchQuery = null)
        {
            var pageResult = new Page.PageResult
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
            };
            List<Booking> bookings = await _bookingService.GetBookings(pageResult,searchQuery);
            return Ok(bookings);
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
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


        //[HttpPost]
        //public async Task<ActionResult<Booking>> PostBooking(Booking booking)
        //{
        //    bookingService.AddBooking(booking);

        //    return CreatedAtAction("GetBooking", new { id = booking.BookingId }, booking);
        //}

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
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
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByStatus(string status)
        {
            return _bookingService.GetBookingsByStatus(status).ToList();
        }

        [HttpGet("search/{start}/{end}")]
        public async Task<ActionResult<IEnumerable<Booking>>> SearchBookings(DateTime start, DateTime end)
        {
            return _bookingService.SearchBookings(start, end).ToList();
        }

        [HttpGet("search/{userId}")]
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
        public async Task<IActionResult> ReserveSlotV2(SlotModel[] slotModels, string userId)
        {
            var booking = _bookingService.PessimistLockAsyncV2(slotModels, userId);

            return booking != null ? Ok(booking) : BadRequest("Failed to reserve slot.");
        }

        [HttpDelete("cancelBooking/{bookingId}")]
        public async Task<IActionResult> CancelBooking(string bookingId)
        {
            if (string.IsNullOrEmpty(bookingId))
            {
                return BadRequest("Invalid booking id.");
            }

            _bookingService.CancelBooking(bookingId);

            return Ok("Booking cancelled successfully.");
        }

        [HttpDelete("delete/{bookingId}")]
        public async Task<IActionResult> DeleteBookingAndSetTimeSlot(string bookingId)
        {
            if (string.IsNullOrEmpty(bookingId))
            {
                return BadRequest("Invalid booking id.");
            }

            await _bookingService.DeleteBookingAndSetTimeSlotAsync(bookingId);

            return Ok("Booking deleted successfully.");
        }

        //post booking type flex
        [HttpPost("flex")]
        public async Task<ActionResult<Booking>> PostBookingTypeFlex(string userId, int numberOfSlot, string branchId)
        {

            var booking = _bookingService.AddBookingTypeFlex(userId, numberOfSlot, branchId);
            return CreatedAtAction("GetBooking", new { id = booking.BookingId }, booking);
        }

        [HttpPost("fix-slot")]
        public async Task<IActionResult> PostBookingTypeFix([FromQuery] int numberOfMonths,
            [FromQuery] string[] dayOfWeek, [FromQuery] DateOnly startDate, [FromBody] TimeSlotModel timeSlotModel,
            [FromQuery] string userId, string branchId)
        {
            return await _bookingService.AddBookingTypeFix(numberOfMonths, dayOfWeek, startDate, timeSlotModel, userId,
                branchId);
        }

        [HttpGet("sortBooking/{sortBy}")]
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
    }
}
