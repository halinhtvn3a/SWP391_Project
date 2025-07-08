using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using Services;
using Microsoft.AspNetCore.Identity;
using DAOs.Helper;
using DAOs.Models;
using Microsoft.AspNetCore.Authorization;
using StackExchange.Redis;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;


        public UsersController(UserService userService, IConnectionMultiplexer redis)
        {
            _userService = new UserService();
            _userService.InitializeRedis(redis);
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchQuery = null)
        {
            var pageResult = new PageResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var response = await _userService.GetUsersResponse(pageResult, searchQuery);
            if (response.Status == "Success")
                return Ok(response);
            return StatusCode(500, response);
        }







        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<IdentityUser>>> GetUsers()
        //{
        //    return _userService.GetUsers().ToList();
        //}

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetUser(string id)
        {
            var response = _userService.GetUserResponse(id);
            if (response.Status == "Success")
                return Ok(response);
            return NotFound(response);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutUser(string id, IdentityUser user)
        //{
        //    if (id != user.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _userService.UpdateUser(id, user);

        //    return NoContent();
        //}

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<IdentityUser>> PostUser(IdentityUser user)
        //{
        //    _userService.AddUser(user);

        //    return CreatedAtAction("GetUser", new { id = user.Id }, user);
        //}

        // DELETE: api/Users/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteUser(string id)
        //{
        //    var user = _userService.GetUser(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    _userService.DeleteUser(id);

        //    return NoContent();
        //}

        //private bool UserExists(string id)
        //{
        //    return _userService.Users.Any(e => e.UserId == id);
        //}

        [HttpPut("{id}/ban")]
        [Authorize(Roles = "Admin")]
        public IActionResult BanUser(string id)
        {
            var response = _userService.BanUserResponse(id);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }


        [HttpPut("{id}/unban")]
        [Authorize(Roles = "Admin")]
        public IActionResult UnbanUser(string id)
        {
            var response = _userService.UnBanUserResponse(id);
            if (response.Status == "Success")
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("GetUserDetailByUserEmail/{userEmail}")]
        [Authorize]
        public IActionResult GetUserByEmail(string userEmail)
        {
            var response = _userService.SearchUserByEmailResponse(userEmail);
            if (response.Status == "Success")
                return Ok(response);
            if (response.Message.Contains("not found"))
                return NotFound(response);
            return BadRequest(response);
        }


    }
}
