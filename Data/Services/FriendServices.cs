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

        public FriendServices(AppDbContext dbContext,IPostservices postservices)
        {
            _dbContext = dbContext;
            _postservices = postservices;
        }

        public async Task AddFriend(Friend friend)
        {
            _dbContext.Friends.Add(friend);
            await _dbContext.SaveChangesAsync();
        }

        public IEnumerable<AppUser> GetFriends(string id)
        {
            List<Friend> friends = GetFriendsByAccountId(id).ToList();
            List<string> profileId = new();

            foreach(Friend friend in friends)
            {
                profileId.Add(friend.FriendId);
            }

            List<AppUser> result = new();

            IEnumerable<AppUser> users= from a in _dbContext.Users
                                        where
                                     profileId.Contains(a.Id)
                                        select a;

            result = users.ToList();

            return result;
        }

        public IEnumerable<Post> GetFriendsPosts(string id)
        {
            List<AppUser> users = GetFriends(id).ToList();
            List<Post> posts = new();

            foreach(var user in users)
            {
                var post = _postservices.GetPostsByUser(user.Id);
                foreach(var p in post)
                {
                    posts.Add(p);
                }
            }

            var userPosts = _postservices.GetPostsByUser(id);

            foreach(var post in userPosts)
            {
                posts.Add(post);
            }

            return posts;
        }

        public  IEnumerable<Friend> GetFriendsByAccountId(string id)
        {
            List<Friend> result = new();

            IEnumerable<Friend> friends = _dbContext.Friends.Where(f => f.AccountId == id && f.FriendId != id);

            result = friends.ToList();

            var friends2 = _dbContext.Friends.Where(f => f.FriendId == id && f.AccountId != id).Select(f => new
            {
                Id = f.Id,
                AccountID = f.FriendId,
                FriendId = f.AccountId,
            });

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
    }
}
