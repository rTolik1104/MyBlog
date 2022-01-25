using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Data.Interfaces
{
    public interface IFriendServices
    {
        IEnumerable<Friend> GetFriendsByAccountId(string id);

        IEnumerable<AppUser> GetFriends(string id);
        IEnumerable<Post> GetFriendsPosts(string id);

        Task AddFriend(Friend friend);
    }
}
