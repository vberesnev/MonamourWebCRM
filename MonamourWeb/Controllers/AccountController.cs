using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MonamourWeb.Models;
using MonamourWeb.Services.Encoding;
using MonamourWeb.ViewModels;

namespace MonamourWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly MonamourDataBaseContext _context;
        private readonly IEncodingService _encodingService;

        public AccountController(MonamourDataBaseContext context, IEncodingService encodingService)
        {
            _context = context;
            _encodingService = encodingService;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            var logins = await _context.Users.Select(x => x.Name).AsNoTracking().ToListAsync();

            var loginViewModel = new LoginViewModel()
            {
                Logins = logins
            };
            return View(loginViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            ViewData["EmptyLogin"] = string.Empty;
            ViewData["WrongPassword"] = string.Empty;

            if (loginViewModel.User.Name == null)
            {
                ViewData["EmptyLogin"] = "Логин должен быть выбран";
                var logins = await _context.Users.Select(x => x.Name).AsNoTracking().ToListAsync();
                loginViewModel.Logins = logins;

                return View(loginViewModel);
            }

            var user = await _context.Users.Where(x => x.Name == loginViewModel.User.Name
                                                 && x.Password == _encodingService.GetHashCode(loginViewModel.User.Password)
                                                 && x.Blocked == false)
                .Include(x => x.Role)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                ViewData["WrongPassword"] = "Неверный пароль";
                var logins = await _context.Users.Select(x => x.Name).AsNoTracking().ToListAsync();
                loginViewModel.Logins = logins;
                
                return View(loginViewModel);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.Title)
            };

            var claimsIdentity=new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = false,
                //IsPersistent = false,
                //ExpiresUtc = DateTime.UtcNow.AddMinutes(3),

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Visits");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
