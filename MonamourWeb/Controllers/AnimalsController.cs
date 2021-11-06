using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonamourWeb.Models;
using MonamourWeb.Services.Filters;
using MonamourWeb.Services.Logs;

namespace MonamourWeb.Controllers
{
    [Authorize]
    public class AnimalsController : BaseController
    {
        public AnimalsController(MonamourDataBaseContext context, ILogService logService)
            : base(context, logService)
        {
            
        }

        public IActionResult All()
        {
            var users = Context.Animals;
            return View(users);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Animal animal)
        {
            if (ModelState.IsValid)
            {
                Context.Animals.Add(animal);
                await Context.SaveChangesAsync();
                await LogService.AddCreationLogAsync<Animal>(animal, UserId);
                return RedirectToAction("All");
            }
            return View();
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var animal = Context.Animals.Find(id);
            
            if (animal == null)
                return NotFound();

            return View(animal);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Animal animal)
        {
            if (ModelState.IsValid)
            {
                var editedAnimal = await Context.Animals.FindAsync(animal.Id);
                if (editedAnimal == null)
                    return NotFound();

                var oldAnimal = editedAnimal.Clone() as Animal;
                editedAnimal.Title = animal.Title;

                await Context.SaveChangesAsync();
                await LogService.AddUpdatedLogAsync<Animal>(oldAnimal, editedAnimal, UserId);
                return RedirectToAction("All");
            }
            return Update(animal.Id);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var animal = Context.Animals.Find(id);

            if (animal == null)
                return NotFound();

            return View(animal);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var animal = await Context.Animals.FindAsync(id);
            if (animal == null)
                return NotFound();
            Context.Animals.Remove(animal);
            await Context.SaveChangesAsync();
            await LogService.AddDeletedLogAsync(animal, UserId);
            return RedirectToAction("All");
        }
    }
}
