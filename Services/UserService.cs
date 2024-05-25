using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserService
    {
        private readonly UserRepository UserRepository = null;
        public UserService()
        {
            if (UserRepository == null)
            {
                UserRepository = new UserRepository();
            }
        }
        public User AddUser(User User) => UserRepository.AddUser(User);
        public void DeleteUser(string id) => UserRepository.DeleteUser(id);
        public User GetUser(string id) => UserRepository.GetUser(id);
        public List<User> GetUsers() => UserRepository.GetUsers();
        public User UpdateUser(string id, User User) => UserRepository.UpdateUser(id, User);
    }
}
