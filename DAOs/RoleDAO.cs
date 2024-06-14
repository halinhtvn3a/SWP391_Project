using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DAOs
{
    public class RoleDAO
    {
        private readonly CourtCallerDbContext DbContext = null;

        public RoleDAO()
        {
            if (DbContext == null)
            {
                DbContext = new CourtCallerDbContext();
            }
        }

        public List<IdentityRole> GetRoles()
        {
            return DbContext.Roles.ToList();
        }

        public IdentityRole GetRole(string id)
        {
            return DbContext.Roles.FirstOrDefault(m => m.Id.Equals(id));
        }

        public IdentityRole AddRole(IdentityRole IdentityRole)
        {
            DbContext.Roles.Add(IdentityRole);
            DbContext.SaveChanges();
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
                DbContext.Update(oIdentityRole);
                DbContext.SaveChanges();
            }

            return oIdentityRole;
        }

        public void DeleteRole(string id)
        {
            IdentityRole oIdentityRole = GetRole(id);
            if (oIdentityRole != null)
            {
                DbContext.Remove(oIdentityRole);
                DbContext.SaveChanges();
            }
        }

        public string[] GetRoleNameByUserId(string userId)
        { 
            var roles = DbContext.UserRoles.Where(m => m.UserId.Equals(userId)).ToList();
            string[] roleNames = new string[roles.Count];
            for (int i = 0; i < roles.Count; i++)
            {
                roleNames[i] = DbContext.Roles.FirstOrDefault(m => m.Id.Equals(roles[i].RoleId)).Name;
            }

            return roleNames;
        }
    }
}
