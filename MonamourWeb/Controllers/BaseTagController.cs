using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MonamourWeb.Controllers
{
    public abstract  class BaseTagController : Controller
    {
        protected IEnumerable<string> Colors;

        public BaseTagController()
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
