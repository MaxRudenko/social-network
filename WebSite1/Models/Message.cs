using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
        public string Ip { get; set; }
        public DateTime Time { get; set; }
    }
    public class SendMessageModel
    {
        public string AvatarPath { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public DateTime Time { get; set; }
        public string Text { get; set; }
    }
}