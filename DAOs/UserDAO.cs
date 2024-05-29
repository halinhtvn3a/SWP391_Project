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

		//public IdentityUser UpdateIdentityUser(string id, IdentityUser IdentityUser)
		//{
		//	IdentityUser oIdentityUser = GetIdentityUser(id);
		//	if (oIdentityUser != null)
		//	{
		//		oIdentityUser.Balance = IdentityUser.Balance;
		//		oIdentityUser.FullName = IdentityUser.FullName;
		//		oIdentityUser.Status = IdentityUser.Status;
		//		dbContext.Update(oIdentityUser);
		//		dbContext.SaveChanges();
		//	}
		//	return oIdentityUser;
		//}

		//public void DeleteIdentityUser(string id)
		//{
		//	IdentityUser oIdentityUser = GetIdentityUser(id);
		//	if (oIdentityUser != null)
		//	{
		//		oIdentityUser.Status = false;
		//		dbContext.Update(oIdentityUser);
		//		dbContext.SaveChanges();
		//	}
		//}

		public IdentityUser BanUser(string id, IdentityUser user)
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


public IdentityUser UnbanUser(string id, IdentityUser user)
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

	}
}
