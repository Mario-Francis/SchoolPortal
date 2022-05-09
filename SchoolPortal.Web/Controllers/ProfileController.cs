using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.Core;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Core.Models;
using SchoolPortal.Services;
using SchoolPortal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ProfileController : Controller
    {
        private readonly IUserService userService;
        private readonly IStudentService studentService;
        private readonly ILoggerService<ProfileController> logger;

        public ProfileController(IUserService userService, 
            IStudentService studentService,
            ILoggerService<ProfileController> logger)
        {
            this.userService = userService;
            this.studentService = studentService;
            this.logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            if (!User.IsInRole(Core.Constants.ROLE_STUDENT))
            {
                return await UserProfile();
            }
            else
            {
                return await StudentProfile();
            }
        }

        [HttpGet("/WardProfile/{id}")]
        public async Task<IActionResult> WardProfile(long id)
        {
            return await StudentProfile(id);
        }

        [NonAction]
        public async Task<IActionResult> StudentProfile(long? id=null)
        {
            var student = await studentService.GetStudent(id ?? HttpContext.GetUserSession().Id);
            var model = StudentVM.FromStudent(student);
            return View("StudentProfile", model);
        }

        [HttpPost("UploadStudentPhoto")]
        public async Task<IActionResult> UploadStudentPhoto(IFormFile file, long? id)
        {
            try
            {
                var photoPath = await studentService.UploadPhoto(id ?? HttpContext.GetUserSession().Id, file);
                return Ok(new { IsSuccess = true, Message = "Photo upload successful", Data = photoPath, ErrorItems = new string[] { } });
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails(), ErrorItems = ex.ErrorItems });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while uploading student photo");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }

        [HttpPost("UpdateStudentProfile")]
        public async Task<IActionResult> UpdateStudentProfile(StudentVM model)
        {
            try
            {
                var student = await studentService.GetStudent(model.Id!=0?model.Id: HttpContext.GetUserSession().Id);
                var _student = student.Clone<Student>();
                _student.FirstName = model.FirstName;
                _student.MiddleName = model.MiddleName;
                _student.Surname = model.Surname;
                _student.Gender = model.Gender;
                _student.DateOfBirth = model.DateOfBirth;
                _student.PhoneNumber = model.PhoneNumber;
                _student.Email = model.Email;

                await studentService.UpdateStudentProfile(_student);
                return Ok(new { IsSuccess = true, Message = "Profile updated successfully", ErrorItems = new string[] { } });
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails(), ErrorItems = ex.ErrorItems });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while updating profile");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }


        [NonAction]
        public async Task<IActionResult> UserProfile()
        {
            var user = await userService.GetUser(HttpContext.GetUserSession().Id);
            var model = UserVM.FromUser(user);

            return View("UserProfile", model);
        }

        [HttpPost("UploadUserPhoto")]
        public async Task<IActionResult> UploadUserPhoto(IFormFile file)
        {
            try
            {
                var photoPath = await userService.UploadPhoto(HttpContext.GetUserSession().Id, file);
                return Ok(new { IsSuccess = true, Message = "Photo upload successful", Data=photoPath, ErrorItems = new string[] { } });
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails(), ErrorItems=ex.ErrorItems });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while uploading user photo");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }

        [HttpPost("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile(UserVM model)
        {
            try
            {
                var user = await userService.GetUser(HttpContext.GetUserSession().Id);
                var _user = user.Clone<User>();
                _user.FirstName = model.FirstName;
                _user.MiddleName = model.MiddleName;
                _user.Surname = model.Surname;
                _user.Gender = model.Gender;
                _user.DateOfBirth = model.DateOfBirth;
                _user.Email = model.Email;
                _user.PhoneNumber = model.PhoneNumber;
                _user.UserRoles = user.UserRoles.Select(ur => ur.Clone<UserRole>()).ToList();

                await userService.UpdateUser(_user);
                return Ok(new { IsSuccess = true, Message = "Profile updated successfully", ErrorItems = new string[] { } });
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails(), ErrorItems = ex.ErrorItems });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while updating profile");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }

        [HttpGet("DeleteUserPhoto")]
        public async Task<IActionResult> DeleteUserPhoto()
        {
            try
            {
                await userService.DeletePhoto(HttpContext.GetUserSession().Id);
                return Ok(new { IsSuccess = true, Message = "Photo deleted successful", ErrorItems = new string[] { } });
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails(), ErrorItems = ex.ErrorItems });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while deleting user photo");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }
    }
}
