using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonamourWeb.Models;
using MonamourWeb.Services.Filters;

namespace MonamourWeb.Controllers
{
    [Authorize]
    public class AnimalsController : Controller
    {
        private readonly MonamourDataBaseContext _context;

        public AnimalsController(MonamourDataBaseContext context)
        {
            _context = context;
        }

        public IActionResult All()
        {
            var users = _context.Animals;
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
        public IActionResult Create(Animal animal)
        {
            if (ModelState.IsValid)
            {
                _context.Animals.Add(animal);
                _context.SaveChanges();
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

            var animal = _context.Animals.Find(id);
            
            if (animal == null)
                return NotFound();

            return View(animal);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePost(Animal animal)
        {
            var editedAnimal = _context.Animals.Find(animal.Id);
            if (editedAnimal == null)
                return NotFound();

            editedAnimal.Title = animal.Title;

            _context.SaveChanges();
            return RedirectToAction("All");
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var animal = _context.Animals.Find(id);

            if (animal == null)
                return NotFound();

            return View(animal);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var animal = _context.Animals.Find(id);
            if (animal == null)
                return NotFound();
            _context.Animals.Remove(animal);
            _context.SaveChanges();
            return RedirectToAction("All");
        }
    }
}
