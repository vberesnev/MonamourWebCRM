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
    public class UsersController : Controller
    {
        private readonly MonamourDataBaseContext _context;
        private readonly IEncodingService _encodingService;
        private readonly ILogService _logService;

        public UsersController(MonamourDataBaseContext context, IEncodingService encodingService, ILogService logService)
        {
            _context = context;
            _encodingService = encodingService;
            _logService = logService;
        }

        [UserRoleFilter]
        public async Task<IActionResult> All(string sort, string search)
        {
            ViewData["IdSort"] = sort == "id" ? "id_desc" : "id";
            ViewData["NameSort"] = sort == "name" ? "name_desc" : "name";
            ViewData["RoleSort"] = sort == "role" ? "role_desc" : "role";
            ViewData["BlockedSort"] = sort == "block" ? "block_desc" : "block";

            ViewData["CurrentSearch"] = search;
            
            var users = _context.Users.Include(x => x.Role).AsQueryable();
            
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
            var roles = _context.UserRoles.ToList();
            var viewModel = new UserViewModel() 
            {
                Roles = roles
            };
            return View(viewModel);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                userViewModel.User.Password = _encodingService.GetHashCode(userViewModel.User.Password);
                _context.Users.Add(userViewModel.User);
                _context.SaveChanges();
                _logService.AddCreationLogAsync<User>(userViewModel.User,
                                            Convert.ToInt32(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));
                return RedirectToAction("All");
            }
            userViewModel.Roles = _context.UserRoles.ToList();
            return View(userViewModel);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var user = _context.Users.Include(x => x.Role).FirstOrDefault(x => x.Id == id);
            
            if (user == null)
                return NotFound();

            var roles = _context.UserRoles.ToList();
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
        public IActionResult UpdatePost(UserViewModel userViewModel)
        {
            var user = _context.Users.Find(userViewModel.User.Id);
            if (user == null)
                return NotFound();

            user.Name = userViewModel.User.Name;
            user.RoleId = userViewModel.User.RoleId;

            if (!string.IsNullOrEmpty(userViewModel.User.Password))
                user.Password = _encodingService.GetHashCode(userViewModel.User.Password);

            _context.SaveChanges();
            return RedirectToAction("All");
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Block(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var user = _context.Users.Include(x => x.Role).FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BlockPost(int? id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();
            user.Blocked = true;
            _context.SaveChanges();
            return RedirectToAction("All");
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Unblock(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var user = _context.Users.Include(x => x.Role).FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UnblockPost(int? id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();
            user.Blocked = false;
            _context.SaveChanges();
            return RedirectToAction("All");
        }
    }
}
