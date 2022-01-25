using MyBlog.ViewModels.PostsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.ViewModels
{
    public class PostListVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImage { get; set; }
        public string PostContent { get; set; }
        public string PostPictureUrl { get; set; }
        public string DatePosted { get; set; }
        public string Authorid { get; set; }
        public int RepliesCount { get; set; }
        public int LikesCount { get; set; }

        public IEnumerable<PostReplyModel> PostReplies { get; set; }
    }
}
