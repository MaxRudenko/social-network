using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    static public class FriendsManager
    {
        static public int Count(string name)
        {
            if (name != null)
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    var user = db.Users.Include("Profile").Where(u => u.Login == name).FirstOrDefault();
                    if (user != null)
                    {
                        var friendsList = Parsing(user.Profile.FriendsList);
                        return friendsList.Count();
                    }
                }
            }
            return 0;
        }
        static public string[] Parsing(string friends)
        {
            if (friends != null)
            {
                var list = friends.Split(' ');

                return list;
            }
            else
            {
                return new string[] { };
            }
        }
        static public bool Check(string name,int? userId)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var user = db.Users.Include("Profile").Where(u => u.Login == name).FirstOrDefault();
                if (user != null)
                {
                    var friendsList = Parsing(user.Profile.FriendsList);
                    var result =friendsList.Where(friend => friend == userId.ToString()).FirstOrDefault();
                    if(result!=null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        static public bool CheckOutgoingRequest(string name,int To)
        {
            using(DatabaseContext db = new DatabaseContext())
            {
                var iam=db.Users.Where(u=>u.Login==name).FirstOrDefault();
                if(iam!=null)
                {
                    var request = db.Requests.Where(r => r.To == To & r.From == iam.Id).FirstOrDefault();
                    if(request==null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return true;
        }
        static public List<User> GetFriends(string friendList)
        {
            var list = Parsing(friendList);
            List<User> myfriends = new List<User>();
            
                foreach (var friendId in list)
                {
                    if (friendId != "")
                    {
                        using (DatabaseContext db = new DatabaseContext())
                        {
                            var Id = Convert.ToInt32(friendId);
                            var friend = db.Users.Include("Profile").Where(u => u.Id == Id).FirstOrDefault();
                            myfriends.Add(friend);
                        }
                    }
                }
            
            return myfriends;
        }
        static public void AddToFriends(int? id,string name)
        {
            using(DatabaseContext db = new DatabaseContext())
            {
                var iam = db.Users.Include("Profile").Where(u => u.Login == name).FirstOrDefault();
                var myfriend = db.Users.Include("Profile").Where(u => u.Id == id).FirstOrDefault();
                var request = db.Requests.Where(r => r.From == myfriend.Id && r.To == iam.Id).FirstOrDefault();
                db.Requests.Remove(request);
                if (myfriend.Profile.FriendsList == null)
                {
                    myfriend.Profile.FriendsList = iam.Id.ToString();
                }
                else
                {
                    myfriend.Profile.FriendsList += " " + iam.Id.ToString();
                }
                if (iam.Profile.FriendsList == null)
                {
                    iam.Profile.FriendsList = myfriend.Id.ToString();
                }
                else
                {
                    iam.Profile.FriendsList += " " + myfriend.Id.ToString();
                }
                db.SaveChanges();
            }
        }
        static public void DeleteFriend(int? id,string name)
        {
            using(DatabaseContext db = new DatabaseContext())
            {
                var user = db.Users.Include("Profile").Where(u => u.Id == id).FirstOrDefault();
                var iam = db.Users.Include("Profile").Where(u => u.Login == name).FirstOrDefault();
                
                var userlist = Parsing(user.Profile.FriendsList);
                user.Profile.FriendsList = null;
                for(int i=0;i<userlist.Count();i++)
                {
                    if(userlist[i]!=iam.Id.ToString())
                    {
                        if(user.Profile.FriendsList==null)
                        {
                            user.Profile.FriendsList = userlist[i];
                        }
                        else
                        {
                            user.Profile.FriendsList =" "+ userlist[i];
                        }
                    }
                }

                var iamlist = Parsing(iam.Profile.FriendsList);
                iam.Profile.FriendsList = null;
                for (int i = 0; i < iamlist.Count(); i++)
                {
                    if (iamlist[i] != user.Id.ToString())
                    {
                        if (iam.Profile.FriendsList == null)
                        {
                            iam.Profile.FriendsList = iamlist[i];
                        }
                        else
                        {
                            iam.Profile.FriendsList = " " + iamlist[i];
                        }
                    }
                }
                db.SaveChanges();
            }
        }
        static public int CheckCountRequest(string name)
        {
            int count = 0;
            if (name != "")
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    var user = db.Users.Where(u => u.Login == name).FirstOrDefault();
                    var req = db.Requests.Where(r => r.To == user.Id).ToList();
                    count = req.Count();
                }
            }
            return count;
        }
        static public int CheckCountOutgoingRequest(string name)
        {
            int count = 0;
            using (DatabaseContext db = new DatabaseContext())
            {
                var user = db.Users.Where(u => u.Login == name).FirstOrDefault();
                var req = db.Requests.Where(r => r.From == user.Id).ToList();
                count = req.Count();
            }
            return count;
        }
    }
}