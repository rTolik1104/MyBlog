using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.ViewModels.Profile
{
    public class EditVM
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ProfileImageName { get; set; }
        public IFormFile ProfileImgFile { get; set; }
        public string PhoneNumber { get; set; }
        public string About { get; set; }
    }
}
