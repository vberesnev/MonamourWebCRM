using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonamourWeb.Models;
using MonamourWeb.Services.Filters;
using MonamourWeb.Services.Pagination;
using MonamourWeb.ViewModels;

namespace MonamourWeb.Controllers
{
    [Authorize]
    public class ClientTagsController : BaseTagController
    {
         private readonly MonamourDataBaseContext _context;

        public ClientTagsController(MonamourDataBaseContext context) : base()
        {
            _context = context;
        }

        public async Task<IActionResult> All()
        {
            var clientTags = _context.ClientTags;
            return View(await clientTags.AsNoTracking().ToListAsync());
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Colors = Colors;
            return View();
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ClientTag clientTag)
        {
            if (ModelState.IsValid)
            {
                _context.ClientTags.Add(clientTag);
                _context.SaveChanges();
                return RedirectToAction("All");
            }

            ViewBag.Colors = Colors;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id, int? page, int? pageSize)
        {
            if (id == null || id == 0)
                return NotFound();

            var clientTag = _context.ClientTags.Find(id);
            
            if (clientTag == null)
                return NotFound();

            var clients = _context.Clients.Where(x => x.Tags.Any(tag => tag.Id == clientTag.Id));
            var viewModel = new ClientTagDetailsViewModel();
            viewModel.PageSettings.PageSize = pageSize ?? 25;
            viewModel.PageSettings.PageSizes = PageSize.GetSelectListItems(pageSize);
            viewModel.ClientTag = clientTag;
            viewModel.PaginatedList = await PaginatedList<Client>.CreateAsync(clients.AsNoTracking(), page ?? 1, pageSize ?? 25);

            return View(viewModel);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var clientTag = _context.ClientTags.Find(id);
            
            if (clientTag == null)
                return NotFound();

            ViewBag.Colors = Colors;
            return View(clientTag);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePost(ClientTag clientTag)
        {
            var editableClientTag = _context.ClientTags.Find(clientTag.Id);
            if (editableClientTag == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                editableClientTag.Title = clientTag.Title;
                editableClientTag.ShortTitle = clientTag.ShortTitle;
                editableClientTag.Color = clientTag.Color;

                _context.SaveChanges();
                return RedirectToAction("All");
            }

            ViewBag.Colors = Colors;
            return RedirectToAction("Update", new {id = clientTag.Id});
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var clientTag = _context.ClientTags.Find(id);

            if (clientTag == null)
                return NotFound();

            return View(clientTag);
        }


        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var clientTag = _context.ClientTags.Find(id);
            if (clientTag == null)
                return NotFound();
            _context.ClientTags.Remove(clientTag);
            _context.SaveChanges();
            return RedirectToAction("All");
        }
    }
}
