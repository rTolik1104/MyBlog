using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using MyBlog.Data.Interfaces;
using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Data.Services
{
    public class UserServices:IUserServices
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public UserServices(AppDbContext dbContext,ILogger<UserServices> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


        public IEnumerable<AppUser> GetAll() => _dbContext.Users;

        public async Task<AppUser> GetById(string id) =>await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        public IEnumerable<AppUser> GetFilterd(string search)
        {
            return GetAll().Where(u => u.UserName.Contains(search));
        }

        public async Task SetProfileImg(string id, Uri uri)
        {
            var user = await GetById(id);
            user.ProfileImgUrl = uri.AbsoluteUri;
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(string id,AppUser appUser)
        {
            try
            {
                 _dbContext.Update(appUser);

                await _dbContext.SaveChangesAsync();
            }

            catch(Exception ex) {
                _logger.LogInformation("Error in update User:" + ex);
                return;
            }
        }
    }
}
