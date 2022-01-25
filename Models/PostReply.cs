using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models
{
    public class PostReply
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public int LikesCount { get; set; }

        public virtual AppUser User { get; set; }
        public Post Post { get; set; }
    }
}
