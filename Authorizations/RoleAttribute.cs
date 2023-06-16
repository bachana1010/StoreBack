using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace StoreBack.Authorizations {
public class RoleAttribute : TypeFilterAttribute
{
    public RoleAttribute(params string[] roles) : base(typeof(RoleFilter))
    {
        Arguments = new object[] { roles };
    }

    private class RoleFilter : IAuthorizationFilter
    {
        private readonly string[] _roles;

        public RoleFilter(string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Check if the user is authenticated
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                return;
            }

            // Get the user's roles from the claims
            var userRoles = user.FindAll(ClaimTypes.Role).Select(c => c.Value);

            // Check if the user's role matches any of the required roles
            if (!_roles.Any(role => userRoles.Contains(role)))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
        }
    }
}
}