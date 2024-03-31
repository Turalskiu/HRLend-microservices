using KnowledgeBaseApi.Domain.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace KnowledgeBaseApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public string? Role { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            // authorization
            var user = (User)context.HttpContext.Items["User"];
            if (user == null)
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            else if (Role != null)
            {
                ROLE r = (ROLE)Enum.Parse(typeof(ROLE), Role.ToUpperInvariant());
                if (!user.Roles.Any(rr => rr == (int)r))
                    context.Result = new JsonResult(new { message = "No rights" }) { StatusCode = StatusCodes.Status403Forbidden };
            }
        }
    }
}
