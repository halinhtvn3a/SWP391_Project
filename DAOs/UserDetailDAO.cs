using DAOs;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using Microsoft.AspNetCore.Identity;

namespace DAOs
{
    public class UserDetailDAO
	{
        private readonly CourtCallerDbContext dbContext = null;

        public UserDetailDAO()
        {
            if (dbContext == null)
            {
                dbContext = new CourtCallerDbContext();
            }
        }

        public List<UserDetail> GetUserDetails()
        {
            //    return dbContext.UserDetails.ToList();
            //}
            //public List<UserDetail> GetUserDetails()
            //{
            var UserDetails = dbContext.UserDetails
          .Include(u => u.User)
          .ToList();

            return UserDetails.Select(u => new UserDetail
            {
                UserDetailId = u.UserDetailId,
                Balance = u.Balance,
                FullName = u.FullName,
                Status = u.Status,
                User = dbContext.Users.FirstOrDefault(m => m.Id.Equals(u.User.Id))
            }).ToList();
        }


        public UserDetail GetUserDetail(string id)
        {
            return dbContext.UserDetails.FirstOrDefault(m => m.UserDetailId.Equals(id));
        }

        public UserDetail AddUserDetail(UserDetail UserDetail)
        {
            dbContext.UserDetails.Add(UserDetail);
            dbContext.SaveChanges();
            return UserDetail;
        }

        public UserDetail UpdateUserDetail(string id, UserDetail UserDetail)
        {
            UserDetail oUserDetail = GetUserDetail(id);
            if (oUserDetail != null)
            {
                oUserDetail.Balance = UserDetail.Balance;
                oUserDetail.FullName = UserDetail.FullName;
                oUserDetail.Status = UserDetail.Status;
                dbContext.Update(oUserDetail);
                dbContext.SaveChanges();
            }
            return oUserDetail;
        }

        public void DeleteUserDetail(string id)
        {
            UserDetail oUserDetail = GetUserDetail(id);
            if (oUserDetail != null)
            {
                oUserDetail.Status = false;
                dbContext.Update(oUserDetail);
                dbContext.SaveChanges();
            }
        }

        //public List<IdentityUser> GetSortList(string field) => GetUsers().OrderBy(u => GetPropertyValue(u, field)).ToList();

        //private object GetPropertyValue(object obj, string propertyName) => obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);

        //public List<IdentityUser> SearchUser(string search) => GetUsers().Where(u => u.Email.Contains(search) || u.Id.Contains(search)).ToList();
        public UserDetail GetUserDetailByUserId(string userId)
        {
            return dbContext.UserDetails.FirstOrDefault(m => m.User.Id.Equals(userId));
        }
    }
}
