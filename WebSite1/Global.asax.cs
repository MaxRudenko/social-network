using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebSite.Filtres;
using WebSite.Models;

namespace WebSite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            
            GlobalFilters.Filters.Add(new MyActionAttribute());


            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //sUserManager.InitialazingAdmin(); //Инициализация учетной записи админа
            //MailMessanger.SendMessage("brut_75@mail.ru", "rudenko.m.a.1995@gmail.com", "brut_75@mail.ru", "V80984342375");
            /*using(DatabaseContext db = new DatabaseContext())
            {
                var prof = db.Profiles.FirstOrDefault();
                prof.FriendsList = null;
                db.SaveChanges();

            }
           */
            
        }
    }
}
