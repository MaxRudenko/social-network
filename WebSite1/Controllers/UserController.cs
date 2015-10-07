using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebSite.Models;

namespace WebSite.Controllers
{
    [AllowAnonymous]
    public class UserController : Controller
    {
        DatabaseContext db = new DatabaseContext();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
       /* public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }*/
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
          
            if(ModelState.IsValid)
            {
                var user = UserManager.UserLogin(model.Login, model.Password, HttpContext.Request.UserHostAddress);
                if(user!=null)
                {
                    var authTicket = new FormsAuthenticationTicket(
                    10, // Version
                    user.Login, // User Login
                    DateTime.Now, // Issue-Date
                    DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), // Expiration
                    model.RememberMe, //TODO: Remember me
                    user.Role, // User Role
                    FormsAuthentication.FormsCookiePath // Cookie Path
                    );
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                    if (authTicket.IsPersistent)
                        authCookie.Expires = authTicket.Expiration;
                    Response.Cookies.Add(authCookie);
                    return RedirectToAction("Index", "Profile",new {userId=user.Id});
                }
               
               
            }
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model, string returnUrl)
        {
            if(ModelState.IsValid)
            {
                if(model.Date.Month>12 || model.Date.Day>31 || model.Date.Year>(DateTime.Now.Year-16) ||model.Date.Year<DateTime.Now.Year-100)
                {
                    ModelState.AddModelError("", "Неправильная дата!");
                    return View(model);
                }
                string role = "";
                var result= UserManager.UserCreate(model,ref role);
                if(result==false)
                {
                    ModelState.AddModelError("", "Пожалуйста введите другой email адрес");
                    return View(model);
                }
                var authTicket = new FormsAuthenticationTicket(
                   10, // Version
                   model.Login, // User Login
                   DateTime.Now, // Issue-Date
                   DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), // Expiration
                   true, //TODO: Remember me
                   role, // User Role
                   FormsAuthentication.FormsCookiePath // Cookie Path
                   );
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                if (authTicket.IsPersistent)
                    authCookie.Expires = authTicket.Expiration;
                Response.Cookies.Add(authCookie);
                return RedirectToAction("Index", "Profile");
            }
            else
            {
                return View(model);
            }
            
        }
        public JsonResult CheckUserName(string login)
        {
            bool result;
            var user = db.Users.Where(u => u.Login == login).FirstOrDefault();
            if (user == null)
                result = true;
            else result = false;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckUserEmail(string email)
        {
            bool result;
            var user = db.Users.Where(u => u.Email == email).FirstOrDefault();
            if (user == null)
                result = true;
            else result = false;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}