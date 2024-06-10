using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using BusinessObjects.Models;
using Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Page =  DAOs.Helper;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    
    public class BookingsController : ControllerBase
    {
        private readonly BookingService bookingService;

        public BookingsController()
        {
            bookingService = new BookingService();
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pageResult = new Page.PageResult
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
            };
            List<Booking> bookings = await bookingService.GetBookings(pageResult);
            return Ok(bookings);
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(string id)
        {
            var booking =  bookingService.GetBooking(id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
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
            var booking = bookingService.GetBooking(id);
            if (booking == null)
            {
                return NotFound();
            }

            bookingService.DeleteBooking(id);

            return NoContent();
        }

        //private bool BookingExists(string id)
        //{
        //    return bookingService.Bookings.Any(e => e.BookingId == id);
        //}

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByStatus(string status)
        {
            return bookingService.GetBookingsByStatus(status).ToList();
        }

        [HttpGet("search/{start}/{end}")]
        public async Task<ActionResult<IEnumerable<Booking>>> SearchBookings(DateTime start, DateTime end)
        {
            return bookingService.SearchBookings(start, end).ToList();
        }

        [HttpGet("search/{userId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> SearchBookingsByUser(string userId)
        {
            return bookingService.SearchBookingsByUser(userId).ToList();
        }

        
        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveSlot(string[] slotId, string userId)
        {
            return await bookingService.PessimistLockAsync(slotId, userId);
        }
        
        [HttpPost("reserve-slot")]
        public async Task<IActionResult> ReserveSlotV2(SlotModel[] slotModels, string userId)
        {
            return await bookingService.PessimistLockAsyncV2(slotModels, userId);
        }

        [HttpDelete("delete/{bookingId}")]
        public async Task<IActionResult> DeleteBookingAndSetTimeSlot(string bookingId)
        {
            if (string.IsNullOrEmpty(bookingId))
            {
                return BadRequest("Invalid booking id.");
            }

            await bookingService.DeleteBookingAndSetTimeSlotAsync(bookingId);

            return Ok("Booking deleted successfully.");
        }

        //post booking type flex
        [HttpPost("flex")]
        public async Task<ActionResult<Booking>> PostBookingTypeFlex(string userId, int numberOfSlot)
        {

            var booking = bookingService.AddBookingTypeFlex(userId, numberOfSlot);
            return CreatedAtAction("GetBooking", new { id = booking.BookingId }, booking);
        }
    }
}
