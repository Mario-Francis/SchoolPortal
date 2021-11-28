using DataTablesParser;
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
    public class ClassRoomsController : Controller
    {
        private readonly AppSettings appSettings;
        private readonly IClassService classService;
        private readonly IUserService userService;
        private readonly IStudentService studentService;
        private readonly ILogger<ClassRoomsController> logger;

        public ClassRoomsController(IOptions<AppSettings> appSettings,
            IClassService classService,
            IUserService  userService,
            IStudentService studentService,
            ILogger<ClassRoomsController> logger)
        {
            this.appSettings = appSettings.Value;
            this.classService = classService;
            this.userService = userService;
            this.studentService = studentService;
            this.logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ClassRoomsDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettings.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var classRooms = classService.GetClassRooms(true).Select(c => ClassRoomVM.FromClassRoom(c, clientTimeOffset));

            var parser = new Parser<ClassRoomVM>(Request.Form, classRooms.AsQueryable())
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost]
        public async Task<IActionResult> AddClassRoom(ClassRoomVM classRoomVM)
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
                    if (classRoomVM.ClassId == 0)
                    {
                        throw new AppException($"Class id is required");
                    }

                    await classService.CreateClassRoom(classRoomVM.ToClassRoom());
                    return Ok(new { IsSuccess = true, Message = "Classroom added succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while creating a new classroom");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateClassRoom(ClassRoomVM classRoomVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errs = ModelState.Values.Where(v => v.Errors.Count > 0).Select(v => v.Errors.First().ErrorMessage);
                    return StatusCode(400, new { IsSuccess = false, Message = "One or more fields failed validation", ErrorItems = errs });
                }
                else if (classRoomVM.Id == 0)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = $"Invalid classroom id {classRoomVM.Id}", ErrorItems = new string[] { } });
                }
                else
                {
                    if (classRoomVM.ClassId == 0)
                    {
                        throw new AppException($"Class id is required");
                    }

                    await classService.UpdateClassRoom(classRoomVM.ToClassRoom());
                    return Ok(new { IsSuccess = true, Message = "Classroom updated succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while updating a classroom");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateClassRoomStatus(long? id, bool isActive)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Classroom is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await classService.UpdateClassRoomStatus(id.Value, isActive);
                    return Ok(new { IsSuccess = true, Message = "Classroom status updated succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while updating a classroom status");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        public async Task<IActionResult> DeleteClassRoom(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Classroom is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await classService.DeleteClassRoom(id.Value);
                    return Ok(new { IsSuccess = true, Message = "Classroom deleted succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while deleting a classroom");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        public async Task<IActionResult> GetClassRoom(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Classroom is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    var classRoom = await classService.GetClassRoom(id.Value);
                    return Ok(new { IsSuccess = true, Message = "Classroom retrieved succeessfully", Data = ClassRoomVM.FromClassRoom(classRoom) });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while fetching a classroom");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpGet("[controller]/GetClassRooms/{classid}")]
        public IActionResult GetClassRooms(long? classid)
        {
            try
            {
                if (classid == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Class is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    var classRooms = classService.GetClassRooms().Where(r => r.ClassId == classid).Select(c => ClassRoomVM.FromClassRoom(c));
                    return Ok(new { IsSuccess = true, Message = "Classroom retrieved succeessfully", Data = classRooms });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while fetching a classroom");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }
        
        [HttpGet("[controller]/{id}")]
        public async Task<IActionResult> ViewClassRoom(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Classroom is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    var classRoom = await classService.GetClassRoom(id.Value);
                    return View(ClassRoomVM.FromClassRoom(classRoom));
                }
            }
            catch (AppException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while fetching a classroom");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[controller]/{id}/TeachersDataTable")]
        public async Task<IActionResult> ClassRoomTeachersDataTable(long? id)
        {
            if (id == null)
            {
                return StatusCode(400, new { IsSuccess = false, Message = "Classroom is not found", ErrorItems = new string[] { } });
            }
            else
            {
                var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
             appSettings.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

                var teachers = (await classService.GetClassRoom(id.Value)).ClassRoomTeachers.Select(t => UserVM.FromUser(t.Teacher, clientTimeOffset));

                var parser = new Parser<UserVM>(Request.Form, teachers.AsQueryable())
                    .SetConverter(x => x.DateOfBirth, x => x.DateOfBirth == null ? "" : x.DateOfBirth.Value.ToString("MMM d, yyyy"))
                      .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                       .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

                return Ok(parser.Parse());
            }
          
        }

        [HttpPost("[controller]/{id}/StudentsDataTable")]
        public async Task<IActionResult> ClassRoomStudentsDataTable(long? id)
        {
            if (id == null)
            {
                return StatusCode(400, new { IsSuccess = false, Message = "Classroom is not found", ErrorItems = new string[] { } });
            }
            else
            {
                var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
             appSettings.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

                var students = (await classService.GetClassRoom(id.Value)).ClassRoomStudents.Select(t => StudentVM.FromStudent(t.Student, clientTimeOffset));

                var parser = new Parser<StudentVM>(Request.Form, students.AsQueryable())
                    .SetConverter(x => x.DateOfBirth, x =>  x.DateOfBirth.ToString("MMM d, yyyy"))
                      .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                       .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

                return Ok(parser.Parse());
            }
        }

    }
}
