using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyBlog.Data;
using MyBlog.Data.Interfaces;
using MyBlog.Models;
using MyBlog.ViewModels.Profile;
using System;
using System.Collections.Generic;
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
                ProfileImgUrl = user.ProfileImgUrl,
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
            var model = new Friend
            {
                AccountId = currentUser.Id,
                FriendId = friend.Id,
                Added = DateTime.Now
            };

            try
            {
                await _friendServices.AddFriend(model);
            }
            catch(Exception ex)
            {
                _logger.LogInformation("Error in add friend action", ex);
                return RedirectToAction(nameof(Friends));
            }

            return RedirectToAction(nameof(Friends));
        }

        [Authorize]
        public async Task<IActionResult> Friends(string searchString)
        {
            var user = await _userManager.GetUserAsync(User);
            var model = _friendServices.GetFriends(user.Id);

            if (!String.IsNullOrEmpty(searchString))
            {
                model = model.Where(u => u.UserName.Contains(searchString) || u.Email.Contains(searchString));
            }

            return View(model);
        }


        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userServices.GetById(id);

            if(user == null)
            {
                return RedirectToAction(nameof(Details));
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,AppUser model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _userServices.Update(id,model);

                return RedirectToAction(nameof(Details));
            }
            catch(Exception ex)
            {
                _logger.LogInformation("Error:", ex);
                return View(model);
            }
        }
    }
}
