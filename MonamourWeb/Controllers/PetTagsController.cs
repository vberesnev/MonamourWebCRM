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
    public class PetTagsController : BaseTagController
    {
        public PetTagsController(MonamourDataBaseContext context, ILogService logService) 
            : base(context, logService)
        {
        }

        public async Task<IActionResult> All()
        {
            var petTags = Context.PetTags;
            return View(await petTags.AsNoTracking().ToListAsync());
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
        public async Task<IActionResult> Create(PetTag petTag)
        {
            if (ModelState.IsValid)
            {
                Context.PetTags.Add(petTag);
                await Context.SaveChangesAsync();
                await LogService.AddCreationLogAsync<PetTag>(petTag, UserId);
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

            var petTag = await Context.PetTags.FindAsync(id);
            
            if (petTag == null)
                return NotFound();

            var pets = Context.Pets
                                        .Include(x => x.Clients)
                                        .Include(x => x.Breed)
                                        .Where(x => x.Tags.Any(tag => tag.Id == petTag.Id));
            var viewModel = new PetTagDetailsViewModel();
            viewModel.PageSettings.PageSize = pageSize ?? 25;
            viewModel.PageSettings.PageSizes = PageSize.GetSelectListItems(pageSize);
            viewModel.PetTag = petTag;
            viewModel.PaginatedList = await PaginatedList<Pet>.CreateAsync(pets.AsNoTracking(), page ?? 1, pageSize ?? 25);

            return View(viewModel);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var petTag = Context.PetTags.Find(id);
            
            if (petTag == null)
                return NotFound();

            ViewBag.Colors = Colors;
            return View(petTag);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PetTag petTag)
        {
            if (ModelState.IsValid)
            {
                var editedPetTag = await Context.PetTags.FindAsync(petTag.Id);
                if (editedPetTag == null)
                    return NotFound();
                var oldPetTag = editedPetTag.Clone() as PetTag;

                editedPetTag.Title = petTag.Title;
                editedPetTag.ShortTitle = petTag.ShortTitle;
                editedPetTag.Color = petTag.Color;

                await Context.SaveChangesAsync();
                await LogService.AddUpdatedLogAsync<PetTag>(oldPetTag, editedPetTag, UserId);
                return RedirectToAction("All");
            }

            ViewBag.Colors = Colors;
            return Update(petTag.Id);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var petTag = Context.PetTags.Find(id);

            if (petTag == null)
                return NotFound();

            return View(petTag);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var petTag = await Context.PetTags.FindAsync(id);
            if (petTag == null)
                return NotFound();
            Context.PetTags.Remove(petTag);
            await Context.SaveChangesAsync();
            await LogService.AddDeletedLogAsync<PetTag>(petTag, UserId);
            return RedirectToAction("All");
        }
    }
}
