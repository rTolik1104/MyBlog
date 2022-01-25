using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.ViewModels.PostsModel
{
    public class CreatePostModel
    {
        public string AuthorName { get; set; }
        public string PostPictureUrl { get; set; }
        public int LikesCount { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
