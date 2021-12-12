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
    public class SubjectsController : Controller
    {
        private readonly AppSettings appSettings;
        private readonly ISubjectService subjectService;
        private readonly IClassService classService;
        private readonly ILogger<SubjectsController> logger;

        public SubjectsController(IOptions<AppSettings> appSettings,
            ISubjectService subjectService,
             IClassService classService,
            ILogger<SubjectsController> logger)
        {
            this.appSettings = appSettings.Value;
            this.subjectService = subjectService;
            this.classService = classService;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubjectsDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettings.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var Subjects = subjectService.GetSubjects().Select(s => SubjectVM.FromSubject(s, clientTimeOffset));

            var parser = new Parser<SubjectVM>(Request.Form, Subjects.AsQueryable())
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost]
        public async Task<IActionResult> AddSubject(SubjectVM SubjectVM)
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
                    if (SubjectVM.ClassId == 0)
                    {
                        throw new AppException($"Class id is required");
                    }

                    await subjectService.CreateSubject(SubjectVM.ToSubject());
                    return Ok(new { IsSuccess = true, Message = "Subject added succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while creating a new subject");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateSubject(SubjectVM SubjectVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errs = ModelState.Values.Where(v => v.Errors.Count > 0).Select(v => v.Errors.First().ErrorMessage);
                    return StatusCode(400, new { IsSuccess = false, Message = "One or more fields failed validation", ErrorItems = errs });
                }
                else if (SubjectVM.Id == 0)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = $"Invalid subject id {SubjectVM.Id}", ErrorItems = new string[] { } });
                }
                else
                {
                    if (SubjectVM.ClassId == 0)
                    {
                        throw new AppException($"Class id is required");
                    }

                    await subjectService.UpdateSubject(SubjectVM.ToSubject());
                    return Ok(new { IsSuccess = true, Message = "Subject updated succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while updating a subject");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateSubjectStatus(long? id, bool isActive)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Subject is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await subjectService.UpdateSubjectStatus(id.Value, isActive);
                    return Ok(new { IsSuccess = true, Message = "Subject status updated succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while updating a subject status");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }


        public async Task<IActionResult> DeleteSubject(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Subject is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await subjectService.DeleteSubject(id.Value);
                    return Ok(new { IsSuccess = true, Message = "Subject deleted succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while deleting a subject");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        public async Task<IActionResult> GetSubject(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Subject is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    var Subject = await subjectService.GetSubject(id.Value);
                    return Ok(new { IsSuccess = true, Message = "Subject retrieved succeessfully", Data = SubjectVM.FromSubject(Subject) });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while fetching a subject");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }
        [HttpGet("[controller]/GetSubjects/{classId}")]
        public async Task<IActionResult> GetSubjects(long? classId)
        {
            try
            {
                if (classId == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Class is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    var @class = await classService.GetClass(classId.Value);
                    if (@class == null)
                    {
                        return StatusCode(400, new { IsSuccess = false, Message = "Class is not found", ErrorItems = new string[] { } });
                    }
                    var subjects = subjectService.GetSubjects(classId.Value).Select(s => SubjectVM.FromSubject(s));
                    return Ok(new { IsSuccess = true, Message = "Subjects retrieved succeessfully", Data = subjects });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error was encountered while fetching class subjects");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

    }
}
