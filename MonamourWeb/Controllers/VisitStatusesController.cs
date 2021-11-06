using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonamourWeb.Models;
using MonamourWeb.Services.Filters;
using MonamourWeb.Services.Logs;

namespace MonamourWeb.Controllers
{
    [Authorize]
    public class VisitStatusesController : BaseController
    {
        public VisitStatusesController(MonamourDataBaseContext context, ILogService logService)
            : base(context, logService)
        {
        }

        [UserRoleFilter]
        public IActionResult All()
        {
            var visitStatuses = Context.VisitStatuses;
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
        public async Task<IActionResult> Create(VisitStatus visitStatus)
        {
            if (ModelState.IsValid)
            {
                Context.VisitStatuses.Add(visitStatus);
                await Context.SaveChangesAsync();
                await LogService.AddCreationLogAsync(visitStatus, UserId);
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

            var visitStatus = Context.VisitStatuses.Find(id);
            
            if (visitStatus == null)
                return NotFound();

            return View(visitStatus);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(VisitStatus visitStatus)
        {
            if (ModelState.IsValid)
            {
                var editedVisitStatus = await Context.VisitStatuses.FindAsync(visitStatus.Id);
                if (editedVisitStatus == null)
                    return NotFound();
                var oldVisitStatus = editedVisitStatus.Clone() as VisitStatus;
                editedVisitStatus.Status = visitStatus.Status;

                await Context.SaveChangesAsync();
                await LogService.AddUpdatedLogAsync(oldVisitStatus, editedVisitStatus, UserId);
                return RedirectToAction("All");
            }

            return Update(visitStatus.Id);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var visitStatus = Context.VisitStatuses.Find(id);

            if (visitStatus == null)
                return NotFound();

            return View(visitStatus);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var visitStatus = await Context.VisitStatuses.FindAsync(id);
            if (visitStatus == null)
                return NotFound();
            Context.VisitStatuses.Remove(visitStatus);
            await Context.SaveChangesAsync();
            await LogService.AddDeletedLogAsync(visitStatus, UserId);
            return RedirectToAction("All");
        }
    }
}
