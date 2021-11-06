using Microsoft.AspNetCore.Mvc;
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
    public class ProceduresController : BaseController
    {
        public ProceduresController(MonamourDataBaseContext context, ILogService logService)
            : base(context, logService)
        {
        }

        public async Task<IActionResult> All(string sort, string search, int? animalId, int? page, int? pageSize)
        {
            var viewModel = new ProceduresAllViewModel();
            viewModel.PageSettings.Sort = sort;
            viewModel.PageSettings.Search = search;
            viewModel.PageSettings.PageSize = pageSize ?? 25;
            viewModel.PageSettings.PageSizes = PageSize.GetSelectListItems(pageSize);
            viewModel.Animals = Context.Animals;
            viewModel.AnimalId = animalId;
            
            var procedures = Context.Procedures.Include(x => x.Animal).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                procedures = procedures.Where(s => s.Title.ToLower().Contains(search)
                                                   || s.Animal.Title.ToLower().Contains(search)
                                                   || s.Info.ToLower().Contains(search)
                                                   || s.Cost.ToString().Contains(search));
            }

            if (animalId != null)
            {
                procedures = procedures.Where(x => x.AnimalId == animalId);
            }

            ViewData["TitleSort"] = sort == "title" ? "title_desc" : "title";
            ViewData["AnimalSort"] = sort == "animal" ? "animal_desc" : "animal";
            ViewData["CostSort"] = sort == "cost" ? "cost_desc" : "cost";

            procedures = sort switch
            {
                "title" => procedures.OrderBy(s => s.Title),
                "title_desc" => procedures.OrderByDescending(s => s.Title),
                "animal" => procedures.OrderBy(s => s.Animal.Title),
                "animal_desc" => procedures.OrderByDescending(s => s.Animal.Title),
                "cost" => procedures.OrderBy(s => s.Cost),
                "cost_desc" => procedures.OrderByDescending(s => s.Cost),
                _ => procedures.OrderBy(s => s.Id)
            };

            viewModel.PaginatedList = await PaginatedList<Procedure>.CreateAsync(procedures.AsNoTracking(), page ?? 1, pageSize ?? 25);
            return View(viewModel);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Create()
        {
            var animals = Context.Animals.ToList();
            var viewModel = new ProcedureViewModel() 
            {
                Animals = animals
            };
            return View(viewModel);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProcedureViewModel procedureViewModel)
        {
            if (ModelState.IsValid)
            {
                procedureViewModel.Procedure.Animal = await Context.Animals.FindAsync(procedureViewModel.Procedure.AnimalId);
                Context.Procedures.Add(procedureViewModel.Procedure);
                await Context.SaveChangesAsync();
                await LogService.AddCreationLogAsync<Procedure>(procedureViewModel.Procedure, UserId);
                return RedirectToAction("All");
            }
            procedureViewModel.Animals = Context.Animals.ToList();
            return View(procedureViewModel);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var procedure = Context.Procedures
                .Include(x => x.Animal)
                .FirstOrDefault(x => x.Id == id);
            
            if (procedure == null)
                return NotFound();

            var animals = Context.Animals.ToList();
            var procedureViewModel = new ProcedureViewModel()
            {
                Procedure = procedure,
                Animals = animals
            };
            return View(procedureViewModel);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProcedureViewModel procedureViewModel)
        {
            if (ModelState.IsValid)
            {
                var procedure = Context.Procedures
                    .Include(x => x.Animal)
                    .FirstOrDefault(x => x.Id == procedureViewModel.Procedure.Id);
                if (procedure == null)
                    return NotFound();
                var oldProcedure = procedure.Clone() as Procedure;

                procedure.Title = procedureViewModel.Procedure.Title;
                procedure.AnimalId = procedureViewModel.Procedure.AnimalId;
                procedure.Cost = procedureViewModel.Procedure.Cost;
                procedure.Info = procedureViewModel.Procedure.Info;
                procedure.ApproximateTime = procedureViewModel.Procedure.ApproximateTime;
                procedure.Animal = await Context.Animals.FindAsync(procedure.AnimalId);

                await Context.SaveChangesAsync();
                await LogService.AddUpdatedLogAsync(oldProcedure, procedure, UserId);
                return RedirectToAction("All");
            }

            return Update(procedureViewModel.Procedure.Id);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var procedure = Context.Procedures
                .Include(x => x.Animal)
                .FirstOrDefault(x => x.Id == id);

            if (procedure == null)
                return NotFound();

            return View(procedure);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var procedure = Context.Procedures
                .Include(x => x.Animal)
                .FirstOrDefault(x => x.Id == id);
            if (procedure == null)
                return NotFound();
            Context.Procedures.Remove(procedure);
            await Context.SaveChangesAsync();
            await LogService.AddDeletedLogAsync(procedure, UserId);
            return RedirectToAction("All");
        }
    }
}
