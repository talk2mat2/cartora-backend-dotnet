using System;
using cartoraServer.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using cartoraServer.services;

namespace cartoraServer.Helpers
{




    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (Users)context.HttpContext.Items["User"]!;
            if (user == null)
            {
                // not logged in
                //context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = new JsonResult(new { statusCode = StatusCodes.Status401Unauthorized, message = "Unauthorized or invalid token", status = false, data = new List<String> { } }) { StatusCode = StatusCodes.Status401Unauthorized };
                //context.Result = new ResData { message = "hekki", status = false } as IActionResult;
            }
        }
    }
}
