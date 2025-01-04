using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace SoundWeb.CustomClasses

{
    public class LayoutByRoleAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as Controller;
            if (controller != null)
            {
                var user = context.HttpContext.User;
                if (user.Identity.IsAuthenticated)
                {
                    var role = user.FindFirst(ClaimTypes.Role)?.Value;
                    switch (role)
                    {
                        case "Admin":
                            controller.ViewData["Layout"] = "~/Views/Shared/_LayoutAdmin.cshtml";
                            break;
                        case "RegisteredUser":
                            controller.ViewData["Layout"] = "~/Views/Shared/_LayoutRegisteredUser.cshtml";
                            break;
                        case "PaidUser":
                            controller.ViewData["Layout"] = "~/Views/Shared/_LayoutPaidUser.cshtml";
                            break;
                        default:
                            controller.ViewData["Layout"] = "~/Views/Shared/_Layout.cshtml";
                            break;
                    }
                }
                else
                {
                    controller.ViewData["Layout"] = "~/Views/Shared/_Layout.cshtml";
                }
            }
            base.OnActionExecuting(context);
        }
    }


}