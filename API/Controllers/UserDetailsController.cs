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

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserDetailsController : ControllerBase
	{
		private readonly UserDetailService UserDetailservice;

		public UserDetailsController()
		{
			UserDetailservice = new UserDetailService();
		}

		// GET: api/UserDetails
		[HttpGet]
		public async Task<ActionResult<IEnumerable<UserDetail>>> GetUserDetails()
		{
			return UserDetailservice.GetUserDetails().ToList();
		}

		// GET: api/UserDetails/5
		[HttpGet("{id}")]
		public async Task<ActionResult<UserDetail>> GetUser(string id)
		{
			var user = UserDetailservice.GetUserDetail(id);

			if (user == null)
			{
				return NotFound();
			}

			return user;
		}

		//PUT: api/UserDetails/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutUser(string id, UserDetail user)
		{
			if (id != user.UserDetailId)
			{
				return BadRequest();
			}

			UserDetailservice.UpdateUserDetail(id, user);

			return NoContent();
		}

		// POST: api/UserDetails
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<UserDetail>> PostUser(UserDetail user)
		{
			UserDetailservice.AddUserDetail(user);

			return CreatedAtAction("GetUser", new { id = user.UserDetailId }, user);
		}

		//DELETE: api/UserDetails/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(string id)
		{
			var user = UserDetailservice.GetUserDetail(id);
			if (user == null)
			{
				return NotFound();
			}

			UserDetailservice.DeleteUserDetail(id);

			return NoContent();
		}

		private bool UserExists(string id)
		{
			return UserDetailservice.GetUserDetails().Any(e => e.UserDetailId == id);
		}
	}
}
