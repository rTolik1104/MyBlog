using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MyBlog.ViewModels.PostsModel
{
    public class CreatePostModel
    {
        public string AuthorName { get; set; }
        public IFormFile ImageFile { get; set; }
        public int LikesCount { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
