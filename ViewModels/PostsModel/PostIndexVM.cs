using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.ViewModels.PostsModel
{
    public class PostIndexVM
    {
        public List<PostListVM> Posts { get; set; }
    }
}
