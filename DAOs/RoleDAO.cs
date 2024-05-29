using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApplication2.Data;

namespace DAOs
{
	public class RoleDAO
	{
		private readonly CourtCallerDbContext dbContext = null;

		public RoleDAO()
		{
			if (dbContext == null)
			{
				dbContext = new CourtCallerDbContext();
			}
		}

		public List<IdentityRole> GetRoles()
		{
			return dbContext.Roles.ToList();
		}

		public IdentityRole GetRole(string id)
		{
			return dbContext.Roles.FirstOrDefault(m => m.Id.Equals(id));
		}

		public IdentityRole AddRole(IdentityRole IdentityRole)
		{
			dbContext.Roles.Add(IdentityRole);
			dbContext.SaveChanges();
			return IdentityRole;
		}

		public IdentityRole UpdateRole(string id, IdentityRole IdentityRole)
		{
			IdentityRole oIdentityRole = GetRole(id);
			if (oIdentityRole != null)
			{
				oIdentityRole.Name = IdentityRole.Name;
				oIdentityRole.ConcurrencyStamp = IdentityRole.ConcurrencyStamp;
				oIdentityRole.NormalizedName = IdentityRole.NormalizedName;
				dbContext.Update(oIdentityRole);
				dbContext.SaveChanges();
			}
			return oIdentityRole;
		}

		public void DeleteRole(string id)
		{
			IdentityRole oIdentityRole = GetRole(id);
			if (oIdentityRole != null)
			{
				dbContext.Remove(oIdentityRole);
				dbContext.SaveChanges();
			}
		}
	}
}
