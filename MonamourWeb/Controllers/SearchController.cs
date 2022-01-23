using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MonamourWeb.Models;
using MonamourWeb.Services.Logs;
using MonamourWeb.ViewModels;
using Newtonsoft.Json;

namespace MonamourWeb.Controllers
{
    public class SearchController : BaseController
    {
        public SearchController(MonamourDataBaseContext context, ILogService logService) : base(context, logService)
        {
        }

        [HttpPost]
        public JsonResult Breed([FromBody] string search)
        {
            IQueryable<Breed> breeds;
            if (string.IsNullOrEmpty(search))
            {
                breeds = Context.Breeds.Take(10);
            }
            else
            {
                search = search.ToLower(); 
                breeds = Context.Breeds
                    .Where(x => x.Title.ToLower().Contains(search))
                    .Take(10); 
            }
            return Json(breeds); 
        }

        [HttpPost]
        public JsonResult Client([FromBody] string search)
        {
            IQueryable<Client> clients;
            if (string.IsNullOrEmpty(search))
                clients = Context.Clients.Take(10);
            else
            {
                search = search.ToLower(); 
                clients = Context.Clients
                    .Where(x => x.Name.ToLower().Contains(search) || x.Phone.Contains(search))
                    .Take(50);
            }

            return Json(clients); 
        }

        [HttpPost]
        public JsonResult ClientById([FromBody] int id)
        {
            var client = Context.Clients
                .Include(x => x.Tags)
                .FirstOrDefault(x => x.Id == id);

            return Json(client);
        }

        [HttpPost]
        public JsonResult PetById([FromBody] int id)
        {
            var pet = Context.Pets
                .Include(x => x.Breed)
                .Include(x => x.Clients)
                .Include(x => x.Tags)
                .FirstOrDefault(x => x.Id == id);

            return Json(pet);
        }

        [HttpPost]
        public JsonResult Pet([FromBody] string search)
        {
            IQueryable<Pet> pets;
            if (string.IsNullOrEmpty(search))
                pets = Context.Pets
                    .Include(x => x.Breed)
                    .Where(x => x.Alive)
                    .Take(20);
            else
            {
                search = search.ToLower();
                pets = Context.Pets
                    .Include(x => x.Breed)
                    .Where(x => x.Name.ToLower().Contains(search) || x.Breed.Title.ToLower().Contains(search))
                    .Where(x => x.Alive)
                    .Take(20);
            }
            return Json(pets); 
        }

        [HttpPost]
        public JsonResult PetFull([FromBody] string search)
        {

            IQueryable<Pet> pets;
            if (string.IsNullOrEmpty(search))
                pets = Context.Pets
                        .Include(x => x.Breed)
                        .Include(x => x.Clients)
                        .Include(x => x.Tags)
                        .Where(x => x.Alive)
                        .Take(20);
                   
            else
            {
                search = search.ToLower();
                pets = Context.Pets
                    .Include(x => x.Breed)
                    .Include(x => x.Clients)
                    .Include(x => x.Tags)
                    .Where(x => x.Name.ToLower().Contains(search) || x.Breed.Title.ToLower().Contains(search))
                    .Where(x => x.Alive)
                    .Take(20);
            }

            return Json(pets.ToArray());
        }

        [HttpPost]
        public JsonResult Phone([FromBody] string phoneNumber)
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

        [HttpPost]
        public JsonResult Procedure([FromBody] string requestString)
        {
            var request = JsonConvert.DeserializeObject<ProceduresForVisitCard>(requestString);
            if (request is null)
                return null;

            var breedId = request.BreedId;
            var search = request.Search;
            
             var animalId = Context.Breeds.Find(breedId).AnimalId;
            
             IQueryable<Procedure> procedures;
             if (string.IsNullOrEmpty(search))
                 procedures = Context.Procedures
                     .Where(x => x.AnimalId == animalId)
                     .Take(15);
             else
             {
                 search = search.ToLower(); 
                 procedures = Context.Procedures
                     .Where(x => x.Title.ToLower().Contains(search) && x.AnimalId == animalId)
                     .Take(15);
             }

            return Json(procedures); 
        }
    }
}
