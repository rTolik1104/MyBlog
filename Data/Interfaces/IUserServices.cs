using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Data.Interfaces
{
    public interface IUserServices
    {
        Task<AppUser> GetById(string id);
        IEnumerable<AppUser> GetAll();
        IEnumerable<AppUser> GetFilterd(string search);


        Task SetProfileImg(string id, Uri uri);
        Task Update(string id,AppUser appUser);
    }
}
