using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyBlog.Models;
using MyBlog.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private ILogger _logger;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model,string returnUrl)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);

                try
                {
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"{user.Id} enter the system");
                        return RedirectToAction("Index", "Post");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Login atepment");
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogInformation("Error in Login:", ex);
                }
                
            }

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Register(string returnurl)
        {
            ViewData["ReturnUrl"] = returnurl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterVM model,string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.UserName, Email = model.Email, MemberScine = DateTime.Now,ProfileImgUrl="Images/default.jpg",PhoneNumber="000000000", };
                var result = await _userManager.CreateAsync(user, model.Password);

                try
                {
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation($"{user.Id} enter the system");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid register!");
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogInformation("Error in Register action:", ex);
                }
                
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
            }
            catch(Exception ex)
            {
                _logger.LogInformation("Error in Logout action:", ex);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
