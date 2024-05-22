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
    public class CourtsController : ControllerBase
    {
        private readonly CourtBookingContext _context;

        public CourtsController(CourtBookingContext context)
        {
            _context = context;
        }

        // GET: api/Courts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Court>>> GetCourts()
        {
            return await _context.Courts.ToListAsync();
        }

        // GET: api/Courts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Court>> GetCourt(string id)
        {
            var court = await _context.Courts.FindAsync(id);

            if (court == null)
            {
                return NotFound();
            }

            return court;
        }

        // PUT: api/Courts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourt(string id, Court court)
        {
            if (id != court.CourtId)
            {
                return BadRequest();
            }

            _context.Entry(court).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourtExists(id))
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

        // POST: api/Courts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Court>> PostCourt(Court court)
        {
            _context.Courts.Add(court);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CourtExists(court.CourtId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCourt", new { id = court.CourtId }, court);
        }

        // DELETE: api/Courts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourt(string id)
        {
            var court = await _context.Courts.FindAsync(id);
            if (court == null)
            {
                return NotFound();
            }

            _context.Courts.Remove(court);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourtExists(string id)
        {
            return _context.Courts.Any(e => e.CourtId == id);
        }
    }
}
