using Microsoft.AspNetCore.Mvc;
using System;
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
    public class LogsController : BaseController
    {
        public LogsController(MonamourDataBaseContext context, ILogService logService)
            : base(context, logService)
        {
        }

        [UserRoleFilter]
        public async Task<IActionResult> Index(DateTime begin, DateTime end, int? userId, string sort, string search, int? page, int? pageSize)
        {
            if (begin == DateTime.MinValue)
                begin = DateTime.Now.Date;

            if (end == DateTime.MinValue)
                end = DateTime.Now.Date;

            var viewModel = new LogsViewModel();
            viewModel.PageSettings.Sort = sort;
            viewModel.PageSettings.Search = search;
            viewModel.PageSettings.PageSize = pageSize ?? 50;
            viewModel.PageSettings.PageSizes = PageSize.GetSelectListItems(pageSize);
            viewModel.Users = Context.Users;
            viewModel.UserId = userId;
            viewModel.Begin = begin;
            viewModel.End = end;

            var logs = Context.Logs
                .Include(x => x.User)
                .Where(x => x.Date >= begin && x.Date < end.AddDays(1))
                .AsQueryable();

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

        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var log = Context.Logs
                .Include(x => x.User)
                .FirstOrDefault(x => x.Id == id);

            if (log == null)
                return NotFound();

            return View(log);
        }
    }
}
