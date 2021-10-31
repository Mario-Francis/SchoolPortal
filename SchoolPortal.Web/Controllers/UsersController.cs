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
    public class UsersController : Controller
    {
        private readonly IUserService userService;
        private readonly ILogger<UsersController> logger;

        public UsersController(IUserService userService,
            ILogger<UsersController> logger)
        {
            this.userService = userService;
            this.logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("[controller]/SearchTeachers")]
        public IActionResult SearchTeachers([FromQuery] string query, [FromQuery] int max = 100)
        {
            try
            {
                var teachers = userService.SearchTeachers(query, max).Select(t => new UserItemVM
                {
                    Id = t.Id,
                    FirstName = t.FirstName,
                    MiddleName = t.MiddleName,
                    Surname = t.Surname,
                    Username = t.Username,
                    Email = t.Email
                });
                return Ok(new { IsSuccess = true, Message = "Success", Data = teachers });
               
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error was encountered while searching teachers");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

    }
}
