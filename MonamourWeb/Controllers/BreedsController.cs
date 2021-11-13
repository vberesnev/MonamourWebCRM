using Microsoft.AspNetCore.Mvc;
using System.Linq;
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
    public class BreedsController : BaseController
    {
        public BreedsController(MonamourDataBaseContext context, ILogService logService)
            : base(context, logService)
        {
        }

        public async Task<IActionResult> All(string sort, string search, int? page, int? pageSize)
        {
            var viewModel = new BreedsAllViewModel();
            viewModel.PageSettings.Sort = sort;
            viewModel.PageSettings.Search = search;
            viewModel.PageSettings.PageSize = pageSize ?? 25;
            viewModel.PageSettings.PageSizes = PageSize.GetSelectListItems(pageSize);

            var breeds = Context.Breeds.Include(x => x.Animal).AsQueryable();
            
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                breeds = breeds.Where(s => s.Title.ToLower().Contains(search)
                                           || s.Animal.Title.ToLower().Contains(search));
            }
            
            viewModel.TotalCount = breeds.Count();
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
            var animals = Context.Animals.ToList();
            var viewModel = new BreedViewModel() 
            {
                Animals = animals
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BreedViewModel breedViewModel)
        {
            if (ModelState.IsValid)
            {
                breedViewModel.Breed.Animal = await Context.Animals.FindAsync(breedViewModel.Breed.AnimalId);
                Context.Breeds.Add(breedViewModel.Breed);
                await Context.SaveChangesAsync();
                await LogService.AddCreationLogAsync<Breed>(breedViewModel.Breed, UserId);
                return RedirectToAction("All");
            }
            breedViewModel.Animals = Context.Animals.ToList();
            return View(breedViewModel);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var breed = Context.Breeds
                .Include(x => x.Animal)
                .FirstOrDefault(x => x.Id == id);
            
            if (breed == null)
                return NotFound();

            var animals = Context.Animals.ToList();
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
        public async Task<IActionResult> Update(BreedViewModel breedViewModel)
        {
            if (ModelState.IsValid)
            {
                var breed = Context.Breeds
                    .Include(x => x.Animal)
                    .FirstOrDefault(x => x.Id == breedViewModel.Breed.Id);
                if (breed == null)
                    return NotFound();
                var oldBreed = breed.Clone() as Breed;

                breed.Title = breedViewModel.Breed.Title;
                breed.AnimalId = breedViewModel.Breed.AnimalId;
                breed.Animal = await Context.Animals.FindAsync(breed.AnimalId);

                await Context.SaveChangesAsync();
                await LogService.AddUpdatedLogAsync<Breed>(oldBreed, breed, UserId);
                return RedirectToAction("All");
            }

            return Update(breedViewModel.Breed.Id);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var user = Context.Breeds
                .Include(x => x.Animal)
                .FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var breed = Context.Breeds
                .Include(x => x.Animal)
                .FirstOrDefault(x => x.Id == id);
            if (breed == null)
                return NotFound();
            Context.Breeds.Remove(breed);
            await Context.SaveChangesAsync();
            await LogService.AddDeletedLogAsync<Breed>(breed, UserId);
            return RedirectToAction("All");
        }
    }
}
