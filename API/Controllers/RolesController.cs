using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
	public class RolesController : Controller
	{
		private readonly RoleService RoleService;

		public RolesController()
		{
			RoleService = new RoleService();
		}

		// GET: api/Roles
		[HttpGet]
		public async Task<ActionResult<IEnumerable<IdentityRole>>> GetRoles()
		{
			return RoleService.GetRoles();
		}

		// GET: api/Roles/5
		[HttpGet("{id}")]
		public async Task<ActionResult<IdentityRole>> GetRole(string id)
		{
			var Role = RoleService.GetRole(id);

			if (Role == null)
			{
				return NotFound();
			}

			return Role;
		}

		[HttpGet("{userId}")]
        
		public async Task<ActionResult<string[]>> GetRoleNameByUserId(string userId)
        {
            var Role = RoleService.GetRoleNameByUserId(userId);

            if (Role == null)
            {
                return NotFound();
            }

            return Role;
        }
		// PUT: api/Roles/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		//[HttpPut("{id}")]
		//public async Task<IActionResult> PutRole(string id, IdentityRole Role)
		//{
		//	if (id != Role.Id)
		//	{
		//		return BadRequest();
		//	}

		//	RoleService.UpdateRole(id, Role);

		//	return NoContent();
		//}

		// POST: api/Roles
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		//[HttpPost]
		//public async Task<ActionResult<IdentityRole>> PostRole(IdentityRole Role)
		//{
		//	RoleService.AddRole(Role);

		//	return CreatedAtAction("GetRole", new { id = Role.Id }, Role);
		//}

		// DELETE: api/Roles/5
		//[HttpDelete("{id}")]
		//public async Task<IActionResult> DeleteRole(string id)
		//{
		//	var Role = RoleService.GetRole(id);

		//	RoleService.DeleteRole(id);

		//	return NoContent();
		//}

		//private bool RoleExists(string id)
		//{
		//	return RoleService.GetRoles().Any(e => e.Id == id);
		//}
	}
}
