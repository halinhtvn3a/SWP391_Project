using DAOs;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;

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
            return dbContext.UserDetails.ToList();
        }
        //public List<UserDetail> GetUserDetails()
        //{
        //    var UserDetails = dbContext.UserDetails
        //  .Include(u => u.User)
        //  .ToList();

        //    return UserDetails.Select(u => new UserDetail
        //    {
        //        UserDetailId = u.UserDetailId,
        //        Balance = u.Balance,
        //        FullName = u.FullName,
        //        Status = u.Status,
        //        //only BookingId
        //        Users = u.Users.Select(b => new Booking
        //        {
        //            BookingId = b.BookingId
        //        }).ToList(),
        //        //only ReviewId
        //        Reviews = u.Reviews.Select(r => new Review
        //        {
        //            ReviewId = r.ReviewId
        //        }).ToList()
        //    }).ToList();
        //}

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
    }
}
