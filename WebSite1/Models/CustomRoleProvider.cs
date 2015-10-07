using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace WebSite.Models
{
    public class CustomRoleProvider : RoleProvider
    {
        public override string[] GetRolesForUser(string login)
        {
            string[] role = new string[] { };
            using (DatabaseContext db = new DatabaseContext())
            {
                try
                {
                    // Получаем пользователя
                    User user = db.Users.Where(u => u.Login == login).FirstOrDefault();
                    if (user != null)
                    {
                      role = new string[] { user.Role };
                    }
                }
                catch
                {
                    role = new string[] { };
                }
            }
            return role;
        }
        public override void CreateRole(string roleName)
        {
            /*Role newRole = new Role() { Name = roleName };
            UserContext db = new UserContext();
            db.Roles.Add(newRole);
            db.SaveChanges();*/
            throw new NotImplementedException();
        }
        public override bool IsUserInRole(string username, string roleName)
        {
            bool outputResult = false;
            // Находим пользователя
            using (DatabaseContext db = new DatabaseContext())
            {
                try
                {
                    // Получаем пользователя
                    User user = db.Users.Where(u => u.Login == username).FirstOrDefault();
                    if (user != null)
                    {
                        //сравниваем
                        if (user.Role == roleName)
                        {
                            outputResult = true;
                        }
                    }
                }
                catch
                {
                    outputResult = false;
                }
            }
            return outputResult;
        }
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}