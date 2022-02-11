using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using MyBlog.Data.Interfaces;
using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Data.Services
{
    public class UserServices:IUserServices
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _webHost;

        public UserServices(AppDbContext dbContext,ILogger<UserServices> logger,IWebHostEnvironment webHost)
        {
            _dbContext = dbContext;
            _logger = logger;
            _webHost = webHost;
        }


        public IEnumerable<AppUser> GetAll()
        {
            try
            {
                return _dbContext.Users;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error: "+ex.Message);
                throw;
            }
        } 

        public async Task<AppUser> GetById(string id)
        {
            try
            {
                return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error: " + ex.Message);
                throw;
            }

        }

        public IEnumerable<AppUser> GetFilterd(string search)
        {
            return GetAll().Where(u => u.UserName.Contains(search));
        }

        public string SetProfileImg(IFormFile newFile)
        {
            string UniqueName = string.Empty;
            if (newFile.FileName != null)
            {
                string uploadFolder = Path.Combine(_webHost.WebRootPath, "Images");
                UniqueName = Guid.NewGuid().ToString() + "_" + newFile.FileName;
                string filePath = Path.Combine(uploadFolder, UniqueName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    newFile.CopyTo(fileStream);
                    fileStream.Close();
                }
            }

            return UniqueName;
        }

        public async Task Update(AppUser appUser)
        {
            try
            {
                 _dbContext.Update(appUser);

                await _dbContext.SaveChangesAsync();
            }
            catch(Exception ex) {
                _logger.LogInformation("Error in update User:" + ex);
                throw;
            }
        }
    }
}
