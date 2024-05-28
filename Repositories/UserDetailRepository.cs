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
        private readonly UserDetailDAO UserDetailDAO = null;
        public UserDetailRepository()
        {
            if (UserDetailDAO == null)
            {
                UserDetailDAO = new UserDetailDAO();
            }
        }
        public UserDetail AddUserDetail(UserDetail UserDetail) => UserDetailDAO.AddUserDetail(UserDetail);

        public void DeleteUserDetail(string id) => UserDetailDAO.DeleteUserDetail(id);

        public UserDetail GetUserDetail(string id) => UserDetailDAO.GetUserDetail(id);

        public List<UserDetail> GetUserDetails() => UserDetailDAO.GetUserDetails();

        public UserDetail UpdateUserDetail(string id, UserDetail UserDetail) => UserDetailDAO.UpdateUserDetail(id, UserDetail);
    }
}
