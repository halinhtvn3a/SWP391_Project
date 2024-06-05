using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DAOs;

namespace Repositories
{
    public class UserDetailRepository
    {
        private readonly UserDetailDAO _userDetailDao = null;
        public UserDetailRepository()
        {
            if (_userDetailDao == null)
            {
                _userDetailDao = new UserDetailDAO();
            }
        }
        public UserDetail AddUserDetail(UserDetail UserDetail) => _userDetailDao.AddUserDetail(UserDetail);

        //public void DeleteUserDetail(string id) => _userDetailDao.DeleteUserDetail(id);

        public UserDetail GetUserDetail(string id) => _userDetailDao.GetUserDetail(id);

        public List<UserDetail> GetUserDetails() => _userDetailDao.GetUserDetails();

        public UserDetail UpdateUserDetail(string id, UserDetail UserDetail) => _userDetailDao.UpdateUserDetail(id, UserDetail);
    }
}
