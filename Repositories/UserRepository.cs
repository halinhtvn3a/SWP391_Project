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
	public class UserRepository
	{
		private readonly UserDAO UserDAO = null;
		public UserRepository()
		{
			if (UserDAO == null)
			{
				UserDAO = new UserDAO();
			}
		}
		public IdentityUser AddUser(IdentityUser IdentityUser) => UserDAO.AddUser(IdentityUser);

		//public void DeleteIdentityUser(string id) => UserDAO.DeleteIdentityUser(id);

		public IdentityUser GetUser(string id) => UserDAO.GetUser(id);

		public List<IdentityUser> GetUsers() => UserDAO.GetUsers();

		//public IdentityUser UpdateIdentityUser(string id, IdentityUser IdentityUser) => UserDAO.UpdateIdentityUser(id, IdentityUser);
	}
}
