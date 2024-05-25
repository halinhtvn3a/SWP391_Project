using DAOs;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<User> GetUsers()
        {
            return dbContext.Users.ToList();
        }

        public User GetUser(string id)
        {
            return dbContext.Users.FirstOrDefault(m => m.UserId.Equals(id));
        }

        public User AddUser(User User)
        {
            dbContext.Users.Add(User);
            dbContext.SaveChanges();
            return User;
        }

        public User UpdateUser(string id, User User)
        {
            User oUser = GetUser(id);
            if (oUser != null)
            {
                oUser.Username = User.Username;
                oUser.Password = User.Password;
                oUser.Balance = User.Balance;
                oUser.Email = User.Email;
                oUser.PhoneNumber = User.PhoneNumber;
                oUser.FullName = User.FullName;
                oUser.Status = User.Status;
                dbContext.Update(oUser);
                dbContext.SaveChanges();
            }
            return oUser;
        }

        public void DeleteUser(string id)
        {
            User oUser = GetUser(id);
            if (oUser != null)
            {
                oUser.Status = false;
                dbContext.Update(oUser);
                dbContext.SaveChanges();
            }
        }
    }
}
