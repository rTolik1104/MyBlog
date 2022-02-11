using Microsoft.AspNetCore.Http;
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

        Task Update(AppUser appUser);
        string SetProfileImg(IFormFile file);
    }
}
