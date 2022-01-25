using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        
        public string PostPictureUrl { get; set; }

        public int LikesCount { get; set; }
        public DateTime Created { get; set; }   


        public virtual AppUser User { get; set; }
        public virtual IEnumerable<PostReply> PostReplies { get; set; }
    }
}
