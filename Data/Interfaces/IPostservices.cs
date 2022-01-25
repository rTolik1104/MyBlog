using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Data.Interfaces
{
    public interface IPostservices
    {
        Post GetByid(int id);
        PostReply GetReplyById(int id);
        IEnumerable<Post> GetAllPosts();
        IEnumerable<Post> GetFilterdPosts(int id,string search);
        IEnumerable<Post> GetFilterdPosts(string search);
        IEnumerable<Post> GetPostsByUser(string id);

        Task Add(Post post);
        Task Delete(int id);
        Task Update(int id, Post post);
        Task AddReply(PostReply postReply);
        Task IncrementLikesCount(int id);
        Task IncrementReplyLikesCount(int id);
    }
}
