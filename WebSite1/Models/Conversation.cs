using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public int UserOne { get; set; }
        public int UserTwo { get; set; }
    }
}