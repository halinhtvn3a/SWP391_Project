using DAOs;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAOs
{
    public class UserDetailDAO
	{
        private readonly CourtCallerDbContext _dbContext = null;

        public UserDetailDAO()
        {
            if (_dbContext == null)
            {
                _dbContext = new CourtCallerDbContext();
            }
        }

        public List<UserDetail> GetUserDetails()
        {
            return _dbContext.UserDetails.ToList();
        }
        //public List<UserDetail> GetUserDetails()
        //{
        //    var UserDetails = _dbContext.UserDetails
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
            return _dbContext.UserDetails.FirstOrDefault(m => m.UserId.Equals(id));
        }

        public UserDetail AddUserDetail(UserDetail UserDetail)
        {
            _dbContext.UserDetails.Add(UserDetail);
            _dbContext.SaveChanges();
            return UserDetail;
        }

        public UserDetail UpdateUserDetail(string id, UserDetail UserDetail)
        {
            UserDetail oUserDetail = GetUserDetail(id);
            if (oUserDetail != null)
            {
                oUserDetail.Balance = UserDetail.Balance;
                oUserDetail.FullName = UserDetail.FullName;
                oUserDetail.Address = UserDetail.Address;
                oUserDetail.ProfilePicture = UserDetail.ProfilePicture;
                oUserDetail.YearOfBirth = UserDetail.YearOfBirth;
                
                _dbContext.Update(oUserDetail);
                _dbContext.SaveChanges();
            }
            return oUserDetail;
        }

        //public void DeleteUserDetail(string id)
        //{
        //    UserDetail oUserDetail = GetUserDetail(id);
        //    if (oUserDetail != null)
        //    {
        //        oUserDetail.Status = false;
        //        _dbContext.Update(oUserDetail);
        //        _dbContext.SaveChanges();
        //    }
        //}
    }
}
