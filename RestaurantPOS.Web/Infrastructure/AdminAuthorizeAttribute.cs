using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RestaurantPOS.Web.Infrastructure
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var waiterId = context.HttpContext.Session.GetString(SessionKeys.WaiterId);
            var isManager = context.HttpContext.Session.GetString(SessionKeys.IsManager);

            if (string.IsNullOrWhiteSpace(waiterId) || isManager != "1")
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}