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
using MonamourWeb.Services.Logs;
using MonamourWeb.ViewModels;

namespace MonamourWeb.Controllers
{
    public class AccountController : BaseController
    {
        
        private readonly IEncodingService _encodingService;

        public AccountController(MonamourDataBaseContext context, ILogService logService, IEncodingService encodingService) 
            : base(context, logService)
        {
            _encodingService = encodingService;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            var logins = await Context.Users
                .Select(x => x.Name)
                .AsNoTracking()
                .ToListAsync();

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
                var logins = await Context.Users.Select(x => x.Name).AsNoTracking().ToListAsync();
                loginViewModel.Logins = logins;

                return View(loginViewModel);
            }

            var user = await Context.Users.Where(x => x.Name == loginViewModel.User.Name
                                                 && x.Password == _encodingService.GetHashCode(loginViewModel.User.Password)
                                                 && x.Blocked == false)
                                            .Include(x => x.Role)
                                            .FirstOrDefaultAsync();

            if (user == null)
            {
                ViewData["WrongPassword"] = "Неверный пароль";
                var logins = await Context.Users.Select(x => x.Name).AsNoTracking().ToListAsync();
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

        [HttpGet]
        public IActionResult MyAccount()
        {
            var user = Context.Users
                .Include(x => x.Role)
                .FirstOrDefault(x => x.Id == UserId);

            if (user is null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(int id, string oldPass, string newPass)
        {
            var user = await Context.Users.FindAsync(id);

            if (user.Password == _encodingService.GetHashCode(oldPass))
            {
                user.Password = _encodingService.GetHashCode(newPass);
                await Context.SaveChangesAsync();
                await LogService.AddChangePasswordLog(user.Name, UserId);
                return Json(new {success  = true} );
            }
            return Json(new {success  = false, message = "Старый пароль неверный"});
        }
    }
}
