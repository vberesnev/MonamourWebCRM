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
    public class PetTagsController : BaseTagController
    {
        private readonly MonamourDataBaseContext _context;

        public PetTagsController(MonamourDataBaseContext context) : base()
        {
            _context = context;
        }

        public async Task<IActionResult> All()
        {
            var petTags = _context.PetTags;
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
        public IActionResult Create(PetTag petTag)
        {
            if (ModelState.IsValid)
            {
                _context.PetTags.Add(petTag);
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

            var petTag = _context.PetTags.Find(id);
            
            if (petTag == null)
                return NotFound();

            var pets = _context.Pets
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

            var petTag = _context.PetTags.Find(id);
            
            if (petTag == null)
                return NotFound();

            ViewBag.Colors = Colors;
            return View(petTag);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePost(PetTag petTag)
        {
            var editedPetTag = _context.PetTags.Find(petTag.Id);
            if (editedPetTag == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                editedPetTag.Title = petTag.Title;
                editedPetTag.ShortTitle = petTag.ShortTitle;
                editedPetTag.Color = petTag.Color;

                _context.SaveChanges();
                return RedirectToAction("All");
            }

            ViewBag.Colors = Colors;
            return RedirectToAction("Update", new {id = petTag.Id});
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var petTag = _context.PetTags.Find(id);

            if (petTag == null)
                return NotFound();

            return View(petTag);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var petTag = _context.PetTags.Find(id);
            if (petTag == null)
                return NotFound();
            _context.PetTags.Remove(petTag);
            _context.SaveChanges();
            return RedirectToAction("All");
        }
    }
}
