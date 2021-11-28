using DataTablesParser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchoolPortal.Core;
using SchoolPortal.Core.DTOs;
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
        private readonly AppSettings appSettings;
        private readonly ILogger<UsersController> logger;

        public UsersController(IUserService userService,
            IOptions<AppSettings> appSettings,
            ILogger<UsersController> logger)
        {
            this.userService = userService;
            this.appSettings = appSettings.Value;
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

        [HttpGet("[controller]/SearchParents")]
        public IActionResult SearchParents([FromQuery] string query, [FromQuery] int max = 100)
        {
            try
            {
                var teachers = userService.SearchParents(query, max).Select(t => new UserItemVM
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
                logger.LogError(ex, $"An error was encountered while searching parents");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }


        [HttpPost]
        public IActionResult UsersDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettings.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var Users = userService.GetUsers().Select(u => UserVM.FromUser(u, clientTimeOffset));

            var parser = new Parser<UserVM>(Request.Form, Users.AsQueryable())
                .SetConverter(x => x.DateOfBirth, x => x.DateOfBirth ==null?"":x.DateOfBirth.Value.ToString("MMM d, yyyy"))
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost]
        public IActionResult AdminsDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettings.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var Users = userService.GetUsers().Where(u=> u.UserRoles.Any(ur=> ur.RoleId == Convert.ToInt64(AppRoles.ADMINISTRATOR) || ur.RoleId == Convert.ToInt64(AppRoles.HEAD_TEACHER)))
                .Select(u => UserVM.FromUser(u, clientTimeOffset));

            var parser = new Parser<UserVM>(Request.Form, Users.AsQueryable())
                .SetConverter(x => x.DateOfBirth, x => x.DateOfBirth == null ? "" : x.DateOfBirth.Value.ToString("MMM d, yyyy"))
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost]
        public IActionResult TeachersDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettings.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var Users = userService.GetUsers().Where(u => u.UserRoles.Any(ur => ur.RoleId == Convert.ToInt64(AppRoles.TEACHER)))
                .Select(u => UserVM.FromUser(u, clientTimeOffset));

            var parser = new Parser<UserVM>(Request.Form, Users.AsQueryable())
                .SetConverter(x => x.DateOfBirth, x => x.DateOfBirth == null ? "" : x.DateOfBirth.Value.ToString("MMM d, yyyy"))
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost]
        public IActionResult ParentsDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettings.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var Users = userService.GetUsers().Where(u => u.UserRoles.Any(ur => ur.RoleId == Convert.ToInt64(AppRoles.PARENT)))
                .Select(u => UserVM.FromUser(u, clientTimeOffset));

            var parser = new Parser<UserVM>(Request.Form, Users.AsQueryable())
                .SetConverter(x => x.DateOfBirth, x => x.DateOfBirth == null ? "" : x.DateOfBirth.Value.ToString("MMM d, yyyy"))
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserVM userVM)
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
                    if (userVM.Roles.Count() == 0)
                    {
                        throw new AppException($"Role is required");
                    }

                    await userService.CreateUser(userVM.ToUser());
                    return Ok(new { IsSuccess = true, Message = "User added succeessfully and credentials sent to user via mail", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while creating a new user");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserVM userVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errs = ModelState.Values.Where(v => v.Errors.Count > 0).Select(v => v.Errors.First().ErrorMessage);
                    return StatusCode(400, new { IsSuccess = false, Message = "One or more fields failed validation", ErrorItems = errs });
                }
                else if (userVM.Id == 0)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = $"Invalid user id {userVM.Id}", ErrorItems = new string[] { } });
                }
                else
                {
                    if (userVM.Roles.Count() == 0)
                    {
                        throw new AppException($"Role is required");
                    }

                    await userService.UpdateUser(userVM.ToUser());
                    return Ok(new { IsSuccess = true, Message = "User updated succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while updating a user");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }


        public async Task<IActionResult> DeleteUser(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "User is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await userService.DeleteUser(id.Value);
                    return Ok(new { IsSuccess = true, Message = "User deleted succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while deleting a user");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        public async Task<IActionResult> ResetPassword(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "User is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await userService.ResetPassword(id.Value);
                    return Ok(new { IsSuccess = true, Message = "User password reset succeessfully and new password sent via mail", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while reseting a user's password");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        public async Task<IActionResult> GetUser(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "User is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    var User = await userService.GetUser(id.Value);
                    return Ok(new { IsSuccess = true, Message = "User retrieved succeessfully", Data = UserVM.FromUser(User) });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while fetching a user");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserStatus(long? id, bool isActive)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "User is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await userService.UpdateUserStatus(id.Value, isActive);
                    return Ok(new { IsSuccess = true, Message = "User status updated succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while updating a user status");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignClassRoom(long? id, long? roomId)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "User is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await userService.AssignClassRoom(id.Value, roomId);
                    return Ok(new { IsSuccess = true, Message = "Teacher assigned to classroom succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while assigning teaccher to classroom");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BatchAddUsers(IFormFile file, long roleId)
        {
            try
            {
                if (file == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "No file uploaded!", ErrorItems = new string[] { } });
                }
                else
                {
                    if (!userService.ValidateFile(file, out List<string> errItems))
                    {
                        return StatusCode(400, new { IsSuccess = false, Message = "Invalid file uploaded.", ErrorItems = errItems });
                    }
                    else
                    {
                        var users = await userService.ExtractData(file);

                        await userService.BatchCreateUser(users, roleId);

                        return Ok(new { IsSuccess = true, Message = "File uploaded and read and users created successfully", ErrorItems = new string[] { } });
                        
                    }
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while adding users in batch");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpGet("[controller]/{id}")]
        public async Task<IActionResult> UserProfile(long? id)
        {
            if (id == null)
            {
                return NotFound(new { ISSuccess = true, Message = "User is not found", ErrorItems = new string[] { } });
            }
            var user = await userService.GetUser(id.Value);
            if (user == null)
            {
                return NotFound("User is not found");
            }

            return View(UserVM.FromUser(user));
        }

        [HttpGet("[controller]/{userId}/Wards")]
        public async Task<IActionResult> Wards(long? userId)
        {
            if (userId == null)
            {
                return NotFound(new { ISSuccess = true, Message = "user is not found", ErrorItems = new string[] { } });
            }
            var user = await userService.GetUser(userId.Value);
            if (user == null)
            {
                return NotFound("User is not found");
            }
            if(!user.UserRoles.Any(r=>r.RoleId == (int)AppRoles.PARENT))
            {
                return NotFound("User is not found");
            }

            return View(UserVM.FromUser(user));
        }

        [HttpPost("[controller]/WardsDataTable/{userId}")]
        public async Task<IActionResult> WardsDataTable(long? userId)
        {
            if (userId == null)
            {
                return NotFound(new { ISSuccess = true, Message = "User is not found", ErrorItems = new string[] { } });
            }
            var user = await userService.GetUser(userId.Value);
            if (user == null)
            {
                return NotFound(new { ISSuccess = true, Message = "User is not found", ErrorItems = new string[] { } });
            }
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettings.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var wards = user.StudentGuardians.Select(g => WardVM.FromStudentGuardian(g, clientTimeOffset));

            var parser = new Parser<WardVM>(Request.Form, wards.AsQueryable());

            return Ok(parser.Parse());
        }

        [HttpPost]
        public async Task<IActionResult> AddWard(WardVM wardVM)
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
                    var ward = wardVM.ToStudentGuardian();

                    await userService.AddParentWard(ward.StudentId, ward.GuardianId, ward.RelationshipId);
                    return Ok(new { IsSuccess = true, Message = "Ward added succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while creating a new ward");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        public async Task<IActionResult> RemoveWard(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Ward is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await userService.RemoveParentWard(id.Value);
                    return Ok(new { IsSuccess = true, Message = "Ward removed succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while removing ward");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }


    }
}
