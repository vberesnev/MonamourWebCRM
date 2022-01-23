using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonamourWeb.Models;
using MonamourWeb.Services.Logs;

namespace MonamourWeb.Controllers
{
    public class ComplexController : BaseController
    {
        public ComplexController(MonamourDataBaseContext context, ILogService logService) : base(context, logService)
        {
        }


        [HttpPost]
        public async Task<JsonResult> CreatePetWithExistedClient(string name, int breedId, int clientId)
        {
            var pet = new Pet()
            {
                Name = name,
                BreedId = breedId,
                Alive = true
            };

            pet.Breed = await Context.Breeds.FindAsync(breedId);
            pet.Clients.Add(await Context.Clients.FindAsync(clientId));

            Context.Pets.Add(pet);
            await Context.SaveChangesAsync();
            await LogService.AddCreationLogAsync<Pet>(pet, UserId);
            return Json(pet);
        }

        [HttpPost]
        public async Task<JsonResult> CreatePetWithClient(string name, int breedId, string clientName, string clientPhone)
        {
            var client = new Client()
            {
                Name = clientName,
                Phone = clientPhone
            };
            
            var pet = new Pet()
            {
                Name = name,
                BreedId = breedId,
                Alive = true
            };

            pet.Clients.Add(client);
            client.Pets.Add(pet);

            Context.Pets.Add(pet);
            Context.Clients.Add(client);
            await Context.SaveChangesAsync();
            await LogService.AddCreationLogAsync<Pet>(pet, UserId);
            await LogService.AddCreationLogAsync<Client>(client, UserId);
            return Json(pet);
        }

        
    }
}
