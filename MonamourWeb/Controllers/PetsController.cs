using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
    public class PetsController : BaseController
    {
        public PetsController(MonamourDataBaseContext context, ILogService logService)
            : base(context, logService)
        {
        }

        public async Task<IActionResult> All(string sort, string search, int? tagId, int? page, int? pageSize)
        {
            var viewModel = new PetsAllViewModel();
            viewModel.PageSettings.Sort = sort;
            viewModel.PageSettings.Search = search;
            viewModel.PageSettings.PageSize = pageSize ?? 25;
            viewModel.PageSettings.PageSizes = PageSize.GetSelectListItems(pageSize);
            viewModel.Tags = Context.PetTags;
            viewModel.TagId = tagId;

            var pets = Context.Pets.Include(x => x.Clients)
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

            viewModel.TotalCount = pets.Count();

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
            viewModel.AllTags = Context.PetTags.ToList();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PetViewModel petViewModel, List<int> clientsOfPet, List<int> tagsOfPet)
        {
            if (ModelState.IsValid)
            {
                var clients = Context.Clients.Where(x => clientsOfPet.Contains(x.Id));
                foreach (var client in clients)
                    petViewModel.Pet.Clients.Add(client); 

                var tags = Context.PetTags.Where(x => tagsOfPet.Contains(x.Id));
                foreach (var tag in tags)
                    petViewModel.Pet.Tags.Add(tag);

                petViewModel.Pet.Alive = true;
                petViewModel.Pet.Breed = await Context.Breeds.FindAsync(petViewModel.Pet.BreedId);
                
                Context.Pets.Add(petViewModel.Pet);
                await Context.SaveChangesAsync();
                await LogService.AddCreationLogAsync<Pet>(petViewModel.Pet, UserId);
                return RedirectToAction("All");
            }
            return View(petViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> CreateQuick(string name, int id, string day)
        {
            var pet = new Pet()
            {
                Name = name,
                BreedId = id,
                Bday = day,
                Alive = true
            };

            pet.Breed = await Context.Breeds.FindAsync(id);
                
            Context.Pets.Add(pet);
            await Context.SaveChangesAsync();
            await LogService.AddCreationLogAsync<Pet>(pet, UserId);
            return Json(true);
        }

        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var pet = Context.Pets
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

            var pet = Context.Pets
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
        public async Task<IActionResult> DeletePost(int? id)
        {
            var pet = Context.Pets
                .Include(x => x.Breed)
                .Include(x => x.Clients)
                .Include(x => x.Tags)
                .FirstOrDefault(x => x.Id == id);
            if (pet == null)
                return NotFound();
            Context.Pets.Remove(pet);
            await Context.SaveChangesAsync();
            await LogService.AddDeletedLogAsync(pet, UserId);
            return RedirectToAction("All");
        }

        [HttpGet]
        public IActionResult Update(int? id)
        {
            var viewModel = new PetViewModel();
            viewModel.Pet = Context.Pets
                .Include(x => x.Breed)
                .Include(x => x.Tags)
                .Include(x => x.Clients)
                .FirstOrDefault(x => x.Id == id);

            if (viewModel.Pet is null)
                return NotFound();

            viewModel.AllTags = Context.PetTags.ToList();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PetViewModel petViewModel, List<int> clientsOfPet, List<int> tagsOfPet)
        {
            if (ModelState.IsValid)
            {
                var pet = Context.Pets
                    .Include(x => x.Breed)
                    .Include(x => x.Tags)
                    .Include(x => x.Clients)
                    .FirstOrDefault(x => x.Id == petViewModel.Pet.Id);

                if (pet is null)
                    return NotFound();

                var oldPet = pet.Clone() as Pet;
                
                var clients = Context.Clients.Where(x => clientsOfPet.Contains(x.Id));
                foreach (var client in clients)
                    petViewModel.Pet.Clients.Add(client); 

                var tags = Context.PetTags.Where(x => tagsOfPet.Contains(x.Id));
                foreach (var tag in tags)
                    petViewModel.Pet.Tags.Add(tag); 

                pet.Update(petViewModel.Pet);
                pet.Breed = await Context.Breeds.FindAsync(pet.BreedId);

                await Context.SaveChangesAsync();
                await LogService.AddUpdatedLogAsync(oldPet, pet, UserId);
                return RedirectToAction("All");
            }
            return Update(petViewModel.Pet.Id);
        }
    }
}
