using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class ProfileController : Controller
    {
        DatabaseContext db = new DatabaseContext();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        // GET: Profile
        public ActionResult Index(int? userId)
        {

            if (userId != null)
            {
                var user = db.Users.Include("Profile").Where(u => u.Id == userId).FirstOrDefault();
                if(user.Login!=User.Identity.Name)
                {
                    ViewBag.Other = true;
                }
                else
                {
                    ViewBag.Other = false;
                }
                if (user.Profile != null)
                {
                    return View(user.Profile);
                }
            }

            if (User.Identity.IsAuthenticated)
            {
                var user = db.Users.Include("Profile").Where(u => u.Login == User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var prof = user.Profile;
                    if (prof != null)
                    {
                        return View(prof);
                    }
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }


        }

        public ActionResult EditProfile()
        {
            var user = db.Users.Include("Profile").Where(u=>u.Login==User.Identity.Name).FirstOrDefault();
            return View(new EditProfileViewModel()
                    {
                        Chair=user.Profile.Chair,
                        Group=user.Profile.Group,
                        Country = user.Profile.Country,
                        Phone = user.Profile.Phone,
                        Skype = user.Profile.Skype,
                        AvatarPath = user.Profile.AvatarPath,
                        FirstName=user.Profile.FirstName,
                        SecondName=user.Profile.SecondName,
                        ThirdName=user.Profile.ThirdName
                    });
        }
        [HttpPost,ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult EditProfile(EditProfileViewModel model,HttpPostedFileBase fileUpload)
        {
            if(ModelState.IsValid)
            {
                if(fileUpload!=null)
                {
                    model.AvatarPath = "/Images/" + User.Identity.Name + fileUpload.FileName;
                    fileUpload.SaveAs(HttpContext.Server.MapPath("~/Images/" +User.Identity.Name+fileUpload.FileName));
                }
                UserManager.EditProfile(model,User.Identity.Name);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }
        [Authorize]
        [HttpGet]
        public ActionResult Search(int? age,string searchString)
        {
            if (age != null)
            {
                var users = db.Users.Include("Profile").Where(u => u.Profile.Date.Year == age && u.Login != User.Identity.Name);
                return View(users);
            }
            if(searchString!=null)
            {
                return View(UserManager.SearchUsers(searchString));
            }
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public ActionResult Requests()
        {
            var iam=db.Users.Include("Profile").Where(u=>u.Login==User.Identity.Name).FirstOrDefault();
            var requestsList = db.Requests.Where(r => r.To == iam.Id && r.Description == "friendadd");
            List<RequestModel> model= new List<RequestModel>();
           
            
            foreach(var item in requestsList)
            {
                using (DatabaseContext dbT = new DatabaseContext())
                {
                    var userFrom = dbT.Users.Include("Profile").Where(u => u.Id == item.From).FirstOrDefault();
                    model.Add(new RequestModel()
                    {
                        Id = item.Id,
                        From = userFrom,
                        To = iam
                    });
                }
            }
            return View(model);
        }
        [Authorize]
        public ActionResult OutgoingRequests()
        {
            var iam = db.Users.Include("Profile").Where(u => u.Login == User.Identity.Name).FirstOrDefault();
            var requestsList = db.Requests.Where(r => r.From == iam.Id && r.Description == "friendadd");
            List<RequestModel> model = new List<RequestModel>();


            foreach (var item in requestsList)
            {
                using (DatabaseContext dbT = new DatabaseContext())
                {
                    var userTo = dbT.Users.Include("Profile").Where(u => u.Id == item.To).FirstOrDefault();
                    model.Add(new RequestModel()
                    {
                        Id = item.Id,
                        To = userTo,
                        From = iam
                    });
                }
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult AddToFriends(int? friend)
        {
            if (friend != null)
            {
                FriendsManager.AddToFriends(friend, User.Identity.Name);
            }
            return RedirectToAction("Requests");
        }
        [Authorize]
        public ActionResult Friends()
        {
            var user = db.Users.Include("Profile").Where(u => u.Login == User.Identity.Name).FirstOrDefault();
            return View(FriendsManager.GetFriends(user.Profile.FriendsList));
        }
        [Authorize]
        public ActionResult DeleteFriend(int? friend,string returnUrl)
        {
            if (friend != null)
            {
                FriendsManager.DeleteFriend(friend,User.Identity.Name);
            }
            return Redirect(returnUrl);
        }
        [Authorize]
        public ActionResult SendFriendRequest(int? friend,string returnUrl)
        {
            if(friend!=null)
            {
                int fr=(int)friend;
                var iam=db.Users.Where(u=>u.Login==User.Identity.Name).FirstOrDefault();

                var req = db.Requests.Where(r => r.From == fr && r.To == iam.Id).FirstOrDefault();
                if (req != null)
                {
                    FriendsManager.AddToFriends(friend, User.Identity.Name);
                }
                else
                {

                    db.Requests.Add(new Request() { From = iam.Id, To = fr, Description = "friendadd" });
                    db.SaveChanges();
                }
            }
            return Redirect(returnUrl);
        }
        [Authorize]
        public ActionResult DeleteRequest(int? friend)
        {
            if(friend!=null)
            {
                var iam=db.Users.Where(u=>u.Login==User.Identity.Name).FirstOrDefault();
                var req = db.Requests.Where(u => u.From == friend && u.To == iam.Id).FirstOrDefault();
                db.Requests.Remove(req);
                db.SaveChanges();
            }
            return RedirectToAction("Requests");
        }
        [Authorize]
        public ActionResult DeleteOutgoingRequest(int? friend)
        {
            if (friend != null)
            {
                var iam = db.Users.Where(u => u.Login == User.Identity.Name).FirstOrDefault();
                var req = db.Requests.Where(u => u.To == friend && u.From == iam.Id).FirstOrDefault();
                db.Requests.Remove(req);
                db.SaveChanges();
            }
            return RedirectToAction("OutgoingRequests");
        }
    }
}