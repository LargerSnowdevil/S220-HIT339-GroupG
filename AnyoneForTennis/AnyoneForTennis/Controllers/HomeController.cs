using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AnyoneForTennis.Models;
using Microsoft.AspNetCore.Authorization;
using AnyoneForTennis.Data;
using Microsoft.AspNetCore.Identity;

namespace AnyoneForTennis.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppDBContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, AppDBContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            //Todo Redirect to next action
            return View();
        }

        public IActionResult Privacy()
        {
            //Todo Remove later
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            //Todo Remove later
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //-----------------------------------------------------------------------------------------------------------------------
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(String username, String password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var user = new IdentityUser
            {
                UserName = model.Username
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (user.UserName.CompareTo("Admin") == 0)
                {
                    var member = new Member
                    {
                        Username = model.Username,
                        Email = model.Email,
                        IsAdmin = true
                    };

                    _context.Members.Add(member);
                    _context.SaveChanges();
                }
                else
                {
                    if (model.IsCoach)
                    {
                        //is a coach
                        var member = new Coach
                        {
                            Name = model.Name,
                            Age = model.Age,
                            Biography = model.Biography,
                            Username = model.Username
                        };

                        _context.Coaches.Add(member);
                        _context.SaveChanges();
                    }
                    else
                    {
                        //is a member
                        var member = new Member
                        {
                            Username = model.Username,
                            Email = model.Email,
                            IsAdmin = false
                        };

                        _context.Members.Add(member);
                        _context.SaveChanges();
                    }
                }
                

                var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    _logger.LogError("Login Failed");
                }

            }
            return View();
        }
    }
}
