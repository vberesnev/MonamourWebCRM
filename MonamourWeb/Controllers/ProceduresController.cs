using Microsoft.AspNetCore.Mvc;
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
    public class ProceduresController : Controller
    {
        private readonly MonamourDataBaseContext _context;

        public ProceduresController(MonamourDataBaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> All(string sort, string search, int? animalId, int? page, int? pageSize)
        {
            var viewModel = new ProceduresAllViewModel();
            viewModel.PageSettings.Sort = sort;
            viewModel.PageSettings.Search = search;
            viewModel.PageSettings.PageSize = pageSize ?? 25;
            viewModel.PageSettings.PageSizes = PageSize.GetSelectListItems(pageSize);
            viewModel.Animals = _context.Animals;
            viewModel.AnimalId = animalId;
            
            var procedures = _context.Procedures.Include(x => x.Animal).AsQueryable();

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
            var animals = _context.Animals.ToList();
            var viewModel = new ProcedureViewModel() 
            {
                Animals = animals
            };
            return View(viewModel);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProcedureViewModel procedureViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Procedures.Add(procedureViewModel.Procedure);
                _context.SaveChanges();
                return RedirectToAction("All");
            }
            procedureViewModel.Animals = _context.Animals.ToList();
            return View(procedureViewModel);
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var procedure = _context.Procedures.Include(x => x.Animal).FirstOrDefault(x => x.Id == id);
            
            if (procedure == null)
                return NotFound();

            var animals = _context.Animals.ToList();
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
        public IActionResult UpdatePost(ProcedureViewModel procedureViewModel)
        {
            var procedure = _context.Procedures.Find(procedureViewModel.Procedure.Id);
            if (procedure == null)
                return NotFound();

            procedure.Title = procedureViewModel.Procedure.Title;
            procedure.AnimalId = procedureViewModel.Procedure.AnimalId;
            procedure.Cost = procedureViewModel.Procedure.Cost;
            procedure.Info = procedureViewModel.Procedure.Info;
            
            _context.SaveChanges();
            return RedirectToAction("All");
        }

        [UserRoleFilter]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var procedure = _context.Procedures.Include(x => x.Animal).FirstOrDefault(x => x.Id == id);

            if (procedure == null)
                return NotFound();

            return View(procedure);
        }

        [UserRoleFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var procedure = _context.Procedures.Find(id);
            if (procedure == null)
                return NotFound();
            _context.Procedures.Remove(procedure);
            _context.SaveChanges();
            return RedirectToAction("All");
        }
    }
}
