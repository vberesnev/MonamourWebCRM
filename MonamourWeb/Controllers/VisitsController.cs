using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonamourWeb.Models;
using MonamourWeb.Services.Logs;
using MonamourWeb.Services.Pagination;
using MonamourWeb.ViewModels;

namespace MonamourWeb.Controllers
{
    [Authorize]
    public class VisitsController : BaseController
    {
        public VisitsController(MonamourDataBaseContext context, ILogService logService)
            : base(context, logService)
        {
        }
        
        public async Task<IActionResult> Index(DateTime begin, DateTime end, int? userId, int? statusId, string sort, string search, int? page, int? pageSize)
        {
            if (begin == DateTime.MinValue)
                begin = DateTime.Now.Date;

            if (end == DateTime.MinValue)
                end = DateTime.Now.Date;
            
            var viewModel = new VisitListViewModel();
            viewModel.PageSettings.Sort = sort;
            viewModel.PageSettings.Search = search;
            viewModel.PageSettings.PageSize = pageSize ?? 50;
            viewModel.PageSettings.PageSizes = PageSize.GetSelectListItems(pageSize);
            viewModel.Users = Context.Users
                .Include(x => x.Role)
                .Where(x => x.Role.Title == "Master");
            viewModel.UserId = userId;
            viewModel.Statuses = Context.VisitStatuses;
            viewModel.StatusId = userId;
            viewModel.Begin = begin;
            viewModel.End = end;

            var visits = Context.Visits
                .Include(x => x.Status)
                .Include(x => x.User)
                .Include(x => x.Pet).ThenInclude(x => x.Tags)
                .Include(x => x.Pet).ThenInclude(x => x.Breed)
                .Include(x => x.Pet).ThenInclude(x => x.Clients).ThenInclude(x => x.Tags)
                .Include(x => x.Payments).ThenInclude(x => x.PaymentType)
                .Where(x => x.Date >= begin && x.Date < end.AddDays(1))
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                visits = visits.Where(s => s.Pet.Name.ToLower().Contains(search) 
                                           || s.Pet.Clients.Any(x => x.Name.ToLower().Contains(search)));
            }

            if (userId != null)
            {
                visits = visits.Where(x => x.UserId == userId );
            }

            if (statusId != null)
            {
                visits = visits.Where(x => x.StatusId == statusId);
            }

            viewModel.TotalCount = visits.Count();
            viewModel.TotalSum = visits.Sum(x => x.Sum);

            ViewData["StatusSort"] = sort == "status" ? "status_desc" : "status";
            ViewData["DateSort"] = sort == "date" ? "date_desc" : "date";
            ViewData["TimeSort"] = sort == "time" ? "time_desc" : "time";
            ViewData["MasterSort"] = sort == "master" ? "master_desc" : "master";
            ViewData["SumSort"] = sort == "sum" ? "sum_desc" : "sum";

            visits = sort switch
            {
                "status" => visits.OrderBy(s => s.Status.Status),
                "status_desc" => visits.OrderByDescending(s => s.Status.Status),
                "date" => visits.OrderBy(s => s.Date),
                "date_desc" => visits.OrderByDescending(s => s.Date),
                "time" => visits.OrderBy(s => s.TimeBegin),
                "time_desc" => visits.OrderByDescending(s => s.TimeBegin),
                "master" => visits.OrderBy(s => s.User.Name),
                "master_desc" => visits.OrderByDescending(s => s.User.Name),
                "sum" => visits.OrderBy(s => s.Sum),
                "sum_desc" => visits.OrderByDescending(s => s.Sum),
                _ => visits.OrderBy(s => s.TimeBegin)
            };

            viewModel.PaginatedList = await PaginatedList<Visit>.CreateAsync(visits.AsNoTracking(), page ?? 1, pageSize ?? 25);
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var users = Context.Users
                .Include(x => x.Role)
                .Where(x => x.Role.Title == "Master");

            var statuses = Context.VisitStatuses;

            var viewModel = new VisitViewModel();
            viewModel.Visit.Date = DateTime.Now;
            viewModel.Visit.TimeBegin = DateTime.Now;
            viewModel.Masters = users;
            viewModel.Visit.User = users.FirstOrDefault(x => x.Id == UserId) ?? users.First();
            viewModel.Visit.Pet = new Pet();

            return View(viewModel);
        }
    }
}
