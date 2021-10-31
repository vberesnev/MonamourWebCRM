using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MonamourWeb.Models;
using MonamourWeb.Services.Filters;
using MonamourWeb.Services.Pagination;
using MonamourWeb.ViewModels;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Internal;

namespace MonamourWeb.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly MonamourDataBaseContext _context;

        public ClientsController(MonamourDataBaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> All(string sort, string search, int? tagId, int? page, int? pageSize)
        {
            var viewModel = new ClientsAllViewModel();
            viewModel.PageSettings.Sort = sort;
            viewModel.PageSettings.Search = search;
            viewModel.PageSettings.PageSize = pageSize ?? 25;
            viewModel.PageSettings.PageSizes = PageSize.GetSelectListItems(pageSize);
            viewModel.Tags = _context.ClientTags;
            viewModel.TagId = tagId;

            var clients = _context.Clients.Include(x => x.Pets).Include(x => x.Tags).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                clients = clients.Where(s => s.Name.ToLower().Contains(search)
                                                   || s.Phone.Contains(search)
                                                   || s.Pets.Any(x => x.Name.ToLower().Contains(search)));
            }

            if (tagId != null)
            {
                clients = clients.Where(x => x.Tags.Any(clientTag => clientTag.Id == tagId));
            }

            ViewData["NameSort"] = sort == "name" ? "name_desc" : "name";
            ViewData["BonusSort"] = sort == "bonus" ? "bonus_desc" : "bonus";

            clients = sort switch
            {
                "name" => clients.OrderBy(s => s.Name),
                "name_desc" => clients.OrderByDescending(s => s.Name),
                "bonus" => clients.OrderBy(s => s.Bonus),
                "bonus_desc" => clients.OrderByDescending(s => s.Bonus),
                _ => clients.OrderBy(s => s.Id)
            };

            viewModel.PaginatedList = await PaginatedList<Client>.CreateAsync(clients.AsNoTracking(), page ?? 1, pageSize ?? 25);
            return View(viewModel);
        }

        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var client = _context.Clients
                .Include(x => x.Pets)
                .Include(x => x.Tags)
                .FirstOrDefault(x => x.Id == id);

            if (client == null)
                return NotFound();

            return View(client);
        }

        [HttpGet]
        public IActionResult Update(int? id)
        {
            var viewModel = new ClientViewModel();
            viewModel.Client = _context.Clients.Where(x => x.Id == id)
                                                .Include(x => x.Tags)
                                                .Include(x => x.Pets)
                                                .ThenInclude(x => x.Breed)
                                                .SingleOrDefault();
            viewModel.AllTags = _context.ClientTags.ToList();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePost(ClientViewModel clientViewModel, List<int> clientPets, List<int> clientTags)
        {
            if (ModelState.IsValid)
            {
                var client = _context.Clients.Where(x => x.Id == clientViewModel.Client.Id)
                    .Include(x => x.Tags)
                    .Include(x => x.Pets)
                    .First();

                var pets = _context.Pets.Where(x => clientPets.Contains(x.Id));
                foreach (var pet in pets)
                    clientViewModel.Client.Pets.Add(pet); 

                var tags = _context.ClientTags.Where(x => clientTags.Contains(x.Id));
                foreach (var tag in tags)
                    clientViewModel.Client.Tags.Add(tag); 

                client.Update(clientViewModel.Client);

                await _context.SaveChangesAsync();
                return RedirectToAction("All");
            }
            return RedirectToAction("Update", clientViewModel.Client.Id);
        }

        [HttpPost]
        public JsonResult SearchPet([FromBody] string search)
        {
            if (string.IsNullOrEmpty(search))
                return Json(null);

            search = search.ToLower();
            var pets = _context.Pets.Include(x => x.Breed)
                                                .Where(x => x.Name.ToLower().Contains(search) || x.Breed.Title.ToLower().Contains(search))
                                                .Where(x => x.Alive);
            
            return Json(pets); 
        }
        
        [UserRoleFilter]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var client = _context.Clients
                .Include(x => x.Pets)
                .Include(x => x.Tags)
                .FirstOrDefault(x => x.Id == id);

            if (client == null)
                return NotFound();

            return View(client);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var client = _context.Clients.Find(id);
            if (client == null)
                return NotFound();
            _context.Clients.Remove(client);
            _context.SaveChanges();
            return RedirectToAction("All");
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new ClientViewModel();
            viewModel.AllTags = _context.ClientTags.ToList();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientViewModel clientViewModel, List<int> clientPets, List<int> clientTags)
        {
            if (ModelState.IsValid)
            {
                var pets = _context.Pets.Where(x => clientPets.Contains(x.Id));
                foreach (var pet in pets)
                    clientViewModel.Client.Pets.Add(pet); 

                var tags = _context.ClientTags.Where(x => clientTags.Contains(x.Id));
                foreach (var tag in tags)
                    clientViewModel.Client.Tags.Add(tag);
                
                _context.Clients.Add(clientViewModel.Client);
                await _context.SaveChangesAsync();
                return RedirectToAction("All");
            }
            return View(clientViewModel);
        }
    }
}
