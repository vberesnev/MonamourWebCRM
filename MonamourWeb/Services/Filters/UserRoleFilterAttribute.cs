using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MonamourWeb.Services.Filters
{
    public class UserRoleFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!filterContext.HttpContext.User.IsInRole("Admin"))
                filterContext.Result = new RedirectToActionResult("AccessForbidden", "Service", null);
        }
    }
}