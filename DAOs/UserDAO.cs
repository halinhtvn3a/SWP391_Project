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
	public class UserDAO
	{
		private readonly CourtCallerDbContext dbContext = null;

		public UserDAO()
		{
			if (dbContext == null)
			{
				dbContext = new CourtCallerDbContext();
			}
		}

		public List<IdentityUser> GetUsers()
		{

			return dbContext.Users.ToList();
		}
		//public List<IdentityUser> GetIdentityUsers()
		//{
		//	var IdentityUsers = dbContext.Users
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
			return dbContext.Users.FirstOrDefault(m => m.Id.Equals(id));
		}

		public IdentityUser AddUser(IdentityUser IdentityUser)
		{
			dbContext.Users.Add(IdentityUser);
			dbContext.SaveChanges();
			return IdentityUser;
		}

        public IdentityUser UpdateIdentityUser(string id, string email)
        {
            IdentityUser oIdentityUser = GetUser(id);
            if (oIdentityUser != null)
            {
                oIdentityUser.Email = email;
                dbContext.Update(oIdentityUser);
                dbContext.SaveChanges();
            }
            return oIdentityUser;
        }

        //public void DeleteIdentityUser(string id)
        //{
        //    IdentityUser oIdentityUser = GetIdentityUser(id);
        //    if (oIdentityUser != null)
        //    {
        //        oIdentityUser.Status = false;
        //        dbContext.Update(oIdentityUser);
        //        dbContext.SaveChanges();
        //    }
        //}

        public IdentityUser BanUser(string id)
        {
            IdentityUser oUser = GetUser(id);
            if (oUser != null)
            {
                oUser.LockoutEnabled = false;
                dbContext.Update(oUser);
                dbContext.SaveChanges();
            }
            return oUser;
        }


        public IdentityUser UnBanUser(string id)
        {
            IdentityUser oUser = GetUser(id);
            if (oUser != null)
            {
                oUser.LockoutEnabled = true;
                dbContext.Update(oUser);
                dbContext.SaveChanges();
            }
            return oUser;
        }

        public List<IdentityUser> SortByEmail() =>  GetUsers().OrderBy(u => u.Email).ToList();

        public List<IdentityUser> SearchUser(string search) => GetUsers().Where(u => u.Email.Contains(search) || u.Id.Contains(search)).ToList();

    }
}
