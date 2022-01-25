using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models
{
    public class AppUser:IdentityUser
    {
        public string ProfileImgUrl { get; set; }
        public DateTime MemberScine { get; set; }
        public string About { get; set; }
        public string AboutTitle { get; set; }

        public virtual IEnumerable<Post> Posts { get; set; }
    }
}
