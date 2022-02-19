using AuthenticationWebPOC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationWebPOC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secured()
        {
            return View();
        }

        [HttpGet("/login")]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("/login")]
        public async Task<IActionResult> ValidateCredentials(string username, string password, string returnUrl)
        {
            var _returnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
            ViewData["ReturnUrl"] = _returnUrl;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Username or password is missing";
                return RedirectToAction("/login");
            }

            if (username.Trim().Equals("sam19", StringComparison.OrdinalIgnoreCase)
                && password.Trim().Equals("abc123", StringComparison.OrdinalIgnoreCase)
                )
            {
                var claims = new List<Claim>();
                claims.Add(new Claim("username", "username"));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, username));
                claims.Add(new Claim(ClaimTypes.Name, "Gaurav Lal"));
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal claimPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimPrincipal);
                return Redirect(_returnUrl);
            }

            TempData["Error"] = "Invalid login: Invalid credentials!";
            return View("login");
        }

        //[HttpPost("/logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
