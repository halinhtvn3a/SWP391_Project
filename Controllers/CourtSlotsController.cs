using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourtManagement.Models;

namespace CourtManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtSlotsController : ControllerBase
    {
        private readonly CourtBookingContext _context;

        public CourtSlotsController(CourtBookingContext context)
        {
            _context = context;
        }

        // GET: api/CourtSlots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourtSlot>>> GetCourtSlots()
        {
            return await _context.CourtSlots.ToListAsync();
        }

        // GET: api/CourtSlots/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourtSlot>> GetCourtSlot(string id)
        {
            var courtSlot = await _context.CourtSlots.FindAsync(id);

            if (courtSlot == null)
            {
                return NotFound();
            }

            return courtSlot;
        }

        // PUT: api/CourtSlots/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourtSlot(string id, CourtSlot courtSlot)
        {
            if (id != courtSlot.CourtslotId)
            {
                return BadRequest();
            }

            _context.Entry(courtSlot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourtSlotExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CourtSlots
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CourtSlot>> PostCourtSlot(CourtSlot courtSlot)
        {
            _context.CourtSlots.Add(courtSlot);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CourtSlotExists(courtSlot.CourtslotId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCourtSlot", new { id = courtSlot.CourtslotId }, courtSlot);
        }

        // DELETE: api/CourtSlots/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourtSlot(string id)
        {
            var courtSlot = await _context.CourtSlots.FindAsync(id);
            if (courtSlot == null)
            {
                return NotFound();
            }

            _context.CourtSlots.Remove(courtSlot);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourtSlotExists(string id)
        {
            return _context.CourtSlots.Any(e => e.CourtslotId == id);
        }
    }
}
