using BusinessObjects;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Repositories
{
	public class RoleRepository
	{
			private readonly RoleDAO RoleDAO = null;
			public RoleRepository()
			{
				if (RoleDAO == null)
				{
					RoleDAO = new RoleDAO();
				}
			}
			public IdentityRole AddRole(IdentityRole Role) => RoleDAO.AddRole(Role);

			public void DeleteRole(string id) => RoleDAO.DeleteRole(id);

			public IdentityRole GetRole(string id) => RoleDAO.GetRole(id);

			public List<IdentityRole> GetRoles() => RoleDAO.GetRoles();

			public IdentityRole UpdateRole(string id, IdentityRole Role) => RoleDAO.UpdateRole(id, Role);
	}
	
}
