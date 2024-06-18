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
using Page = DAOs.Helper;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotsController : ControllerBase
    {
        private readonly TimeSlotService _timeSlotService;

        public TimeSlotsController()
        {
            _timeSlotService = new TimeSlotService();
        }

        // GET: api/TimeSlots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> GetTimeSlots()
        {
            return _timeSlotService.GetTimeSlots();
        }
        
        [HttpGet("page/")]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> GetTimeSlotsPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchQuery = null)
        {
            var pageResult = new Page.PageResult
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
            };
            List<TimeSlot> timeSlots = await _timeSlotService.GetTimeSlots(pageResult,searchQuery);
            return Ok(timeSlots);
        }

        // GET: api/TimeSlots/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TimeSlot>> GetTimeSlot(string id)
        {
            var timeSlot = _timeSlotService.GetTimeSlot(id);

            if (timeSlot == null)
            {
                return NotFound();
            }

            return timeSlot;
        }
        
        [HttpGet("bookingId/{bookingId}")]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> GetTimeSlotByBookingId(string bookingId)
        {
            var timeSlot = _timeSlotService.GetTimeSlotsByBookingId(bookingId);

            if (timeSlot == null)
            {
                return NotFound();
            }

            return timeSlot;
        }

        // PUT: api/TimeSlots/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTimeSlot(string id, SlotModel slotModel)
        {
            var timeSlot = _timeSlotService.GetTimeSlot(id);
            if (id != timeSlot.SlotId)
            {
                return BadRequest();
            }

            _timeSlotService.UpdateTimeSlot(id, slotModel);

            return NoContent();
        }

        // POST: api/TimeSlots
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TimeSlot>> PostTimeSlot(TimeSlot timeSlot)
        {
            _timeSlotService.AddTimeSlot(timeSlot);

            return CreatedAtAction("GetTimeSlot", new { id = timeSlot.SlotId }, timeSlot);
        }

        // DELETE: api/TimeSlots/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteTimeSlot(string id)
        //{
        //    var timeSlot = timeSlotService.GetTimeSlot(id);
        //    if (timeSlot == null)
        //    {
        //        return NotFound();
        //    }

        //    timeSlotService.TimeSlots.Remove(timeSlot);

        //    return NoContent();
        //}

        [HttpGet("isBooked")]
        public async Task<ActionResult<bool>> IsSlotBookedInBranch(SlotModel slotModel)
        {
            return _timeSlotService.IsSlotBookedInBranch(slotModel);
        }

        [HttpPut("changeSlot/{slotId}")]
        public async Task<ActionResult<TimeSlot>> ChangeSlot(SlotModel slotModel, string slotId)
        {
            TimeSlot timeSlot = _timeSlotService.ChangeSlot(slotModel, slotId);

            return Ok(new TimeSlot()
            {
                SlotId = timeSlot.SlotId,
                CourtId = timeSlot.CourtId,
                BookingId = timeSlot.BookingId,
                SlotDate = timeSlot.SlotDate,
                SlotStartTime = timeSlot.SlotStartTime,
                SlotEndTime = timeSlot.SlotEndTime,
                Price = timeSlot.Price,
                Status = timeSlot.Status
            });
        }

        [HttpGet("userId/{userId}")]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> GetTimeSlotsByUserId(string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pageResult = new Page.PageResult
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
            };
            List<TimeSlot> timeSlots = await _timeSlotService.GetTimeSlotsByUserId(userId, pageResult);
            return Ok(timeSlots);
        }

        [HttpGet("sortSlot/{sortBy}")]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> SortTimeSlot(string sortBy, bool isAsc, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pageResult = new Page.PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return await _timeSlotService.SortTimeSlot(sortBy, isAsc, pageResult);
        }
        private bool TimeSlotExists(string id)
        {
            return _timeSlotService.GetTimeSlots().Any(e => e.SlotId == id);
        }
    }
}
