using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class Profile
    {
        [Key]
        [ForeignKey("User")]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public DateTime Date { get; set; }
        public string Gender { get; set; }
        public string AvatarPath { get; set; }
        public string Skype { get; set; }
        public string Phone { get; set; }
        public string Group { get; set; }
        public string Chair { get; set; }
        public string Country { get; set; }
        public string FriendsList { get; set; }
        public User User { get; set; }
    }
}