using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyBlog.Data.Interfaces;
using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Data.Services
{
    public class PostServices : IPostservices
    {
        private readonly AppDbContext _dbContext;

        public PostServices(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Add(Post post)
        {
            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddReply(PostReply postReply)
        {
            _dbContext.Add(postReply);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var post =await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
            _dbContext.Posts.Remove(post);
            await _dbContext.SaveChangesAsync();
        }

        public IEnumerable<Post> GetAllPosts()
        {
            return _dbContext.Posts
                    .Include(u => u.User)
                    .Include(p => p.PostReplies)
                    .ThenInclude(u => u.User);
        }

        public Post GetByid(int id)
        {
            return _dbContext.Posts.Where(p => p.Id == id)
                .Include(u => u.User)
                .Include(p => p.PostReplies)
                .ThenInclude(u => u.User)
                .First();
        }

        public PostReply GetReplyById(int id)
        {
            return _dbContext.PostReplies.Where(p => p.Id == id)
                .Include(u => u.User)
                .First();
        }

        public IEnumerable<Post> GetFilterdPosts(int id, string search)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Post> GetFilterdPosts(string search)
        {
            return GetAllPosts()
                .Where(p => p.Title.Contains(search) 
                || p.Description.Contains(search));
        }

        public IEnumerable<Post> GetPostsByUser(string id)
        {
            return GetAllPosts().Where(u => u.User.Id == id);
        }

        public async Task IncrementLikesCount(int id)
        {
            var post = GetByid(id);
            post.LikesCount = GetIncrement(post.LikesCount);
            await _dbContext.SaveChangesAsync();
        }

        private int GetIncrement(int likesCount)
        {
            return likesCount + 1;
        }

        public async Task IncrementReplyLikesCount(int id)
        {
            var postReply = GetReplyById(id);
            postReply.LikesCount = GetReplyIncrement(postReply.LikesCount);
            await _dbContext.SaveChangesAsync();
        }

        private int GetReplyIncrement(int likesCount)
        {
            return likesCount + 1;
        }

        public async Task Update(int id, Post post)
        {
            EntityEntry entityEntry = _dbContext.Entry<Post>(post);
            entityEntry.State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }
    }
}
