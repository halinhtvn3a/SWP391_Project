using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using Repositories;
using Services;
using Microsoft.AspNetCore.Authorization;
using DAOs.Helper;
using DAOs.Models;
using Firebase.Storage;
using Firebase.Auth;
using System.Text.Json;
using MailKit.Search;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class BranchesController : ControllerBase
    {
        private readonly BranchService _branchService;

        public BranchesController()
        {
            _branchService = new BranchService();
        }

        // GET: api/Branches

        [HttpGet]
        public async Task<IActionResult> GetBranches([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchQuery = null)
        {
            var pageResult = new PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var response = await _branchService.GetBranchesResponse(pageResult, searchQuery);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("HomePage")]
        public async Task<IActionResult> GetBranchesHomePage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string status = "Active", [FromQuery] string searchQuery = null)
        {
            var pageResult = new PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var response = await _branchService.GetBranchesResponse(pageResult, status, searchQuery);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        // GET: api/Branches/5

        [HttpGet("{id}")]
        public IActionResult GetBranch(string id)
        {
            var response = _branchService.GetBranchResponse(id);
            if (response.Status == "Success")
                return Ok(response);
            return NotFound(response);
        }

        // PUT: api/Branches/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult PutBranch(string id, [FromForm] PutBranch branchModel)
        {
            var response = _branchService.UpdateBranchResponse(id, branchModel);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }



        // POST: api/Branches

        //[HttpPost]
        //public async Task<ActionResult<Branch>> PostBranch(BranchModel branchModel)
        //{
        //    var branch = _branchService.AddBranch(branchModel);

        //    return CreatedAtAction("GetBranch", new { id = branch.BranchId }, branch);
        //}


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostBranch([FromForm] BranchModel branchModel)
        {
            var response = await _branchService.AddBranchResponseAsync(branchModel);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        // DELETE: api/Branches/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteBranch(string id)
        {
            var response = _branchService.DeleteBranchResponse(id);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        //private bool BranchExists(string id)
        //{
        //    return _branchService.GetBranches().Any(e => e.BranchId == id);
        //}

        [HttpGet("status")]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranchesByStatus(string status)
        {
            return _branchService.GetBranchesByStatus(status).ToList();
        }

        //[HttpGet("GetBranchByPrice/{minPrice}&&{maxPrice}")]
        //public async Task<ActionResult<IEnumerable<Branch>>> SortBranchByPrice(decimal minPrice, decimal maxPrice)
        //{
        //    return _branchService.GetBranchByPrice(minPrice, maxPrice).ToList();
        //}

        [HttpGet("courtId/{courtId}")]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranchesByCourtId(string courtId)
        {
            return _branchService.GetBranchesByCourtId(courtId).ToList();
        }

        [HttpGet("lastBranch/{userId}")]
        public async Task<ActionResult<Branch>> GetLastBranch(string userId)
        {
            return _branchService.GetLastBranch(userId);
        }

        [HttpGet("sortBranch/{sortBy}")]
        public async Task<ActionResult<IEnumerable<Branch>>> SortBranch(string sortBy, bool isAsc, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pageResult = new PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return await _branchService.SortBranch(sortBy, isAsc, pageResult);
        }

        [HttpGet("sortBranchByDistance")]
        public async Task<ActionResult<PagingResponse<BranchDistance>>> SortBranchByDistance([FromQuery] LocationModel user, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pageResult = new PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var (branches, total) = await _branchService.SortBranchByDistance(user, pageResult);
            if (total == 0)
            {
                return NotFound("No branches found or unable to sort branches by distance.");
            }

            var response = new PagingResponse<BranchDistance>
            {
                Data = branches,
                Total = total
            };
            return Ok(response);
        }

        [HttpGet("GetBranchByPrice/{minPrice}&&{maxPrice}")]
        public async Task<ActionResult<PagingResponse<Branch>>> GetBranchByPrice([FromQuery] decimal minPrice = 0, [FromQuery] decimal maxPrice = 200, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {

            var pageResult = new PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var (branches, total) = await _branchService.GetBranchByPrice(minPrice, maxPrice, pageResult);
            if (total == 0)
            {
                return NotFound("No branches found.");
            }
            var response = new PagingResponse<Branch>
            {
                Data = branches,
                Total = total
            };
            return Ok(response);
        }

        [HttpGet("GetBranchByRating/{rating}")]
        public async Task<ActionResult<PagingResponse<Branch>>> GetBranchByRating([FromQuery] int rating = 5, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {

            var pageResult = new PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var (branches, total) = await _branchService.GetBranchByRating(rating, pageResult);
            if (total == 0)
            {
                return NotFound("No branches found.");
            }
            var response = new PagingResponse<Branch>
            {
                Data = branches,
                Total = total
            };
            return Ok(response);
        }

    }
}
