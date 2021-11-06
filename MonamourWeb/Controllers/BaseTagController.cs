using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MonamourWeb.Models;
using MonamourWeb.Services.Logs;

namespace MonamourWeb.Controllers
{
    public abstract  class BaseTagController : BaseController
    {
        protected IEnumerable<string> Colors;

        public BaseTagController(MonamourDataBaseContext context, ILogService logService)
            : base(context, logService)
        {
            Colors = new[]
            {
                "Red",
                "Green",
                "Blue",
                "Orange",
                "Brown",
                "Pink",
                "Yellow",
                "Coral",
                "Lime",
                "Cyan"
            };
        }
    }
}
