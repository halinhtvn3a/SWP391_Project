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

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class BranchesController : ControllerBase
    {
        private readonly BranchService branchService;

        public BranchesController()
        {
            branchService = new BranchService();
        }

        // GET: api/Branches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranches()
        {
            return branchService.GetBranches().ToList();
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
        public async Task<IActionResult> PutBranch(string id, Branch branch)
        {
            if (id != branch.BranchId)
            {
                return BadRequest();
            }

            branchService.UpdateBranch(id, branch);

            return NoContent();
        }

        // POST: api/Branches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Branch>> PostBranch(Branch branch)
        {
            branchService.AddBranch(branch);

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

        private bool BranchExists(string id)
        {
            return branchService.GetBranches().Any(e => e.BranchId == id);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Branch>>> SearchBranches(string search)
        {
            return branchService.SearchBranches(search).ToList();
        }

        [HttpGet("status")]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranchesByStatus(bool status)
        {
            return branchService.GetBranchesByStatus(status).ToList();
        }
    }
}
