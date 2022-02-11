using Microsoft.AspNetCore.Http;
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
        IEnumerable<Post> GetFilterdPosts(int? id,string search);
        IEnumerable<Post> GetPostsByUser(string id);

        Task AddPost(Post post);
        Task DeletePost(int id);
        Task UpdatePost(int id, Post post);
        Task AddPostReply(PostReply postReply);
        Task IncrementPostLikesCount(int id);
        Task IncrementPostReplyLikesCount(int id);
        string SaveImage(IFormFile formFile);
    }
}
