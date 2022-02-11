using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using MyBlog.Data.Interfaces;
using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Data.Services
{
    public class PostServices : IPostservices
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _webHost;
        private readonly ILogger _logger;

        public PostServices(AppDbContext dbContext,IWebHostEnvironment webHost,ILogger<PostServices> logger)
        {
            _dbContext = dbContext;
            _webHost=webHost;
            _logger=logger;
        }


        public async Task AddPost(Post post)
        {
            try
            {
                _dbContext.Posts.Add(post);
                await _dbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogInformation("Error:" + ex.Message);
                throw;
            }
        }

        public async Task AddPostReply(PostReply postReply)
        {
            try
            {
                _dbContext.Add(postReply);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error:" + ex.Message);
                throw;
            }
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

        public IEnumerable<Post> GetFilterdPosts(int? id, string search)
        {
            return GetAllPosts()
                .Where(p => p.Title.Contains(search)
                || p.Description.Contains(search));
        }

        public IEnumerable<Post> GetPostsByUser(string id)
        {
            return GetAllPosts().Where(u => u.User.Id == id);
        }

        public async Task IncrementPostLikesCount(int id)
        {
            var post = GetByid(id);
            post.LikesCount = GetIncrement(post.LikesCount);
            await _dbContext.SaveChangesAsync();
        }

        private int GetIncrement(int likesCount)
        {
            return likesCount + 1;
        }

        public async Task IncrementPostReplyLikesCount(int id)
        {
            var postReply = GetReplyById(id);
            postReply.LikesCount = GetReplyIncrement(postReply.LikesCount);
            await _dbContext.SaveChangesAsync();
        }

        private int GetReplyIncrement(int likesCount)
        {
            return likesCount + 1;
        }

        public async Task UpdatePost(int id, Post post)
        {
            try
            {
                EntityEntry entityEntry = _dbContext.Entry<Post>(post);
                entityEntry.State = EntityState.Modified;

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error:" + ex.Message);
                throw;
            }
        }

        public string SaveImage(IFormFile newFile)
        {
            string UniqueName = string.Empty;
            if(newFile.FileName !=null)
            {
                string uploadFolder = Path.Combine(_webHost.WebRootPath, "Images");
                UniqueName = Guid.NewGuid().ToString() + "_" + newFile.FileName;
                string filePath=Path.Combine(uploadFolder, UniqueName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    newFile.CopyTo(fileStream);
                    fileStream.Close();
                }
            }

            return UniqueName;
        }

        public async Task DeletePost(int id)
        {
            Post post = await _dbContext.Posts.FirstOrDefaultAsync(x => x.Id == id);
            try
            {
                if (post.PostPictureUrl != null)
                {
                    string uploadFolder = Path.Combine(_webHost.WebRootPath, "Images");
                    string filePath = Path.Combine(uploadFolder, post.PostPictureUrl);

                    FileInfo fileInfo = new FileInfo(filePath);
                    if (fileInfo.Exists)
                    {
                        fileInfo.Delete();
                    }
                }
                _dbContext.Posts.Remove(post);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error:" + ex.Message);
                throw;
            }
            
        }
    }
}
