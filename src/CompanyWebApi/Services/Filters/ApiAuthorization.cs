using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace CompanyWebApi.Services.Filters
{
    /// <summary>
    /// Custom authorization filter attribute
    /// </summary>
    public class ApiAuthorization : Attribute, IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }

            // Check whether a controller and action does contain AllowAnonymousAttribute
            var hasAllowAnonymous = filterContext.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute));
            if (hasAllowAnonymous)
            {
                return Task.CompletedTask;
            }

            // Authorization check
            var user = filterContext.HttpContext.User;
            if (user.Identity is not {IsAuthenticated: false})
            {
                // User is authenticated
                return Task.CompletedTask;
            }

            filterContext.Result = new UnauthorizedObjectResult("*** Please authorize user! ***");
            return Task.CompletedTask;
        }
    }
}