﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class RequestModel
    {
        public int Id { get; set; }
        public User From { get; set; }
        public User To { get; set; }
    }
}