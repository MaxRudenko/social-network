using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class Request
    {
        public int Id { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string Description { get; set; }
    }
}