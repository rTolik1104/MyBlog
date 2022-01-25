using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class PostController : Controller
    {
        private readonly IPostservices _postservices;
        private readonly IUserServices _userServices;
        private readonly IFriendServices _friendServices;

        private static UserManager<AppUser> _userManager;

        public PostController(IFriendServices friendServices, IPostservices postservices,IUserServices userServices, UserManager<AppUser> userManager)
        {
            _friendServices = friendServices;
            _postservices = postservices;
            _userServices = userServices;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var posts = new List<Post>();
            var user = await _userManager.GetUserAsync(User);
            posts = _friendServices.GetFriendsPosts(user.Id).ToList();

            var postList = posts.Select(p => new PostListVM
            {
                Id = p.Id,
                Title = p.Title,
                Authorid = p.User.Id,
                AuthorImage = p.User.ProfileImgUrl,
                AuthorName=p.User.UserName,
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

        public IActionResult Details(int id)
        {
            var post = _postservices.GetByid(id);
            var replies = BuildPostReplies(post.PostReplies);

            var model = new PostListVM
            {
                Id = post.Id,
                Title = post.Title,
                Authorid = post.User.Id,
                AuthorImage = post.User.ProfileImgUrl,
                PostContent = post.Description,
                PostPictureUrl = post.PostPictureUrl,
                DatePosted = post.Created.ToString(),
                RepliesCount = post.PostReplies.Count(),
                AuthorName=post.User.UserName,
                LikesCount = post.LikesCount,
                PostReplies = replies
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

        [Authorize]
        public IActionResult Create()
        {
            var model = new CreatePostModel
            {
                AuthorName = User.Identity.Name,
                LikesCount = 0
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostModel model)
        {
            var userId = _userManager.GetUserId(User);
            var user = _userManager.FindByIdAsync(userId).Result;

            if (ModelState.IsValid)
            {
                var post = BuildPost(model, user);
                await _postservices.Add(post);
            }
            else
            {
                ModelState.AddModelError("", "Invalid create post");
            }

            return RedirectToAction(nameof(Index));
        }

        private Post BuildPost(CreatePostModel model, AppUser user)
        {
            return new Post
            {
                Title = model.Title,
                PostPictureUrl = model.PostPictureUrl,
                Description = model.Content,
                Created=DateTime.Now,
                User = user
            };
        }

        [Authorize]
        public IActionResult Images()
        {
            var posts = new List<Post>();
            posts = _postservices.GetAllPosts().Where(x=>x.PostPictureUrl!=null).OrderByDescending(x => x.Id).ToList();

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

        public static Dictionary<int, List<AppUser>> _likes = new();
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Like(int id)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            if (!_likes.ContainsKey(id))
            {
                _likes.Add(id, new List<AppUser>());
            }

            var user1 = _likes[id].FirstOrDefault(x => x.UserName == user.UserName);
            if (user1==null)
            {
                _likes[id].Add(user);
                await _postservices.IncrementLikesCount(id);
            }

            return Json(_postservices.GetByid(id).LikesCount);
        }
    }
}
