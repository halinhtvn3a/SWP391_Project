using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using Services;
using Microsoft.AspNetCore.Authorization;
using DAOs.Helper;




namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtsController : ControllerBase
    {
        private readonly CourtService courtService;

        public CourtsController()
        {
            courtService = new CourtService();
        }

        // GET: api/Courts
        [HttpGet]
        
        public async Task<ActionResult<IEnumerable<Court>>> GetCourts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) 
        {
            var pageResult = new PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var court = await courtService.GetCourts(pageResult);
            return Ok(court);
        }

        // GET: api/Courts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Court>> GetCourt(string id)
        {
            var court = courtService.GetCourt(id);

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

            courtService.UpdateCourt(id, court);

            return NoContent();
        }

        // POST: api/Courts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Court>> PostCourt(Court court)
        {

            courtService.AddCourt(court);

            return CreatedAtAction("GetCourt", new { id = court.CourtId }, court);
        }

        // DELETE: api/Courts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourt(string id)
        {
            var court = courtService.GetCourt(id);
            if (court == null)
            {
                return NotFound();
            }

            courtService.DeleteCourt(id);

            return NoContent();
        }

        //private bool CourtExists(string id)
        //{
        //    return courtService.GetCourts().Any(e => e.CourtId == id);
        //}

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Court>>> GetActiveCourts()
        {
            return courtService.GetActiveCourts();
        }

    }
}
