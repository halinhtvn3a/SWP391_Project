using BusinessObjects;
using DAOs.Models;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserDetailService
    {
        private readonly UserDetailRepository UserDetailRepository = null;
        public UserDetailService()
        {
            if (UserDetailRepository == null)
            {
                UserDetailRepository = new UserDetailRepository();
            }
        }
        public UserDetail AddUserDetail(UserDetail UserDetail) => UserDetailRepository.AddUserDetail(UserDetail);
        //public void DeleteUserDetail(string id) => UserDetailRepository.DeleteUserDetail(id);
        public UserDetail GetUserDetail(string id) => UserDetailRepository.GetUserDetail(id);
        public List<UserDetail> GetUserDetails() => UserDetailRepository.GetUserDetails();
        public UserDetail UpdateUserDetail(string id, UserDetailsModel userDetailsModel) => UserDetailRepository.UpdateUserDetail(id, userDetailsModel);
        public List<UserDetail> SearchUserByEmail(string searchValue) => UserDetailRepository.SearchUserByEmail(searchValue);

    }
}
