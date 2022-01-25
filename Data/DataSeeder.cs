using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Data
{
    public class DataSeeder
    {
        public static Task AddSuperUser(AppDbContext dbContext)
        {
            var roleStore = new RoleStore<IdentityRole>(dbContext);
            var userStore = new UserStore<AppUser>(dbContext);

            var user = new AppUser
            {
                UserName = "ForumAdmin",
                NormalizedUserName = "forumAdmin",
                Email = "tolqin233@gmail.com",
                NormalizedEmail = "tolqin233@gmail.com",
                EmailConfirmed = true,
                LockoutEnabled = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var hasher = new PasswordHasher<AppUser>();
            var hashedPassword = hasher.HashPassword(user, "admin");
            user.PasswordHash = hashedPassword;


            var isAdminRole = dbContext.Roles.Any(r => r.Name == "Admin");

            if (!isAdminRole)
            {
                roleStore.CreateAsync(new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "admin"
                });
            }

            var hasSuperUser = dbContext.Users.Any(u => u.NormalizedUserName == user.UserName);

            if (!hasSuperUser)
            {
                userStore.CreateAsync(user);
                userStore.AddToRoleAsync(user, "Admin");
            }

            dbContext.SaveChanges();

            return Task.CompletedTask;
        }
    }
}
