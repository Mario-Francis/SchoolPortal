using DataTablesParser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchoolPortal.Core;
using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Models;
using SchoolPortal.Services;
using SchoolPortal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentService studentService;
        private readonly IClassService classService;
        private readonly AppSettings appSettings;
        private readonly ILogger<StudentsController> logger;

        public StudentsController(
            IStudentService studentService,
            IClassService classService,
            IOptions<AppSettings> appSettings,
            ILogger<StudentsController> logger
            )
        {
            this.studentService = studentService;
            this.classService = classService;
            this.appSettings = appSettings.Value;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult StudentsDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettings.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var Users = studentService.GetStudents().Select(s => StudentVM.FromStudent(s, clientTimeOffset));

            var parser = new Parser<StudentVM>(Request.Form, Users.AsQueryable())
                .SetConverter(x => x.EnrollmentDate, x => x.DateOfBirth.ToString("MMM d, yyyy"))
                .SetConverter(x => x.DateOfBirth, x =>  x.DateOfBirth.ToString("MMM d, yyyy"))
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }
        [HttpPost]
        public IActionResult UnderGraduatesDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettings.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var Users = studentService.GetStudents().Where(s=>!s.IsGraduated).Select(s => StudentVM.FromStudent(s, clientTimeOffset));

            var parser = new Parser<StudentVM>(Request.Form, Users.AsQueryable())
                .SetConverter(x => x.EnrollmentDate, x => x.DateOfBirth.ToString("MMM d, yyyy"))
                .SetConverter(x => x.DateOfBirth, x => x.DateOfBirth.ToString("MMM d, yyyy"))
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost]
        public IActionResult GraduatesDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettings.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var Users = studentService.GetStudents().Where(s => s.IsGraduated).Select(s => StudentVM.FromStudent(s, clientTimeOffset));

            var parser = new Parser<StudentVM>(Request.Form, Users.AsQueryable())
                .SetConverter(x => x.EnrollmentDate, x => x.DateOfBirth.ToString("MMM d, yyyy"))
                .SetConverter(x => x.DateOfBirth, x => x.DateOfBirth.ToString("MMM d, yyyy"))
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpGet("[controller]/{studentId}")]
        public async Task<IActionResult> StudentProfile(long? studentId)
        {
            if (studentId == null)
            {
                return NotFound(new { ISSuccess = true, Message = "Student is not found", ErrorItems = new string[] { } });
            }
            var student = await studentService.GetStudent(studentId.Value);
            if (student == null)
            {
                return NotFound("Student is not found");
            }

            return View(StudentVM.FromStudent(student));
        }

        [HttpGet("[controller]/{studentId}/Guardians")]
        public async Task<IActionResult> Guardians(long? studentId)
        {
            if (studentId == null)
            {
                return NotFound(new { ISSuccess = true, Message = "Student is not found", ErrorItems = new string[] { } });
            }
            var student = await studentService.GetStudent(studentId.Value);
            if (student == null)
            {
                return NotFound("Student is not found");
            }

            return View(StudentVM.FromStudent(student));
        }

        [HttpPost("[controller]/GuardiansDataTable/{studentId}")]
        public async  Task<IActionResult> GuardiansDataTable(long? studentId)
        {
            if (studentId == null)
            {
                return NotFound(new { ISSuccess = true, Message = "Student is not found", ErrorItems = new string[] { } });
            }
            var student = await studentService.GetStudent(studentId.Value);
            if (student == null)
            {
                return NotFound(new { ISSuccess = true, Message = "Student is not found", ErrorItems = new string[] { } });
            }
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettings.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var guardians = student.StudentGuardians.Select(g => GuardianVM.FromStudentGuardian(g, clientTimeOffset));

            var parser = new Parser<GuardianVM>(Request.Form, guardians.AsQueryable());

            return Ok(parser.Parse());
        }



        [HttpPost]
        public async Task<IActionResult> AddStudent(StudentVM studentVM)
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
                    var student = studentVM.ToStudent();
                    long classRoomId = 0;
                    if (studentVM.ClassId != null)
                    {
                        if (studentVM.ClassRoomId != null)
                        {
                            classRoomId = studentVM.ClassRoomId.Value;
                        }
                        else
                        {
                            var _class = await classService.GetClass(studentVM.ClassId.Value);
                            classRoomId = _class.ClassRooms.FirstOrDefault().Id;
                        }
                    }
                    else
                    {
                        var _class = await classService.GetClass(studentVM.EntryClassId);
                        classRoomId = _class.ClassRooms.FirstOrDefault().Id;
                    }
                    student.ClassRoomStudents = new List<ClassRoomStudent>
                    {
                        new ClassRoomStudent
                        {
                            ClassRoomId = classRoomId
                        }
                    };

                    await studentService.CreateStudent(student);
                    return Ok(new { IsSuccess = true, Message = "Student added succeessfully and credentials sent to student via mail", ErrorItems = new string[] { } });
                    
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while creating a new student");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddGuardian(GuardianVM guardianVM)
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
                    var guardian = guardianVM.ToStudentGuardian();

                    await studentService.AddStudentGuardian(guardian.StudentId, guardian.GuardianId, guardian.RelationshipId);
                    return Ok(new { IsSuccess = true, Message = "Guardian added succeessfully", ErrorItems = new string[] { } });

                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while creating a new guardian");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateStudent(StudentVM studentVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errs = ModelState.Values.Where(v => v.Errors.Count > 0).Select(v => v.Errors.First().ErrorMessage);
                    return StatusCode(400, new { IsSuccess = false, Message = "One or more fields failed validation", ErrorItems = errs });
                }
                else if (studentVM.Id == 0)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = $"Invalid student id {studentVM.Id}", ErrorItems = new string[] { } });
                }
                else
                {
                    var student = studentVM.ToStudent();
                    long classRoomId = 0;
                    if (studentVM.ClassId != null)
                    {
                        if (studentVM.ClassRoomId != null)
                        {
                            classRoomId = studentVM.ClassRoomId.Value;
                        }
                        else
                        {
                            var _class = await classService.GetClass(studentVM.ClassId.Value);
                            classRoomId = _class.ClassRooms.FirstOrDefault().Id;
                        }
                    }
                    else
                    {
                        var _class = await classService.GetClass(studentVM.EntryClassId);
                        classRoomId = _class.ClassRooms.FirstOrDefault().Id;
                    }
                    student.ClassRoomStudents = new List<ClassRoomStudent>
                    {
                        new ClassRoomStudent
                        {
                            ClassRoomId = classRoomId
                        }
                    };
                    await studentService.UpdateStudent(student);
                    return Ok(new { IsSuccess = true, Message = "Student updated succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while updating a student");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }


        public async Task<IActionResult> DeleteStudent(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Student is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await studentService.DeleteStudent(id.Value);
                    return Ok(new { IsSuccess = true, Message = "Student deleted succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while deleting a student");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }
        public async Task<IActionResult> RemoveGuardian(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Guardian is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await studentService.RemoveStudentGuardian(id.Value);
                    return Ok(new { IsSuccess = true, Message = "Guardian removed succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while removing guardian");
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
                    return StatusCode(400, new { IsSuccess = false, Message = "Student is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await studentService.ResetPassword(id.Value);
                    return Ok(new { IsSuccess = true, Message = "Student password reset succeessfully and new password sent via mail", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while reseting a student's password");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        public async Task<IActionResult> GetStudent(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Student is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    var Student = await studentService.GetStudent(id.Value);
                    return Ok(new { IsSuccess = true, Message = "Student retrieved succeessfully", Data = StudentVM.FromStudent(Student) });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while fetching a student");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStudentStatus(long? id, bool isActive)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Student is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await studentService.UpdateStudentStatus(id.Value, isActive);
                    return Ok(new { IsSuccess = true, Message = "Student status updated succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while updating a student status");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGraduationStatus(long? id, bool isGraduated)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Student is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await studentService.UpdateStudentGraduationStatus(id.Value, isGraduated);
                    return Ok(new { IsSuccess = true, Message = "Student graduation status updated succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while updating a student status");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BatchAddStudents(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "No file uploaded!", ErrorItems = new string[] { } });
                }
                else
                {
                    if (!studentService.ValidateFile(file, out List<string> errItems))
                    {
                        return StatusCode(400, new { IsSuccess = false, Message = "Invalid file uploaded.", ErrorItems = errItems });
                    }
                    else
                    {
                        var students = await studentService.ExtractData(file);

                        await studentService.BatchCreateStudent(students);

                        return Ok(new { IsSuccess = true, Message = "File uploaded and read and students created successfully", ErrorItems = new string[] { } });

                    }
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while adding students in batch");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpGet("[controller]/SearchStudents")]
        public IActionResult SearchStudents([FromQuery] string query, [FromQuery] int max = 100)
        {
            try
            {
                var students = studentService.SearchStudents(query, max).Select(s => StudentItemVM.FromStudent(s));
                return Ok(new { IsSuccess = true, Message = "Success", Data = students });

            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error was encountered while searching students");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

    }
}
