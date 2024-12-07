using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SchoolPortal.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Middlewares
{
    public class PasswordChangeCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public PasswordChangeCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<PasswordChangeCheckMiddleware> logger)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var currentUser = context.GetUserSession();
                if (!currentUser.IsInitialPasswordChanged && !context.Request.Path.Value.ToLower().Contains("passwordsetup"))
                {
                    context.Response.Redirect("/passwordSetup");
                }
                else
                {
                    if (currentUser.IsInitialPasswordChanged && context.Request.Path.Value.ToLower().Contains("passwordsetup"))
                    {
                        context.Response.Redirect("/dashboard");
                    }
                }
            }

            await _next(context);
        }
    }
}
