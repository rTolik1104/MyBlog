using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models
{
    public class Friend
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public string FriendId { get; set; }
        public DateTime Added { get; set; }
    }
}
