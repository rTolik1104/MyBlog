using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Data.Interfaces;
using MyBlog.Models;
using MyBlog.ViewModels.PostsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Controllers
{
    public class PostReplyController : Controller
    {
        private readonly IPostservices _postservices;
        private readonly IUserServices _userServices;
        private readonly UserManager<AppUser> _userManager;

        public PostReplyController(IPostservices postservices,IUserServices userServices,UserManager<AppUser> userManager)
        {
            _postservices = postservices;
            _userServices = userServices;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Create(int id)
        {
            var post = _postservices.GetByid(id);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var model = new PostReplyModel
            {
                PostContent = post.Description,
                PostId = post.Id,
                PostTitle=post.Title,

                AuthorId = user.Id.ToString(),
                AuthorName=user.UserName,
                AuthorImage=user.ProfileImgUrl,
                DatePosted = DateTime.Now.ToString()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PostReplyModel model)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            var reply = BuildReply(model, user);

            await _postservices.AddReply(reply);

            return RedirectToAction("Index","Post");
        }

        public static Dictionary<int, List<AppUser>> _likesReply = new();
        [HttpPost]
        public async Task<IActionResult> Like(int id)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            if (!_likesReply.ContainsKey(id))
            {
                _likesReply.Add(id, new List<AppUser>());
            }
            
            var user1 = _likesReply[id].FirstOrDefault(x => x.UserName == user.UserName);

            if (user1 == null)
            {
                _likesReply[id].Add(user);
                await _postservices.IncrementReplyLikesCount(id);
            }

            return Json(_postservices.GetReplyById(id).LikesCount);
        }

        private PostReply BuildReply(PostReplyModel model, AppUser user)
        {
            var post = _postservices.GetByid(model.PostId);

            return new PostReply
            {
                Post = post,
                Content = model.PostReplyContent,
                Created = DateTime.Now,
                User = user
            };
        }
    }
}
