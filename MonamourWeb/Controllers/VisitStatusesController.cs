using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonamourWeb.Models;
using MonamourWeb.Services.Filters;

namespace MonamourWeb.Controllers
{
    [Authorize]
    public class VisitStatusesController : Controller
    {
        private readonly MonamourDataBaseContext _context;

        public VisitStatusesController(MonamourDataBaseContext context)
        {
            _context = context;
        }

        [UserRoleFilter]
        public IActionResult All()
        {
            var visitStatuses = _context.VisitStatuses;
            return View(visitStatuses);
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
        public IActionResult Create(VisitStatus visitStatus)
        {
            if (ModelState.IsValid)
            {
                _context.VisitStatuses.Add(visitStatus);
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

            var visitStatus = _context.VisitStatuses.Find(id);
            
            if (visitStatus == null)
                return NotFound();

            return View(visitStatus);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePost(VisitStatus visitStatus)
        {
            var editedVisitStatus = _context.PaymentTypes.Find(visitStatus.Id);
            if (editedVisitStatus == null)
                return NotFound();

            editedVisitStatus.Type = visitStatus.Status;

            _context.SaveChanges();
            return RedirectToAction("All");
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var visitStatus = _context.VisitStatuses.Find(id);

            if (visitStatus == null)
                return NotFound();

            return View(visitStatus);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var visitStatus = _context.VisitStatuses.Find(id);
            if (visitStatus == null)
                return NotFound();
            _context.VisitStatuses.Remove(visitStatus);
            _context.SaveChanges();
            return RedirectToAction("All");
        }
    }
}
