using BusinessObjects;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using DAOs.Helper;

namespace Repositories
{
	public class UserRepository
	{
		private readonly UserDAO _userDao = null;
		public UserRepository()
		{
			if (_userDao == null)
			{
                _userDao = new UserDAO();
			}
		}

        public string GenerateUserId() => _userDao.GenerateUserId();

        public IdentityUser AddUser(IdentityUser IdentityUser) => _userDao.AddUser(IdentityUser);

        //public void DeleteIdentityUser(string id) => UserDAO.DeleteIdentityUser(id);

        public async Task<List<IdentityUser>> GetUsers(PageResult page) => await _userDao.GetUsers(page);



        public IdentityUser GetUser(string id) => _userDao.GetUser(id);

		//public List<IdentityUser> GetUsers() => UserDAO.GetUsers();

		//public IdentityUser UpdateIdentityUser(string id, IdentityUser IdentityUser) => UserDAO.UpdateIdentityUser(id, IdentityUser);

		public void BanUser(string id) => _userDao.BanUser(id);

		public void UnBanUser(string id) => _userDao.UnBanUser(id);
	}
}
