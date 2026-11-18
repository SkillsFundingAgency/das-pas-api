using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SFA.DAS.PAS.Api.Infrastructure;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireApiVersionHeaderAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Headers.ContainsKey("X-Version"))
        {
            context.Result = new BadRequestResult();
        }
    }
}
