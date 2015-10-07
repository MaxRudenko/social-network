using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class MessengerController : Controller
    {
        DatabaseContext db = new DatabaseContext();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        // GET: Messenger
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DeleteConversation(int? id)
        {
            var con = db.Conversations.Find(id);
            if(con!=null)
            {
                db.Conversations.Remove(con);
                db.SaveChanges();
            }
            return RedirectToAction("Conversations");
        }
        public ActionResult Conversations()
        {
            var iam=db.Users.Where(u=>u.Login==User.Identity.Name).FirstOrDefault();
            User friend;
            Message msg;

            var conversations = db.Conversations.Where(con => con.UserOne == iam.Id || con.UserTwo == iam.Id);
            List<ConversationViewModel> model = new List<ConversationViewModel>();
            foreach (var item in conversations)
            {
                using (DatabaseContext db1 = new DatabaseContext())
                {
                    msg = db1.Messages.Where(m => m.ConversationId == item.Id).ToList().LastOrDefault();
                    if (msg == null)
                    {
                        msg = new Message() { Text = " " };
                    }
                    if (iam.Id == item.UserOne)
                    {
                        friend = db1.Users.Include("Profile").Where(u => u.Id == item.UserTwo).FirstOrDefault();
                    }
                    else
                    {
                        friend = db1.Users.Include("Profile").Where(u => u.Id == item.UserOne).FirstOrDefault();
                    }

                    model.Add(new ConversationViewModel()
                    {
                        Id=item.Id,
                        Iam = iam,
                        Friend = friend,
                        Messages = new List<Message>() { msg }
                    });
                }
            }
            return View(model);
        }
        [Authorize]
        public ActionResult SearchConversation(int? userid,string returnUrl)
        {
            var user = db.Users.Where(u => u.Id == userid).FirstOrDefault();
            var iam = db.Users.Where(u => u.Login == User.Identity.Name).FirstOrDefault();
            if(user!=null && iam!=null)
            {
                Conversation conversation;
                if(user.Id>iam.Id)
                {
                    conversation=db.Conversations.Where(con => con.UserOne == user.Id && con.UserTwo == iam.Id).FirstOrDefault();
                    
                }else{
                    conversation = db.Conversations.Where(con => con.UserOne == iam.Id && con.UserTwo == user.Id).FirstOrDefault();
                }
                if(conversation!=null)
                {
                    return RedirectToAction("Conversation", new {id = conversation.Id });
                }
                else
                {
                    int userOne,userTwo;
                    if(user.Id>iam.Id)
                    {
                        userOne = user.Id;
                        userTwo = iam.Id;
                    }
                    else
                    {
                        userOne = iam.Id;
                        userTwo = user.Id;
                    }
                    var con=db.Conversations.Add(new Conversation()
                    {
                        UserOne=userOne,
                        UserTwo=userTwo
                    });
                    db.SaveChanges();
                    return RedirectToAction("Conversation", new { id = con.Id });
                }
            }
            return Redirect(returnUrl);
        }
        public ActionResult Conversation(int? id)
        {
            if(id==null)
            {
                return RedirectToAction("Index", "Home");
            }
            var iam = db.Users.Include("Profile").Where(u => u.Login == User.Identity.Name).FirstOrDefault();
            var conversation=db.Conversations.Where(con=>con.Id==id).FirstOrDefault();
            int friendid;
            if(conversation.UserOne==iam.Id)
            {
                friendid=conversation.UserTwo;
            }else{
                friendid=conversation.UserOne;
            }
            var friend = db.Users.Include("Profile").Where(u => u.Id == friendid).FirstOrDefault();
            var messageList = db.Messages.Where(r => r.ConversationId == id).ToList() ;
           // messageList.Reverse();
            ViewBag.AvatarPath = iam.Profile.AvatarPath;
            var conversModel = new ConversationViewModel()
            {
                Messages = messageList,
                Iam=iam,
                Friend=friend,
                Id=(int)id
            };
            ViewBag.IdConversation = id;
            return View(conversModel);
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Send(Message model,int? idConversation,string returnUrl)
        {
            
            if (ModelState.IsValid)
            {
                var iam = db.Users.Where(u => u.Login == User.Identity.Name).FirstOrDefault();
                db.Messages.Add(new Message()
                {
                    Text = model.Text,
                    Time = DateTime.Now,
                    UserId = iam.Id,
                    Ip = Request.UserHostAddress,
                    ConversationId = (int)idConversation
                });
                db.SaveChanges();
                return Redirect(returnUrl);
            }
            else
            {
                return View(model);
            }
        }
    }
}