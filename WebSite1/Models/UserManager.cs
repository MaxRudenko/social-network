using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebSite.Models
{
    static public class UserManager
    {
        static public void InitialazingAdmin()
        {
            /*using (DatabaseContext db = new DatabaseContext())
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                string pass = "qwe123";
                byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(pass));
                string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);

                if (db.Users.Count() == 0)
                {
                    var user = new User() { FullName = "Руденко Максим Алексеевич", HashPassword = result, Login = "admin123", Role = "admin", Email = "max.rudenko.2014@mail.ru" };
                    db.Users.Add(user);
                    db.SaveChanges();
                    db.Profiles.Add(new Profile(){Id=user.Id,FirstName="Максим",SecondName="Руденко",ThirdName="Алексеевич"});
                    db.SaveChanges();
                }
            }*/
        }
        static public bool UserCreate(RegisterViewModel model,ref string role)
        {
            if (model.Login == "admin123")
                role = "admin";
            else role = "user";
            if (MailMessanger.SendMessage("brut_75@mail.ru", model.Email, "brut_75@mail.ru","V80984342375"))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                string pass = model.Password;
                byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(pass));
                string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);

                var user = new User() { FullName = model.SecondName+" "+model.FirstName+" "+model.ThirdName
                    , HashPassword = result
                    , Login = model.Login
                    , Role = role
                    , Email = model.Email ,
                    LastDate=DateTime.Now
                };
                using (DatabaseContext db = new DatabaseContext())
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    db.Profiles.Add(new Profile() { 
                        Id = user.Id, FirstName = model.FirstName, 
                        SecondName = model.SecondName, 
                        ThirdName = model.ThirdName,
                        Date=model.Date,
                        Gender=model.Gender,
                        AvatarPath = "/Images/noavatar.png"
                        
                    });
                    db.SaveChanges();
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        static public User UserLogin(string login, string password, string ip)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            string pass = password;
            byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(pass));
            string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
            using(DatabaseContext db = new DatabaseContext())
            {
                var user = db.Users.Where(u => u.Login == login).FirstOrDefault();
                if(user!=null && user.HashPassword==result)
                {
                    return user;
                }
            }
            return null;
        }
        static public void EditProfile(EditProfileViewModel model,string userLogin)
        {
            using(DatabaseContext db = new DatabaseContext())
            {
                var user=db.Users.Where(u => u.Login == userLogin).FirstOrDefault();
                var profile=db.Profiles.Where(p=>p.Id==user.Id).FirstOrDefault();
                if(user!=null)
                {
                    if (model.AvatarPath != null)
                    { profile.AvatarPath = model.AvatarPath; }
                    profile.Chair = model.Chair;
                    profile.Country = model.Country;
                    profile.Skype = model.Skype;
                    profile.Phone = model.Phone;
                    profile.Group = model.Group;
                    db.SaveChanges();
                }
            }
        }
        static public void AddSession(string name)
        {
            using(DatabaseContext db =new DatabaseContext())
            {
                var user = db.Users.Where(u => u.Login == name).FirstOrDefault();
                if (user != null)
                {
                    user.LastDate = DateTime.Now;
                    db.SaveChanges();
                }
            }
        }
        static public string isOnline(int id)
        {
            using(DatabaseContext db = new DatabaseContext())
            {
                var user = db.Users.Where(u => u.Id == id).FirstOrDefault();
                if(user.LastDate.Year==DateTime.Now.Year&& user.LastDate.Month==DateTime.Now.Month&&user.LastDate.Day==DateTime.Now.Day)
                {
                    TimeSpan time = DateTime.Now - user.LastDate;
                    if(time.Hours==0 && time.Minutes<10)
                    {
                        return "Online";
                    }
                }
                
                return "";
            }
        }
        static public List<User> SearchUsers(string searchString)
        {
            List<User> result = new List<User>();
            string[] massInStrings = searchString.Split(' ');

            using (DatabaseContext db = new DatabaseContext())
            {
                foreach (var str in massInStrings)
                {
                    var users = db.Users.Include("Profile").Where(u => u.Profile.FirstName==str||u.Profile.SecondName==str||u.Profile.ThirdName==str).ToList();
                    if (users != null)
                    {
                       result.AddRange(users);
                    }
                }
            }
            result.Distinct();

            return result;
        }
    }
}