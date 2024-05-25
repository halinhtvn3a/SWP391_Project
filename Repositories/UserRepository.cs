using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DAOs;

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
        public User AddUser(User User) => UserDAO.AddUser(User);

        public void DeleteUser(string id) => UserDAO.DeleteUser(id);

        public User GetUser(string id) => UserDAO.GetUser(id);

        public List<User> GetUsers() => UserDAO.GetUsers();

        public User UpdateUser(string id, User User) => UserDAO.UpdateUser(id, User);
    }
}
