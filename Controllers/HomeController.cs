using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyBlog.Data.Interfaces;
using MyBlog.Models;
using MyBlog.ViewModels;
using MyBlog.ViewModels.PostsModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostservices _postservices;
        private readonly IUserServices _userServices;
        private readonly UserManager<AppUser> _userManager;
        public HomeController(ILogger<HomeController> logger,IPostservices postservices,IUserServices userServices, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _postservices = postservices;
            _userServices = userServices;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
