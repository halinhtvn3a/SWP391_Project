using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using DAOs.Helper;

namespace DAOs
{
	public class UserDAO
	{
		private readonly CourtCallerDbContext _dbContext = null;
        Guid guid =Guid.NewGuid();

        public UserDAO()
		{
			if (_dbContext == null)
			{
				_dbContext = new CourtCallerDbContext();
			}
		}


        public string GenerateUserId ()
        {
            return "U" + guid.ToString(); 
        }

        public async Task<List<IdentityUser>> GetUsers(PageResult pageResult)
        {
            var query = _dbContext.Users.AsQueryable();
            Pagination pagination = new Pagination(_dbContext);
            List<IdentityUser> identityUsers = await pagination.GetListAsync<IdentityUser>(query, pageResult);
            return identityUsers;
        }


        //public List<IdentityUser> GetUsers()
        //{

        //	return _dbContext.Users.ToList();
        //}
        //public List<IdentityUser> GetIdentityUsers()
        //{
        //	var IdentityUsers = _dbContext.Users
        //  .Include(u => u.IdentityUser)
        //  .ToList();

        //	return IdentityUsers.Select(u => new IdentityUser
        //	{
        //		IdentityUserId = u.IdentityUserId,
        //		Balance = u.Balance,
        //		FullName = u.FullName,
        //		Status = u.Status,
        //		//only BookingId
        //		IdentityUsers = u.IdentityUsers.Select(b => new Booking
        //		{
        //			BookingId = b.BookingId
        //		}).ToList(),
        //		//only ReviewId
        //		Reviews = u.Reviews.Select(r => new Review
        //		{
        //			ReviewId = r.ReviewId
        //		}).ToList()
        //	}).ToList();
        //}

        public IdentityUser GetUser(string id)
		{
			return _dbContext.Users.FirstOrDefault(m => m.Id.Equals(id));
		}

		public IdentityUser AddUser(IdentityUser IdentityUser)
		{
			_dbContext.Users.Add(IdentityUser);
			_dbContext.SaveChanges();
			return IdentityUser;
		}

        //public IdentityUser UpdateIdentityUser(string id, IdentityUser IdentityUser)
        //{
        //	IdentityUser oIdentityUser = GetIdentityUser(id);
        //	if (oIdentityUser != null)
        //	{
        //		oIdentityUser.Balance = IdentityUser.Balance;
        //		oIdentityUser.FullName = IdentityUser.FullName;
        //		oIdentityUser.Status = IdentityUser.Status;
        //		_dbContext.Update(oIdentityUser);
        //		_dbContext.SaveChanges();
        //	}
        //	return oIdentityUser;
        //}

        public void DeleteUser(string id)
        {
            IdentityUser oidentityuser = GetUser(id);
            if (oidentityuser != null)
            {
                oidentityuser.LockoutEnabled = false;
                _dbContext.Update(oidentityuser);
                _dbContext.SaveChanges();
            }
        }

        public IdentityUser BanUser(string id)
        {
            IdentityUser oUser = GetUser(id);
            if (oUser != null)
            {
                oUser.LockoutEnabled = false;
                _dbContext.Update(oUser);
                _dbContext.SaveChanges();
            }
            return oUser;
        }


        public IdentityUser UnBanUser(string id)
        {
            IdentityUser oUser = GetUser(id);
            if (oUser != null)
            {
                oUser.LockoutEnabled = true;
                _dbContext.Update(oUser);
                _dbContext.SaveChanges();
            }
            return oUser;
        }


    }
}
