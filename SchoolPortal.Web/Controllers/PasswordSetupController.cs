using Microsoft.AspNetCore.Mvc;
using SchoolPortal.Core;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Services;
using SchoolPortal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Controllers
{
    public class PasswordSetupController : Controller
    {
        private readonly IUserService userService;
        private readonly IStudentService studentService;
        private readonly ILoggerService<PasswordSetupController> logger;

        public PasswordSetupController(IUserService userService, 
            IStudentService studentService,
            ILoggerService<PasswordSetupController> logger)
        {
            this.userService = userService;
            this.studentService = studentService;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> Index(ResetPasswordVM model)
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
                    if (model.UserType.ToLower() == Constants.USER_TYPE_STUDENT.ToLower())
                    {
                        await studentService.SetupPassword(model.ToPasswordRequestObject());
                    }
                    else
                    {
                        await userService.SetupPassword(model.ToPasswordRequestObject());
                    }

                    TempData["successMessage"] = "Password setup successful!";
                    return Ok(new { IsSuccess = true, Message = "Password setup successful", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails(), ErrorItems = ex.ErrorItems });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }
    }
}
