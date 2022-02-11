using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Data;
using MyBlog.Data.Interfaces;
using MyBlog.Models;
using MyBlog.ViewModels.Profile;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IFriendServices _friendServices;

        private static UserManager<AppUser> _userManager;

        public SearchController(AppDbContext dbContext, UserManager<AppUser> userManager,IFriendServices friendServices)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _friendServices = friendServices;
        }

        public async Task<IActionResult> Index(string? searchString)
        {
            var user = _userManager.GetUserAsync(User).Result;

            var users = _dbContext.Users.Select(u => new ProfileVM
            {
                UserId = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                ProfileImgName = u.ProfileImgUrl,
                MemberScine = u.MemberScine,
                isFriend = _friendServices.IsFriend(u),
                PhoneNumber = u.PhoneNumber,
                About = u.About,
                AboutTitle = u.AboutTitle
            }).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.UserName.Contains(searchString, StringComparison.OrdinalIgnoreCase) || u.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var model = new ProfileListVM
            {
                Profiles = users,
            };
            return View(model);
        }
    }
}
