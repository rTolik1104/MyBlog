using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.ViewModels.PostsModel
{
    public class PostReplyModel
    {
        public int Id { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImage { get; set; }
        public string PostContent { get; set; }
        public string DatePosted { get; set; }
        public string AuthorId { get; set; }
        public int LikesCount { get; set; }

        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostReplyContent { get; set; }
    }
}
