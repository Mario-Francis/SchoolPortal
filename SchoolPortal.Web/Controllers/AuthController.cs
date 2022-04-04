using Castle.Core.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolPortal.Core;
using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Services;
using SchoolPortal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService userMgr;
        private readonly IStudentService studentService;
        private readonly ILogger<AuthController> logger;
        private readonly IHttpContextAccessor contextAccessor;

        public AuthController(IUserService userMgr, 
            IStudentService studentService,
            ILogger<AuthController> logger,
            IHttpContextAccessor contextAccessor)
        {
            this.userMgr = userMgr;
            this.studentService = studentService;
            this.logger = logger;
            this.contextAccessor = contextAccessor;
        }
        public async Task<IActionResult> Index(string returnUrl = null)
        {
            if(!await userMgr.AnyUserExists())
            {
                return RedirectToAction("Setup");
            }else if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        public async Task<IActionResult> Setup()
        {
            if (await userMgr.AnyUserExists())
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Setup(SetupVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errs = ModelState.Values.Where(v => v.Errors.Count > 0).Select(v => v.Errors.First().ErrorMessage);
                    return StatusCode(400, new { IsSuccess = false, Message = "One or more fields failed validation", ErrorItems = errs });
                }
                else
                {
                    if (model.Password != model.ConfirmPassword)
                    {
                        throw new AppException($"Passwords don't match");
                    }

                    await userMgr.InitialUserSetup(model.ToUser());
                    return Ok(new { IsSuccess = true, Message = "Setup completed successfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());
                logger.LogError(ex, "An error was encountered during initial app setup");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errs = ModelState.Values.Where(v => v.Errors.Count > 0).Select(v => v.Errors.First().ErrorMessage);
                    return StatusCode(400, new { IsSuccess = false, Message = "One or more fields failed validation", ErrorItems = errs });
                }
                else
                {
                    if(model.Type == "parent" || model.Type == "staff")
                    {
                        var isValid = await userMgr.IsUserAuthentic(new LoginCredential { Email = model.Username, Password = model.Password });
                        var user = await userMgr.GetUser(model.Username);
                        if(model.Type == "parent" && !user.UserRoles.Any(ur=> ur.RoleId == (long)AppRoles.PARENT))
                        {
                            throw new AppException($"Invalid credentials");
                        }
                        if (model.Type == "staff" && !user.UserRoles.Any(ur => ur.RoleId == (long)AppRoles.ADMINISTRATOR || ur.RoleId == (long)AppRoles.HEAD_TEACHER || ur.RoleId == (long)AppRoles.TEACHER))
                        {
                            throw new AppException($"Invalid credentials");
                        }

                        var sessionObject = SessionObject.FromUser(user);

                        ClaimsIdentity identity = new ClaimsIdentity(Constants.AUTH_COOKIE_ID);
                        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, sessionObject.Username));
                        identity.AddClaim(new Claim(ClaimTypes.Email, sessionObject.Email));
                        identity.AddClaim(new Claim(ClaimTypes.Surname, sessionObject.Surname));
                        identity.AddClaim(new Claim(ClaimTypes.GivenName, sessionObject.FirstName));
                        identity.AddClaim(new Claim(ClaimTypes.Name, sessionObject.FullName));
                        identity.AddClaim(new Claim(ClaimTypes.Sid, sessionObject.Id.ToString()));
                        identity.AddClaim(new Claim(ClaimTypes.Actor, Constants.USER_TYPE_USER));

                        foreach (var r in sessionObject.Roles)
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, r.Name));
                        }
                        var principal = new ClaimsPrincipal(identity);
                        await contextAccessor.HttpContext.SignInAsync(Constants.AUTH_COOKIE_ID, principal);

                        return Ok(new { IsSuccess = true, Message = "You were logged in successfully", ErrorItems = new string[] { } });
                    }
                    else if(model.Type == "student")
                    {
                        var isValid = await studentService.IsStudentAuthentic(new LoginCredential { Email = model.Username, Password = model.Password });
                        var student = await studentService.GetStudent(model.Username);
                        var sessionObject = SessionObject.FromStudent(student);

                        ClaimsIdentity identity = new ClaimsIdentity(Constants.AUTH_COOKIE_ID);
                        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, sessionObject.Username));
                        identity.AddClaim(new Claim(ClaimTypes.Email, sessionObject.Email));
                        identity.AddClaim(new Claim(ClaimTypes.Surname, sessionObject.Surname));
                        identity.AddClaim(new Claim(ClaimTypes.GivenName, sessionObject.FirstName));
                        identity.AddClaim(new Claim(ClaimTypes.Name, sessionObject.FullName));
                        identity.AddClaim(new Claim(ClaimTypes.Sid, sessionObject.Id.ToString()));
                        identity.AddClaim(new Claim(ClaimTypes.Actor, Constants.USER_TYPE_STUDENT));

                        foreach (var r in sessionObject.Roles)
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, r.Name));
                        }
                        var principal = new ClaimsPrincipal(identity);
                        await contextAccessor.HttpContext.SignInAsync(Constants.AUTH_COOKIE_ID, principal);

                        return Ok(new { IsSuccess = true, Message = "You were logged in successfully", ErrorItems = new string[] { } });
                    }
                    else
                    {
                        throw new AppException($"Invalid user type '{model.Type}'");
                    }
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());
                logger.LogError(ex, "An error was encountered during initial app setup");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.ClearUserSession();
            await HttpContext.SignOutAsync(Constants.AUTH_COOKIE_ID);

            return RedirectToAction("Index");
        }

    }
}
