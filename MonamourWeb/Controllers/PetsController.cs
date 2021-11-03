using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
    public class PetsController : Controller
    {
        private readonly MonamourDataBaseContext _context;

        public PetsController(MonamourDataBaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> All(string sort, string search, int? tagId, int? page, int? pageSize)
        {
            var viewModel = new PetsAllViewModel();
            viewModel.PageSettings.Sort = sort;
            viewModel.PageSettings.Search = search;
            viewModel.PageSettings.PageSize = pageSize ?? 25;
            viewModel.PageSettings.PageSizes = PageSize.GetSelectListItems(pageSize);
            viewModel.Tags = _context.PetTags;
            viewModel.TagId = tagId;

            var pets = _context.Pets.Include(x => x.Clients)
                                                .Include(x => x.Tags)
                                                .Include(x => x.Breed)
                                                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                pets = pets.Where(s => s.Name.ToLower().Contains(search)
                                                   || s.Breed.Title.Contains(search)
                                                   || s.Clients.Any(x => x.Name.ToLower().Contains(search)));
            }

            if (tagId != null)
            {
                pets = pets.Where(x => x.Tags.Any(petTag => petTag.Id == tagId));
            }

            ViewData["NameSort"] = sort == "name" ? "name_desc" : "name";
            ViewData["BreedSort"] = sort == "breed" ? "breed_desc" : "breed";

            pets = sort switch
            {
                "name" => pets.OrderBy(s => s.Name),
                "name_desc" => pets.OrderByDescending(s => s.Name),
                "breed" => pets.OrderBy(s => s.Breed.Title),
                "breed_desc" => pets.OrderByDescending(s => s.Breed.Title),
                _ => pets.OrderBy(s => s.Id)
            };

            viewModel.PaginatedList = await PaginatedList<Pet>.CreateAsync(pets.AsNoTracking(), page ?? 1, pageSize ?? 25);
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new PetViewModel();
            viewModel.AllTags = _context.PetTags.ToList();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PetViewModel petViewModel, List<int> clientsOfPet, List<int> tagsOfPet)
        {
            if (ModelState.IsValid)
            {
                var clients = _context.Clients.Where(x => clientsOfPet.Contains(x.Id));
                foreach (var client in clients)
                    petViewModel.Pet.Clients.Add(client); 

                var tags = _context.PetTags.Where(x => tagsOfPet.Contains(x.Id));
                foreach (var tag in tags)
                    petViewModel.Pet.Tags.Add(tag);

                petViewModel.Pet.Alive = true;
                
                _context.Pets.Add(petViewModel.Pet);
                await _context.SaveChangesAsync();
                return RedirectToAction("All");
            }
            return View(petViewModel);
        }

        [HttpPost]
        public JsonResult SearchBreed([FromBody] string search)
        {
            IQueryable<Breed> breeds;
            if (string.IsNullOrEmpty(search))
            {
                breeds = _context.Breeds.Take(10);
            }
            else
            {
                search = search.ToLower(); 
                breeds = _context.Breeds
                    .Where(x => x.Title.ToLower().Contains(search))
                    .Take(10); 
            }
            return Json(breeds); 
        }

        [HttpPost]
        public JsonResult SearchClient([FromBody] string search)
        {
            IQueryable<Client> clients;
            if (string.IsNullOrEmpty(search))
                clients = _context.Clients.Take(10);
            else
            {
                search = search.ToLower(); 
                clients = _context.Clients
                    .Where(x => x.Name.ToLower().Contains(search) || x.Phone.Contains(search))
                    .Take(50);
            }

            return Json(clients); 
        }

        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var pet = _context.Pets
                .Include(x => x.Breed)
                .Include(x => x.Clients)
                .Include(x => x.Tags)
                .FirstOrDefault(x => x.Id == id);

            if (pet == null)
                return NotFound();

            return View(pet);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var pet = _context.Pets
                .Include(x => x.Breed)
                .Include(x => x.Clients)
                .Include(x => x.Tags)
                .FirstOrDefault(x => x.Id == id);

            if (pet == null)
                return NotFound();

            return View(pet);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var pet = _context.Pets.Find(id);
            if (pet == null)
                return NotFound();
            _context.Pets.Remove(pet);
            _context.SaveChanges();
            return RedirectToAction("All");
        }

        [HttpGet]
        public IActionResult Update(int? id)
        {
            var viewModel = new PetViewModel();
            viewModel.Pet = _context.Pets.Where(x => x.Id == id)
                .Include(x => x.Breed)
                .Include(x => x.Tags)
                .Include(x => x.Clients)
                .SingleOrDefault();
            viewModel.AllTags = _context.PetTags.ToList();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePost(PetViewModel petViewModel, List<int> clientsOfPet, List<int> tagsOfPet)
        {
            if (ModelState.IsValid)
            {
                var pet = _context.Pets.Where(x => x.Id == petViewModel.Pet.Id)
                    .Include(x => x.Breed)
                    .Include(x => x.Tags)
                    .Include(x => x.Clients)
                    .First();

                var clients = _context.Clients.Where(x => clientsOfPet.Contains(x.Id));
                foreach (var client in clients)
                    petViewModel.Pet.Clients.Add(client); 

                var tags = _context.PetTags.Where(x => tagsOfPet.Contains(x.Id));
                foreach (var tag in tags)
                    petViewModel.Pet.Tags.Add(tag); 

                pet.Update(petViewModel.Pet);

                await _context.SaveChangesAsync();
                return RedirectToAction("All");
            }
            return RedirectToAction("Update", petViewModel.Pet.Id);
        }
    }
}
