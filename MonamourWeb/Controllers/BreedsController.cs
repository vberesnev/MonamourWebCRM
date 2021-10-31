using Microsoft.AspNetCore.Mvc;
using System.Linq;
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
    public class BreedsController : Controller
    {
        private readonly MonamourDataBaseContext _context;

        public BreedsController(MonamourDataBaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> All(string sort, string search, int? page, int? pageSize)
        {
            var viewModel = new BreedsAllViewModel();
            viewModel.PageSettings.Sort = sort;
            viewModel.PageSettings.Search = search;
            viewModel.PageSettings.PageSize = pageSize ?? 25;
            viewModel.PageSettings.PageSizes = PageSize.GetSelectListItems(pageSize);

            var breeds = _context.Breeds.Include(x => x.Animal).AsQueryable();
            
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                breeds = breeds.Where(s => s.Title.ToLower().Contains(search)
                                           || s.Animal.Title.ToLower().Contains(search));
            }
            
            ViewData["IdSort"] = sort == "id" ? "id_desc" : "id";
            ViewData["TitleSort"] = sort == "title" ? "title_desc" : "title";
            ViewData["AnimalSort"] = sort == "animal" ? "animal_desc" : "animal";

            breeds = sort switch
            {
                "id" => breeds.OrderBy(s => s.Id),
                "id_desc" => breeds.OrderByDescending(s => s.Id),
                "title" => breeds.OrderBy(s => s.Title),
                "title_desc" => breeds.OrderByDescending(s => s.Title),
                "animal" => breeds.OrderBy(s => s.Animal.Title),
                "animal_desc" => breeds.OrderByDescending(s => s.Animal.Title),
                _ => breeds.OrderBy(s => s.Id)
            };

            viewModel.PaginatedList = await PaginatedList<Breed>.CreateAsync(breeds.AsNoTracking(), page ?? 1, pageSize ?? 25);
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var animals = _context.Animals.ToList();
            var viewModel = new BreedViewModel() 
            {
                Animals = animals
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BreedViewModel breedViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Breeds.Add(breedViewModel.Breed);
                _context.SaveChanges();
                return RedirectToAction("All");
            }
            breedViewModel.Animals = _context.Animals.ToList();
            return View(breedViewModel);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var breed = _context.Breeds.Include(x => x.Animal).FirstOrDefault(x => x.Id == id);
            
            if (breed == null)
                return NotFound();

            var animals = _context.Animals.ToList();
            var breedViewModel = new BreedViewModel()
            {
                Breed = breed,
                Animals = animals
            };
            return View(breedViewModel);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePost(BreedViewModel breedViewModel)
        {
            var breed = _context.Breeds.Find(breedViewModel.Breed.Id);
            if (breed == null)
                return NotFound();

            breed.Title = breedViewModel.Breed.Title;
            breed.AnimalId = breedViewModel.Breed.AnimalId;

            _context.SaveChanges();
            return RedirectToAction("All");
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var user = _context.Breeds.Include(x => x.Animal).FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var breed = _context.Breeds.Find(id);
            if (breed == null)
                return NotFound();
            _context.Breeds.Remove(breed);
            _context.SaveChanges();
            return RedirectToAction("All");
        }
    }
}
