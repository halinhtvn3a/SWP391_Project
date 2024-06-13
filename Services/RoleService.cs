using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
	public class RoleService
	{
		private readonly RoleRepository RoleRepository = null;
		public RoleService()
		{
			if (RoleRepository == null)
			{
				RoleRepository = new RoleRepository();
			}
		}
		public IdentityRole AddRole(IdentityRole Role) => RoleRepository.AddRole(Role);
		public void DeleteRole(string id) => RoleRepository.DeleteRole(id);
		public IdentityRole GetRole(string id) => RoleRepository.GetRole(id);
		public List<IdentityRole> GetRoles() => RoleRepository.GetRoles();
		public IdentityRole UpdateRole(string id, IdentityRole Role) => RoleRepository.UpdateRole(id, Role);

		public string[] GetRoleNameByUserId(string userId) => RoleRepository.GetRoleNameByUserId(userId);
	}
}
