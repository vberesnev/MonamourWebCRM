using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonamourWeb.Models;
using MonamourWeb.Services.Filters;
using MonamourWeb.Services.Logs;
using MonamourWeb.Services.Pagination;
using MonamourWeb.ViewModels;

namespace MonamourWeb.Controllers
{
    [Authorize]
    public class ClientTagsController : BaseTagController
    {
        public ClientTagsController(MonamourDataBaseContext context, ILogService logService) 
            : base(context, logService)
        {
        }

        public async Task<IActionResult> All()
        {
            var clientTags = Context.ClientTags;
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
        public async Task<IActionResult> Create(ClientTag clientTag)
        {
            if (ModelState.IsValid)
            {
                Context.ClientTags.Add(clientTag);
                await Context.SaveChangesAsync();
                await LogService.AddCreationLogAsync<ClientTag>(clientTag, UserId);
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

            var clientTag = Context.ClientTags.Find(id);
            
            if (clientTag == null)
                return NotFound();

            var clients = Context.Clients
                                        .Include(x => x.Pets)
                                        .Where(x => x.Tags.Any(tag => tag.Id == clientTag.Id));
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

            var clientTag = Context.ClientTags.Find(id);
            
            if (clientTag == null)
                return NotFound();

            ViewBag.Colors = Colors;
            return View(clientTag);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ClientTag clientTag)
        {
            if (ModelState.IsValid)
            {
                var editableClientTag = await Context.ClientTags.FindAsync(clientTag.Id);
                if (editableClientTag == null)
                    return NotFound();

                var oldClientTag = editableClientTag.Clone() as ClientTag;

                editableClientTag.Title = clientTag.Title;
                editableClientTag.ShortTitle = clientTag.ShortTitle;
                editableClientTag.Color = clientTag.Color;

                await Context.SaveChangesAsync();
                await LogService.AddUpdatedLogAsync<ClientTag>(oldClientTag, editableClientTag, UserId);
                return RedirectToAction("All");
            }

            ViewBag.Colors = Colors;
            return Update(clientTag.Id);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var clientTag = Context.ClientTags.Find(id);

            if (clientTag == null)
                return NotFound();

            return View(clientTag);
        }


        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var clientTag = await Context.ClientTags.FindAsync(id);
            if (clientTag == null)
                return NotFound();
            Context.ClientTags.Remove(clientTag);
            await Context.SaveChangesAsync();
            await LogService.AddDeletedLogAsync<ClientTag>(clientTag, UserId);
            return RedirectToAction("All");
        }
    }
}
