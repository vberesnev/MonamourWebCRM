using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MonamourWeb.Models;
using MonamourWeb.Services.Logs;

namespace MonamourWeb.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly MonamourDataBaseContext Context;
        protected readonly ILogService LogService;

        public BaseController(MonamourDataBaseContext context, ILogService logService)
        {
            Context = context;
            LogService = logService;
        }

        protected int UserId => Convert.ToInt32(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
    }
}
