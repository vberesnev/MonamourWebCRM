using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonamourWeb.Models;
using MonamourWeb.Services.Encoding;
using MonamourWeb.Services.Filters;
using MonamourWeb.Services.Logs;
using MonamourWeb.ViewModels;

namespace MonamourWeb.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IEncodingService _encodingService;
        public UsersController(MonamourDataBaseContext context, ILogService logService, IEncodingService encodingService)
            : base(context, logService)
        {
            _encodingService = encodingService;
        }

        [UserRoleFilter]
        public async Task<IActionResult> All(string sort, string search)
        {
            ViewData["IdSort"] = sort == "id" ? "id_desc" : "id";
            ViewData["NameSort"] = sort == "name" ? "name_desc" : "name";
            ViewData["RoleSort"] = sort == "role" ? "role_desc" : "role";
            ViewData["BlockedSort"] = sort == "block" ? "block_desc" : "block";

            ViewData["CurrentSearch"] = search;
            
            var users = Context.Users.Include(x => x.Role).AsQueryable();
            
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                users = users.Where(s => s.Name.ToLower().Contains(search)
                                         || s.Role.Title.ToLower().Contains(search));
            }
            
            users = sort switch
            {
                "id" => users.OrderBy(s => s.Id),
                "id_desc" => users.OrderByDescending(s => s.Id),
                "name" => users.OrderBy(s => s.Name),
                "name_desc" => users.OrderByDescending(s => s.Name),
                "role" => users.OrderBy(s => s.Role.Title),
                "role_desc" => users.OrderByDescending(s => s.Role.Title),
                "block" => users.OrderBy(s => s.Blocked),
                "block_desc" => users.OrderByDescending(s => s.Blocked),
                _ => users.OrderBy(s => s.Id)
            };

            return View(await users.AsNoTracking().ToListAsync());
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Create()
        {
            var roles = Context.UserRoles.ToList();
            var viewModel = new UserViewModel() 
            {
                Roles = roles
            };
            return View(viewModel);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                userViewModel.User.Password = _encodingService.GetHashCode(userViewModel.User.Password);
                userViewModel.User.Role = Context.UserRoles.Find(userViewModel.User.RoleId);
                Context.Users.Add(userViewModel.User);
                await Context.SaveChangesAsync();
                await LogService.AddCreationLogAsync<User>(userViewModel.User, UserId);
                return RedirectToAction("All");
            }
            userViewModel.Roles = Context.UserRoles.ToList();
            return View(userViewModel);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var user = Context.Users
                .Include(x => x.Role)
                .FirstOrDefault(x => x.Id == id);
            
            if (user == null)
                return NotFound();

            var roles = Context.UserRoles.ToList();
            var userViewModel = new UserViewModel()
            {
                User = user,
                Roles = roles
            };
            return View(userViewModel);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = Context.Users
                    .Include(x => x.Role)
                    .FirstOrDefault(x => x.Id == userViewModel.User.Id);
                if (user == null)
                    return NotFound();

                var oldUser = user.Clone() as User;

                user.Name = userViewModel.User.Name;
                user.RoleId = userViewModel.User.RoleId;
                user.Role = await Context.UserRoles.FindAsync(user.RoleId);

                if (!string.IsNullOrEmpty(userViewModel.User.Password))
                    user.Password = _encodingService.GetHashCode(userViewModel.User.Password);

                await Context.SaveChangesAsync();
                await LogService.AddUpdatedLogAsync<User>(oldUser,  user, UserId);
                return RedirectToAction("All");
            }

            return Update(userViewModel.User.Id);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Block(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var user = Context.Users
                .Include(x => x.Role)
                .FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound();
            return View(user);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockPost(int? id)
        {
            var user = await Context.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            
            user.Blocked = true;
            await Context.SaveChangesAsync();
            await LogService.AddBlockUserLogAsync(user.Name, UserId);
            return RedirectToAction("All");
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Unblock(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var user = Context.Users
                .Include(x => x.Role)
                .FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnblockPost(int? id)
        {
            var user = await Context.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            user.Blocked = false;
            await Context.SaveChangesAsync();
            await LogService.AddUnblockUserLogAsync(user.Name, UserId);
            return RedirectToAction("All");
        }
    }
}
