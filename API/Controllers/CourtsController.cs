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
using DAOs.Models;




namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtsController : ControllerBase
    {
        private readonly CourtService _courtService;

        public CourtsController()
        {
            _courtService = new CourtService();
        }

        // GET: api/Courts
        [HttpGet]
        public async Task<IActionResult> GetCourts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchQuery = null)
        {
            var pageResult = new PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var response = await _courtService.GetCourtsResponse(pageResult, searchQuery);
            if (response.Status == "Success")
                return Ok(response);
            return StatusCode(500, response);
        }

        // GET: api/Courts/5
        [HttpGet("{id}")]
        public IActionResult GetCourt(string id)
        {
            var response = _courtService.GetCourtResponse(id);
            if (response.Status == "Success")
                return Ok(response);
            return NotFound(response);
        }

        // PUT: api/Courts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult PutCourt(string id, CourtModel courtModel)
        {
            var response = _courtService.UpdateCourtResponse(id, courtModel);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        // POST: api/Courts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult PostCourt(CourtModel courtModel)
        {
            var response = _courtService.AddCourtResponse(courtModel);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        // DELETE: api/Courts/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteCourt(string id)
        {
            var response = _courtService.DeleteCourtResponse(id);
            if (response.Status == "Success")
                return Ok(response);
            return NotFound(response);
        }

        //private bool CourtExists(string id)
        //{
        //    return courtService.GetCourts().Any(e => e.CourtId == id);
        //}

        [HttpGet("status/{status}")]
        public IActionResult GetCourtsByStatus(string status)
        {
            var response = _courtService.GetCourtsByStatusResponse(status);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("sort/{sortBy}")]
        public async Task<IActionResult> SortCourt(string sortBy, bool isAsc, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pageResult = new PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var response = await _courtService.SortCourtResponse(sortBy, isAsc, pageResult);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("/numberOfCourt/{branchId}")]
        public IActionResult GetNumberOfCourtsByBranchId(string branchId)
        {
            var response = _courtService.GetNumberOfCourtsByBranchIdResponse(branchId);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("AvailableCourts")]
        public IActionResult AvailableCourts([FromBody] SlotModel slotModel)
        {
            var response = _courtService.AvailableCourtsResponse(slotModel);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("GetCourtsByBranchId")]
        public async Task<IActionResult> GetCourtsByBranchId([FromQuery] string branchId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchQuery = null)
        {
            var pageResult = new PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var response = await _courtService.GetCourtsByBranchIdResponse(branchId, pageResult, searchQuery);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

    }
}
