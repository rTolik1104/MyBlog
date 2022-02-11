using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.ViewModels.Profile
{
    public class ProfileVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ProfileImgName { get; set; }
        public string PhoneNumber { get; set; }
        public string AboutTitle { get; set; }
        public string About { get; set; }
        public int ImagesCount { get; set; }
        public int PostsCount { get; set; }
        public int FriendsCount { get; set; }
        public DateTime MemberScine { get; set; }
        public List<AppUser> Friends { get; set; }
        
        public bool isAdmin { get; set; }
        public bool isFriend { get; set; }
    }
}
