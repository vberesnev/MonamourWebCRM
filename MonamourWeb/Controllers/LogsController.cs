using Microsoft.AspNetCore.Mvc;
using System;
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
    public class LogsController : Controller
    {
        private readonly MonamourDataBaseContext _context;

        public LogsController(MonamourDataBaseContext context)
        {
            _context = context;
        }

        [UserRoleFilter]
        public async Task<IActionResult> Index(DateTime? begin, DateTime? end, int? userId, string sort, string search, int? page, int? pageSize)
        {
            begin = begin.HasValue ? begin.Value.Date : DateTime.Now.Date;
            end = end.HasValue ? end.Value.Date : DateTime.Now.Date;

            var viewModel = new LogsViewModel();
            viewModel.PageSettings.Sort = sort;
            viewModel.PageSettings.Search = search;
            viewModel.PageSettings.PageSize = pageSize ?? 50;
            viewModel.PageSettings.PageSizes = PageSize.GetSelectListItems(pageSize);
            viewModel.Users = _context.Users;
            viewModel.UserId = userId;
            viewModel.Begin = begin;
            viewModel.End = end;

            var logs = _context.Logs.Include(x => x.User).Where(x => x.Date >= begin && x.Date < end.Value.AddDays(1)).AsQueryable();

            if (userId != null)
                logs = logs.Where(x => x.UserId == userId);
            
            if (!string.IsNullOrEmpty(search))
                logs = logs.Where(s => s.Message.ToLower().Contains(search.ToLower()));
            
            ViewData["DateSort"] = sort == "date" ? "date_desc" : "date";
            ViewData["UserSort"] = sort == "user" ? "user_desc" : "user";

            logs = sort switch
            {
                "date" => logs.OrderBy(s => s.Date),
                "date_desc" => logs.OrderByDescending(s => s.Date),
                "user" => logs.OrderBy(s => s.User.Name),
                "user_desc" => logs.OrderByDescending(s => s.User.Name),
                _ => logs.OrderBy(s => s.Date)
            };

            viewModel.PaginatedList = await PaginatedList<Log>.CreateAsync(logs.AsNoTracking(), page ?? 1, pageSize ?? 50);
            return View(viewModel);
        }
    }
}
