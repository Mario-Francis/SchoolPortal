using DataTablesParser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchoolPortal.Core;
using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Services;
using SchoolPortal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ClassRoomsController : Controller
    {
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;
        private readonly IClassService classService;
        private readonly IUserService userService;
        private readonly IStudentService studentService;
        private readonly ILoggerService<ClassRoomsController> logger;

        public ClassRoomsController(
            IOptionsSnapshot<AppSettings> appSettingsDelegate,
            IClassService classService,
            IUserService  userService,
            IStudentService studentService,
            ILoggerService<ClassRoomsController> logger)
        {
            this.appSettingsDelegate = appSettingsDelegate;
            this.classService = classService;
            this.userService = userService;
            this.studentService = studentService;
            this.logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("ClassRoomsDataTable")]
        public IActionResult ClassRoomsDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var classRooms = classService.GetClassRooms(true).Select(c => ClassRoomVM.FromClassRoom(c, clientTimeOffset));

            var parser = new Parser<ClassRoomVM>(Request.Form, classRooms.AsQueryable())
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost("AddClassRoom")]
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
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails(), ErrorItems=ex.ErrorItems });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while creating a new classroom");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }


        [HttpPost("UpdateClassRoom")]
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
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails(), ErrorItems = ex.ErrorItems });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while updating a classroom");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }

        [HttpPost("UpdateClassRoomStatus/{id}")]
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
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails(), ErrorItems = ex.ErrorItems });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while updating a classroom status");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }

        [HttpGet("DeleteClassRoom/{id}")]
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
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails(), ErrorItems = ex.ErrorItems });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while deleting a classroom");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail =ex.GetErrorDetails() });
            }
        }

        [HttpGet("GetClassRoom/{id}")]
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
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails(), ErrorItems = ex.ErrorItems });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while fetching a classroom");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }

        [HttpGet("GetClassRooms/{classid}")]
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
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails(), ErrorItems = ex.ErrorItems });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while fetching a classroom");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }
        
        [HttpGet("{id}")]
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
                logger.LogException(ex);
                logger.LogError("An error was encountered while fetching a classroom");

                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{id}/TeachersDataTable")]
        public async Task<IActionResult> ClassRoomTeachersDataTable(long? id)
        {
            if (id == null)
            {
                return StatusCode(400, new { IsSuccess = false, Message = "Classroom is not found", ErrorItems = new string[] { } });
            }
            else
            {
                var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
             appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

                var teachers = (await classService.GetClassRoom(id.Value)).ClassRoomTeachers.Select(t => UserVM.FromUser(t.Teacher, clientTimeOffset));

                var parser = new Parser<UserVM>(Request.Form, teachers.AsQueryable())
                    .SetConverter(x => x.DateOfBirth, x => x.DateOfBirth == null ? "" : x.DateOfBirth.Value.ToString("MMM d, yyyy"))
                      .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                       .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

                return Ok(parser.Parse());
            }
          
        }

        [HttpPost("{id}/StudentsDataTable")]
        public async Task<IActionResult> ClassRoomStudentsDataTable(long? id)
        {
            if (id == null)
            {
                return StatusCode(400, new { IsSuccess = false, Message = "Classroom is not found", ErrorItems = new string[] { } });
            }
            else
            {
                var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
             appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

                var students = (await classService.GetClassRoom(id.Value)).ClassRoomStudents.Select(t => StudentVM.FromStudent(t.Student, clientTimeOffset));

                var parser = new Parser<StudentVM>(Request.Form, students.AsQueryable())
                    .SetConverter(x => x.DateOfBirth, x =>  x.DateOfBirth.ToString("MMM d, yyyy"))
                      .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                       .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

                return Ok(parser.Parse());
            }
        }

        #region teachers

        [HttpGet("/MyClassRoom")]
        public async Task<IActionResult> MyClassRoom()
        {
            if (HttpContext.GetUserSession().HasClassRoom)
            {
                var classroom = await classService.GetClassRoom(HttpContext.GetUserSession().ClassRoomId);
                var teachers = classroom.ClassRoomTeachers.Select(ct => UserVM.FromUser(ct.Teacher));
                var subjects = classroom.Class.Subjects.Select(s => SubjectVM.FromSubject(s));

                ViewData["teachers"] = teachers;
                ViewData["subjects"] = subjects;
                return View(ClassRoomVM.FromClassRoom(classroom));
            }
            else
            {
                return View(null);
            }
           
        }

        #endregion teachers

        #region parents
        [HttpGet("WardClassRoom/{classRoomId}")]
        public async Task<IActionResult> WardClassRoom(long classRoomId)
        {
            var classroom = await classService.GetClassRoom(classRoomId);
            var teachers = classroom.ClassRoomTeachers.Select(ct => UserVM.FromUser(ct.Teacher));
            var subjects = classroom.Class.Subjects.Select(s => SubjectVM.FromSubject(s));
            var maleCount = classroom.ClassRoomStudents.Count(rs => rs.Student.Gender == "Male");
            var femaleCount = classroom.ClassRoomStudents.Count(rs => rs.Student.Gender == "Female");


            ViewData["teachers"] = teachers;
            ViewData["subjects"] = subjects;
            ViewData["maleCount"] = maleCount;
            ViewData["femaleCount"] = femaleCount;

            return View(ClassRoomVM.FromClassRoom(classroom));

        }
        #endregion parents
    }
}
