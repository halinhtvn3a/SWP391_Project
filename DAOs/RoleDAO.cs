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
        private readonly CourtCallerDbContext _courtCallerDbContext = null;

        public RoleDAO()
        {
            if (_courtCallerDbContext == null)
            {
                _courtCallerDbContext = new CourtCallerDbContext();
            }
        }

        public List<IdentityRole> GetRoles()
        {
            return _courtCallerDbContext.Roles.ToList();
        }

        public IdentityRole GetRole(string id)
        {
            return _courtCallerDbContext.Roles.FirstOrDefault(m => m.Id.Equals(id));
        }

        public IdentityRole AddRole(IdentityRole IdentityRole)
        {
            _courtCallerDbContext.Roles.Add(IdentityRole);
            _courtCallerDbContext.SaveChanges();
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
                _courtCallerDbContext.Update(oIdentityRole);
                _courtCallerDbContext.SaveChanges();
            }

            return oIdentityRole;
        }

        public void DeleteRole(string id)
        {
            IdentityRole oIdentityRole = GetRole(id);
            if (oIdentityRole != null)
            {
                _courtCallerDbContext.Remove(oIdentityRole);
                _courtCallerDbContext.SaveChanges();
            }
        }

        public string[] GetRoleNameByUserId(string userId)
        { 
            var roles = _courtCallerDbContext.UserRoles.Where(m => m.UserId.Equals(userId)).ToList();
            string[] roleNames = new string[roles.Count];
            for (int i = 0; i < roles.Count; i++)
            {
                roleNames[i] = _courtCallerDbContext.Roles.FirstOrDefault(m => m.Id.Equals(roles[i].RoleId)).Name;
            }

            return roleNames;
        }
    }
}
