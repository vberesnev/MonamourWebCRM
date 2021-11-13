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
    public class ClientsController : BaseController
    {
        public ClientsController(MonamourDataBaseContext context, ILogService logService)
            : base(context, logService)
        {
        }

        public async Task<IActionResult> All(string sort, string search, int? tagId, int? page, int? pageSize)
        {
            var viewModel = new ClientsAllViewModel();
            viewModel.PageSettings.Sort = sort;
            viewModel.PageSettings.Search = search;
            viewModel.PageSettings.PageSize = pageSize ?? 25;
            viewModel.PageSettings.PageSizes = PageSize.GetSelectListItems(pageSize);
            viewModel.Tags = Context.ClientTags;
            viewModel.TagId = tagId;

            var clients = Context.Clients
                .Include(x => x.Pets)
                .Include(x => x.Tags)
                .AsQueryable();

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

            viewModel.TotalCount = clients.Count();

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

            var client = Context.Clients
                .Include(x => x.Pets)
                .ThenInclude(x => x.Breed)
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
            viewModel.Client = Context.Clients
                .Include(x => x.Tags)
                .Include(x => x.Pets)
                .ThenInclude(x => x.Breed)
                .FirstOrDefault(x => x.Id == id);

            if (viewModel.Client is null)
                return NotFound();

            viewModel.AllTags = Context.ClientTags.ToList();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ClientViewModel clientViewModel, List<int> clientPets, List<int> clientTags)
        {
            if (ModelState.IsValid)
            {
                var client = Context.Clients
                    .Include(x => x.Tags)
                    .Include(x => x.Pets)
                    .ThenInclude(x => x.Breed)
                    .FirstOrDefault(x => x.Id == clientViewModel.Client.Id);

                if (client is null)
                    return NotFound();

                var oldClient = client.Clone() as Client;

                var pets = Context.Pets
                    .Include(x => x.Breed)
                    .Where(x => clientPets.Contains(x.Id));
                foreach (var pet in pets)
                    clientViewModel.Client.Pets.Add(pet); 

                var tags = Context.ClientTags.Where(x => clientTags.Contains(x.Id));
                foreach (var tag in tags)
                    clientViewModel.Client.Tags.Add(tag);

                client.Update(clientViewModel.Client);

                await Context.SaveChangesAsync();
                await LogService.AddUpdatedLogAsync<Client>(oldClient, client, UserId);
                return RedirectToAction("All");
            }
            return Update(clientViewModel.Client.Id);
        }

        [HttpPost]
        public JsonResult SearchPet([FromBody] string search)
        {
            IQueryable<Pet> pets;
            if (string.IsNullOrEmpty(search))
                pets = Context.Pets.Take(10);
            else
            {
                search = search.ToLower();
                pets = Context.Pets.Include(x => x.Breed)
                    .Where(x => x.Name.ToLower().Contains(search) || x.Breed.Title.ToLower().Contains(search))
                    .Where(x => x.Alive);
            }
            return Json(pets); 
        }

        [HttpPost]
        public JsonResult CheckPhone([FromBody] string phoneNumber)
        {
            var phones = phoneNumber.Split(" ");

            var clients = new List<Client>();

            foreach (var phone in phones)
            {
                if (string.IsNullOrEmpty(phone))
                    continue;
                clients.AddRange(Context.Clients.Where(x => x.Phone.Contains(phone)));
            }
            return Json(clients);
        }
        
        [UserRoleFilter]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var client = Context.Clients
                .Include(x => x.Pets)
                .ThenInclude(x => x.Breed)
                .Include(x => x.Tags)
                .FirstOrDefault(x => x.Id == id);

            if (client == null)
                return NotFound();

            return View(client);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var client = Context.Clients
                .Include(x => x.Pets)
                .ThenInclude(x => x.Breed)
                .Include(x => x.Tags)
                .FirstOrDefault(x => x.Id == id);
            if (client == null)
                return NotFound();
            Context.Clients.Remove(client);
            await Context.SaveChangesAsync();
            await LogService.AddDeletedLogAsync<Client>(client, UserId);
            return RedirectToAction("All");
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new ClientViewModel();
            viewModel.AllTags = Context.ClientTags.ToList();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientViewModel clientViewModel, List<int> clientPets, List<int> clientTags)
        {
            if (ModelState.IsValid)
            {
                var pets = Context.Pets
                    .Include(x => x.Breed)
                    .Where(x => clientPets.Contains(x.Id));
                foreach (var pet in pets)
                    clientViewModel.Client.Pets.Add(pet); 

                var tags = Context.ClientTags.Where(x => clientTags.Contains(x.Id));
                foreach (var tag in tags)
                    clientViewModel.Client.Tags.Add(tag);
                clientViewModel.Client.Name = clientViewModel.Client.Name?.Trim();

                Context.Clients.Add(clientViewModel.Client);
                await Context.SaveChangesAsync();
                await LogService.AddCreationLogAsync<Client>(clientViewModel.Client, UserId);
                return RedirectToAction("All");
            }
            return View(clientViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> CreateQuick(string name, string phone)
        {
            var client = new Client()
            {
                Name = name,
                Phone = phone
            };

            Context.Clients.Add(client);
            await Context.SaveChangesAsync();
            await LogService.AddCreationLogAsync<Client>(client, UserId);
            return Json(true);
        }
    }
}
