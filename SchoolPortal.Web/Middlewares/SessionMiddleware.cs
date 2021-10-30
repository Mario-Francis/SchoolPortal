using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchoolPortal.Core;
using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Middlewares
{
    public class SessionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public SessionMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IUserService userService, ILogger<SessionMiddleware> logger)
        {
            if(context.User.Identity.IsAuthenticated)
            {
                await AttachUserToContext(context, userService, logger);
            }
           
            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, IUserService userService, ILogger<SessionMiddleware> logger)
        {
            try
            {
                // attach user to context 

                var id = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid).Value;
                var type = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor).Value;
                if (id != null && type != null)
                {
                    SessionObject sessionObject = null;
                    if (type == Constants.USER_TYPE_USER)
                    {
                        var user = await userService.GetUser(Convert.ToInt64(id));
                        sessionObject = SessionObject.FromUser(user);
                        context.SetUserSession(sessionObject);
                    }
                    else if (type == Constants.USER_TYPE_STUDENT)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while attaching user to the current Http Context");
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}
