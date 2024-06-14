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
        public async Task<IActionResult> PutTimeSlot(string id, TimeSlot timeSlot)
        {
            if (id != timeSlot.SlotId)
            {
                return BadRequest();
            }

            _timeSlotService.UpdateTimeSlot(id, timeSlot);

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

        private bool TimeSlotExists(string id)
        {
            return _timeSlotService.GetTimeSlots().Any(e => e.SlotId == id);
        }
    }
}
