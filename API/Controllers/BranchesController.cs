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
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranches([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchQuery = null)
        {
            var pageResult = new PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            List<Branch> branches = await _branchService.GetBranches(pageResult,searchQuery);

            return Ok(branches);
        }

        // GET: api/Branches/5
       
        [HttpGet("{id}")]
        public async Task<ActionResult<Branch>> GetBranch(string id)
        {
            var branch = _branchService.GetBranch(id);

            if (branch == null)
            {
                return NotFound();
            }

            return branch;
        }

        // PUT: api/Branches/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBranch(string id, BranchModel branchModel)
        {
            var branch = _branchService.GetBranch(id);
            if (id != branch.BranchId)
            {
                return BadRequest();
            }

            _branchService.UpdateBranch(id, branchModel);

            return CreatedAtAction("GetBranch", new { id = branch.BranchId }, branch);
        }

        // POST: api/Branches

        //[HttpPost]
        //public async Task<ActionResult<Branch>> PostBranch(BranchModel branchModel)
        //{
        //    var branch = _branchService.AddBranch(branchModel);

        //    return CreatedAtAction("GetBranch", new { id = branch.BranchId }, branch);
        //}


        [HttpPost]
        public async Task<ActionResult<Branch>> PostBranch([FromForm] BranchModel branchModel)
        {
            var imageUrls = new List<string>();

            foreach (var file in branchModel.BranchPictures)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                using (var stream = file.OpenReadStream())
                {
                    var task = new FirebaseStorage("court-callers.appspot.com")
                        .Child("BranchImage")
                        .Child(fileName)
                        .PutAsync(stream);

                    var downloadUrl = await task;
                    imageUrls.Add(downloadUrl);
                }
            }

            branchModel.BranchPicture = JsonSerializer.Serialize(imageUrls);
            var branch = _branchService.AddBranch(branchModel);

            return CreatedAtAction("GetBranch", new { id = branch.BranchId }, branch);
        }

        // DELETE: api/Branches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(string id)
        {
            var branch = _branchService.GetBranch(id);
            if (branch == null)
            {
                return NotFound();
            }
            _branchService.DeleteBranch(id);
            return NoContent();
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

        [HttpGet("GetBranchByPrice/{minPrice}&&{maxPrice}")]
        public async Task<ActionResult<IEnumerable<Branch>>> SortBranchByPrice(decimal minPrice, decimal maxPrice)
        {
            return _branchService.GetBranchByPrice(minPrice, maxPrice).ToList();
        }

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
    }
}
