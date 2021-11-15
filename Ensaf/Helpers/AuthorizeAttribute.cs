using Ensaf.Domain.Enums;
using Ensaf.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ensaf.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IList<Roles> _roles;

        public AuthorizeAttribute(params Roles[] roles)
        {
            _roles = roles ?? new Roles[] { };
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (User)context.HttpContext.Items["Account"];
            if(user != null)
            {
                List<string> userRoles = user.UserRoles.Select(r => r.Role.Name).ToList();
                List<string> requiredRolesList = _roles.Select(a => a.ToString()).ToList();
                if ((_roles.Any() && !requiredRolesList.All(r => userRoles.Contains(r))))
                {
                    // not logged in or role not authorized
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }
            else
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };

            }


        }
    }
}
