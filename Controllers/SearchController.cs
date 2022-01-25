using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Data;
using MyBlog.Data.Interfaces;
using MyBlog.Models;
using MyBlog.ViewModels;
using MyBlog.ViewModels.PostsModel;
using System;
using System.Collections.Generic;
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

        public IActionResult Index(string searchString)
        {
            var users = _dbContext.Users.ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.UserName.Contains(searchString, StringComparison.InvariantCultureIgnoreCase) || u.Email.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            return View(users);
        }

        public async Task<IActionResult> Posts(string searchString)
        {
            var user = await _userManager.GetUserAsync(User);

            var posts = new List<Post>();
            posts = _friendServices.GetFriendsPosts(user.Id).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                posts = posts.Where(u => u.Description.Contains(searchString) || u.Title.Contains(searchString) || u.User.UserName.Contains(searchString)).ToList();
            }

            var postList = posts.Select(p => new PostListVM
            {
                Id = p.Id,
                Title = p.Title,
                Authorid = p.User.Id,
                AuthorImage = p.User.ProfileImgUrl,
                AuthorName = p.User.UserName,
                PostContent = p.Description,
                PostPictureUrl = p.PostPictureUrl,
                DatePosted = p.Created.ToString(),
                RepliesCount = p.PostReplies.Count(),
                LikesCount = p.LikesCount,
                PostReplies = BuildPostReplies(p.PostReplies)
            });

            var model = new PostIndexVM
            {
                Posts = postList.ToList()
            };

            return View(model);
        }

        private IEnumerable<PostReplyModel> BuildPostReplies(IEnumerable<PostReply> postReplies)
        {
            return postReplies.Select(r => new PostReplyModel
            {
                Id = r.Id,
                PostContent = r.Content,
                DatePosted = r.Created.ToString(),
                LikesCount = r.LikesCount,
                AuthorName = r.User.UserName,
                AuthorId = r.User.Id,
                AuthorImage = r.User.ProfileImgUrl
            });
        }
    }
}
