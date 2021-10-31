using Microsoft.AspNetCore.Mvc;
using System;
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

        public IActionResult Details()
        {
            throw new NotImplementedException();
        }

        public IActionResult Create()
        {
            throw new NotImplementedException();
        }

        public IActionResult Update()
        {
            throw new NotImplementedException();
        }

        [UserRoleFilter]
        public IActionResult Delete()
        {
            throw new NotImplementedException();
        }
    }
}
