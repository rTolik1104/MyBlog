using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyBlog.Data;
using MyBlog.Data.Interfaces;
using MyBlog.Models;
using MyBlog.ViewModels.Profile;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserServices _userServices;
        private readonly IFriendServices _friendServices;
        private readonly IPostservices _postservices;
        private ILogger _logger;

        public ProfileController(IUserServices userServices,UserManager<AppUser> userManager, IFriendServices friendServices, IPostservices postservices,ILogger<ProfileController> logger)
        {
            _userManager = userManager;
            _friendServices = friendServices;
            _postservices = postservices;
            _userServices = userServices;
            _logger = logger;
        }


        [Authorize]
        public async Task<IActionResult> Details()
        {
            var user = await _userManager.GetUserAsync(User);
            var userRoles =await _userManager.GetRolesAsync(user);

            var model = new ProfileVM
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                ProfileImgName = user.ProfileImgUrl,
                MemberScine = user.MemberScine,
                Friends=_friendServices.GetFriends(user.Id).ToList(),
                ImagesCount=_postservices.GetPostsByUser(user.Id).Where(p=>p.PostPictureUrl != null).Count(),
                PostsCount= _postservices.GetPostsByUser(user.Id).Count(),
                FriendsCount= _friendServices.GetFriends(user.Id).Count(),
                isAdmin = userRoles.Contains("Admin"),
                PhoneNumber=user.PhoneNumber,
                About=user.About,
                AboutTitle=user.AboutTitle
            };

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> AddFriend(string id)
        {
            var friend = await _userManager.FindByIdAsync(id);
            var currentUser = await _userManager.GetUserAsync(User);

            var model = BuildFriend(currentUser, friend);

            await _friendServices.AddFriend(model);

            return RedirectToAction(nameof(Friends));
        }

        private Friend BuildFriend(AppUser currentUser, AppUser friend)
        {
            return new Friend
            {
                AccountId = currentUser.Id,
                FriendId = friend.Id,
                Added = DateTime.Now
            };
        }

        [Authorize]
        public async Task<IActionResult> Friends(string? searchString)
        {
            var user = await _userManager.GetUserAsync(User);
            var model = _friendServices.GetFriends(user.Id);

            if (!String.IsNullOrEmpty(searchString))
            {
                model = model.Where(u => u.UserName.Contains(searchString,StringComparison.OrdinalIgnoreCase) || u.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userServices.GetById(id);

            var userModel = new EditVM
            {
                UserName = user.UserName,
                Email = user.Email,
                ProfileImageName = user.ProfileImgUrl,
                PhoneNumber = user.PhoneNumber,
                About = user.About
            };

            if(user == null)
            {
                return RedirectToAction(nameof(Details));
            }
            return View(userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,EditVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var user = EditUser(id, model);
                await _userServices.Update(user);

                var userRoles = await _userManager.GetRolesAsync(user);

                var modelView = new ProfileVM
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    ProfileImgName = user.ProfileImgUrl,
                    MemberScine = user.MemberScine,
                    Friends = _friendServices.GetFriends(user.Id).ToList(),
                    ImagesCount = _postservices.GetPostsByUser(user.Id).Where(p => p.PostPictureUrl != null).Count(),
                    PostsCount = _postservices.GetPostsByUser(user.Id).Count(),
                    FriendsCount = _friendServices.GetFriends(user.Id).Count(),
                    isAdmin = userRoles.Contains("Admin"),
                    PhoneNumber = user.PhoneNumber,
                    About = user.About,
                    AboutTitle = user.AboutTitle
                };

                return RedirectToAction(nameof(Details),modelView);
            }
            catch(Exception ex)
            {
                _logger.LogInformation("Error:", ex);
                return View(model);
            }
        }

        private AppUser EditUser(string id, EditVM model)
        {
            var user = _userServices.GetById(id);

            var newUser = user.Result;

            newUser.About = model.About;
            newUser.Email = model.Email;
            newUser.PhoneNumber= model.PhoneNumber;
            newUser.UserName = model.UserName;
            if (model.ProfileImgFile != null)
            {
                newUser.ProfileImgUrl = _postservices.SaveImage(model.ProfileImgFile);
            }
            else
            {
                newUser.ProfileImgUrl = model.ProfileImageName;
            }
            return newUser;
        }
    }
}
