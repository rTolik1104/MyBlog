using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyBlog.Data.Interfaces;
using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Data.Services
{
    public class FriendServices : IFriendServices
    {
        private readonly AppDbContext _dbContext;
        private readonly IPostservices _postservices;
        private readonly ILogger _logger;


        private List<AppUser> _users = new();
        public FriendServices(AppDbContext dbContext,IPostservices postservices,ILogger<FriendServices> logger)
        {
            _dbContext = dbContext;
            _postservices = postservices;
            _logger = logger;
        }

        public async Task AddFriend(Friend friend)
        {
            try
            {
                _dbContext.Friends.Add(friend);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error: "+ex.Message);
            }
            
        }

        public IEnumerable<AppUser> GetFriends(string id)
        {
            try
            {
                List<Friend> friends = GetFriendsByAccountId(id).Result.ToList();
                List<string> profileId = new();

                foreach (Friend friend in friends)
                {
                    profileId.Add(friend.FriendId);
                }

                List<AppUser> result = new();

                IEnumerable<AppUser> users =  from a in _dbContext.Users
                                             where
                                          profileId.Contains(a.Id)
                                             select a;

                result = users.ToList();
                _users.Clear();
                foreach(AppUser user in result)
                {
                    _users.Add(user);
                }

                return result;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public IEnumerable<Post> GetFriendsPosts(string id)
        {
            try
            {
                List<AppUser> users = GetFriends(id).ToList();
                List<Post> posts = new();

                foreach (var user in users)
                {
                    var post = _postservices.GetPostsByUser(user.Id);
                    foreach (var p in post)
                    {
                        posts.Add(p);
                    }
                }

                var userPosts = _postservices.GetPostsByUser(id);

                foreach (var post in userPosts)
                {
                    posts.Add(post);
                }

                return posts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new List<Post>();
            }
            
        }

        public async  Task<IEnumerable<Friend>> GetFriendsByAccountId(string id)
        {
            List<Friend> result = new();

            IEnumerable<Friend> friends = await _dbContext.Friends.Where(f => f.AccountId == id && f.FriendId != id).ToListAsync();

            result = friends.ToList();

            var friends2 = await _dbContext.Friends.Where(f => f.FriendId == id && f.AccountId != id).Select(f => new
            {
                Id = f.Id,
                AccountID = f.FriendId,
                FriendId = f.AccountId,
            }).ToListAsync();

            foreach (Object f in friends2)
            {
                Friend friend = f as Friend;
                if (friend != null)
                {
                    result.Add(friend);
                }
            }

            return result;
        }

        public bool IsFriend(AppUser user)
        {
           return _users.Contains(user);
        }
    }
}
