using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MonamourWeb.Models;
using MonamourWeb.Services.Logs;

namespace MonamourWeb.Controllers
{
    [Authorize]
    public class VisitsController : BaseController
    {
        public VisitsController(MonamourDataBaseContext context, ILogService logService)
            : base(context, logService)
        {
        }
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
