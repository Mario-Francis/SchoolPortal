using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolPortal.Core;
using SchoolPortal.Services;
using SchoolPortal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserManagerService userMgr;
        private readonly ILogger<AuthController> logger;

        public AuthController(IUserManagerService userMgr, ILogger<AuthController> logger)
        {
            this.userMgr = userMgr;
            this.logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            if(!await userMgr.AnyUserExists())
            {
                return RedirectToAction("Setup");
            }
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
    }
}
