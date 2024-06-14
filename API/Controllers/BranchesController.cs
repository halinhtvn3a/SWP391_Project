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

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  
    public class BranchesController : ControllerBase
    {
        private readonly BranchService branchService;

        public BranchesController()
        {
            branchService = new BranchService();
        }

        // GET: api/Branches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranches([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pageResult = new PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            List<Branch> branches = await branchService.GetBranches(pageResult);

            return Ok(branches);
        }

        // GET: api/Branches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Branch>> GetBranch(string id)
        {
            var branch = branchService.GetBranch(id);

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
            var branch = branchService.GetBranch(id);
            if (id != branch.BranchId)
            {
                return BadRequest();
            }

            branchService.UpdateBranch(id, branchModel);

            return CreatedAtAction("GetBranch", new { id = branch.BranchId }, branch);
        }

        // POST: api/Branches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Branch>> PostBranch(BranchModel branchModel)
        {
            var branch = branchService.AddBranch(branchModel);

            return CreatedAtAction("GetBranch", new { id = branch.BranchId }, branch);
        }

        // DELETE: api/Branches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(string id)
        {
            var branch = branchService.GetBranch(id);
            if (branch == null)
            {
                return NotFound();
            }
            branchService.DeleteBranch(id);
            return NoContent();
        }

        //private bool BranchExists(string id)
        //{
        //    return branchService.GetBranches().Any(e => e.BranchId == id);
        //}

        [HttpGet("status")]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranchesByStatus(string status)
        {
            return branchService.GetBranchesByStatus(status).ToList();
        }

    }
}
