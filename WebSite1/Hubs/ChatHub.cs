using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using WebSite.Models;

namespace WebSite.Hubs
{
    public class ChatHub : Hub
    {
        public override System.Threading.Tasks.Task OnConnected()
        {
            if(Context.User.Identity.IsAuthenticated)
            {
                Groups.Add(Context.ConnectionId, "authenticated");
            }
            else
            {
                Groups.Add(Context.ConnectionId, "anonymous");
            }
            return base.OnConnected();
        }
        public void SendToMultipleGroups(string[] groups)
        {
            Clients.Groups(groups).aMethodOnTheClient();
        }
        public void SendToUser(string username,string conversationId,string iamId,string friendId, string message)
        {
            if (message != null || message != "")
            {
                User iam;
                var msg = new Message()
                     {
                         Text = message,
                         Time = DateTime.Now,
                         UserId = Convert.ToInt32(iamId),
                         Ip = HttpContext.Current.Request.UserHostAddress,
                         ConversationId = Convert.ToInt32(conversationId)
                     };
                using (DatabaseContext db = new DatabaseContext())
                {
                    iam = db.Users.Include("Profile").Where(u => u.Id.ToString() == iamId).FirstOrDefault();
                    db.Messages.Add(msg);
                    db.SaveChanges();
                }
                var msgToSend = new SendMessageModel()
                {
                   AvatarPath=iam.Profile.AvatarPath,
                   Text=message,
                   Time=msg.Time,
                   UserId=iam.Id ,
                   FullName=iam.FullName
                };
                Clients.User(username).newMessage(msgToSend);
            }
        }
        public void CauseError(int a)
        {
            if(a<0)
            {
                throw new HubException("'a' must be grater than 0", a);
            }
            else if(a>0)
            {
                throw new InvalidOperationException("This will not be sent to the client");
            }
        }
    }
}