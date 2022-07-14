using DataTablesParser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SchoolPortal.Core;
using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Extensions;
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
    public class AttendanceRecordsController : Controller
    {
        private readonly IAttendanceRecordService recordService;
        private readonly IClassService classService;
        private readonly IStudentService studentService;
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;
        private readonly ILoggerService<AttendanceRecordsController> logger;

        public AttendanceRecordsController(
            IAttendanceRecordService recordService,
            IClassService classService,
            IStudentService studentService,
            IOptionsSnapshot<AppSettings> appSettingsDelegate,
            ILoggerService<AttendanceRecordsController> logger)
        {
            this.recordService = recordService;
            this.classService = classService;
            this.studentService = studentService;
            this.appSettingsDelegate = appSettingsDelegate;
            this.logger = logger;
        }

        [Authorize(Roles = "Administrator, Head Teacher")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("RecordsDataTable")]
        public IActionResult RecordsDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var records = recordService.GetRecords().Select(r => AttendanceRecordVM.FromAttendanceRecord(r, clientTimeOffset));

            var parser = new Parser<AttendanceRecordVM>(Request.Form, records.AsQueryable())
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost("BatchUploadRecords")]
        public async Task<IActionResult> BatchUploadRecords(BatchUploadAttendanceRecordVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errs = ModelState.Values.Where(v => v.Errors.Count > 0).Select(v => v.Errors.First().ErrorMessage);
                    return StatusCode(400, new { IsSuccess = false, Message = "One or more fields failed validation", ErrorItems = errs });
                }
                else if (model.File == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "No file uploaded!", ErrorItems = new string[] { } });
                }
                else
                {
                    if (!AppUtilities.ValidateFile(model.File, out List<string> errItems, appSettingsDelegate.Value.MaxUploadSize))
                    {
                        return StatusCode(400, new { IsSuccess = false, Message = "Invalid file uploaded.", ErrorItems = errItems });
                    }
                    else
                    {
                        var records = await recordService.ExtractData(model.File);

                        await recordService.BatchCreateRecords(records, model.Session, model.TermId, model.SchoolOpenCount);

                        return Ok(new { IsSuccess = true, Message = "File uploaded and read successfully", ErrorItems = new string[] { } });

                    }
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while adding attendance records in batch");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }

        [HttpPost("AddRecord")]
        public async Task<IActionResult> AddRecord(AttendanceRecordVM recordVM)
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
                    if (recordVM.TermId == 0)
                    {
                        throw new AppException($"Term id is required");
                    }
                    if (recordVM.StudentId == 0)
                    {
                        throw new AppException($"Student id is required");
                    }

                    await recordService.CreateRecord(recordVM.ToAttendanceRecord());
                    return Ok(new { IsSuccess = true, Message = "Record added succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while adding record");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }

        [HttpPost("UpdateRecord")]
        public async Task<IActionResult> UpdateRecord(AttendanceRecordVM recordVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errs = ModelState.Values.Where(v => v.Errors.Count > 0).Select(v => v.Errors.First().ErrorMessage);
                    return StatusCode(400, new { IsSuccess = false, Message = "One or more fields failed validation", ErrorItems = errs });
                }
                else if (recordVM.Id == 0)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = $"Invalid record id {recordVM.Id}", ErrorItems = new string[] { } });
                }
                else
                {
                    if (recordVM.TermId == 0)
                    {
                        throw new AppException($"Term id is required");
                    }

                    if (recordVM.StudentId == 0)
                    {
                        throw new AppException($"Student id is required");
                    }

                    await recordService.UpdateRecord(recordVM.ToAttendanceRecord());
                    return Ok(new { IsSuccess = true, Message = "Record updated succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while updating record");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }
        [HttpGet("DeleteRecord/{id}")]
        public async Task<IActionResult> DeleteRecord(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Record is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await recordService.DeleteRecord(id.Value);
                    return Ok(new { IsSuccess = true, Message = "Record deleted succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while deleting record");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }

        [HttpGet("GetRecord/{id}")]
        public async Task<IActionResult> GetRecord(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new
                    {
                        IsSuccess = false,
                        Message = "Record is not found",
                        ErrorItems = new string[] { }
                    });
                }
                else
                {
                    var record = await recordService.GetRecord(id.Value);
                    return Ok(new
                    {
                        IsSuccess = true,
                        Message = "Record retrieved succeessfully",
                        Data = AttendanceRecordVM.FromAttendanceRecord(record)
                    });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorDetail = ex.GetErrorDetails()
                });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while fetching record");

                return StatusCode(500, new
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorDetail = ex.GetErrorDetails()
                });
            }
        }

        [HttpGet("GetRecord")]
        public async Task<IActionResult> GetRecord(string session, long? termId, long? studentId)
        {
            try
            {
                if (session == null || termId == null || studentId == null)
                {
                    return StatusCode(400, new
                    {
                        IsSuccess = false,
                        Message = "Record is not found",
                        ErrorItems = new string[] { }
                    });
                }
                else
                {
                    var record = await recordService.GetRecord(session, termId.Value, studentId.Value);
                    return Ok(new
                    {
                        IsSuccess = true,
                        Message = "Record retrieved succeessfully",
                        Data = AttendanceRecordVM.FromAttendanceRecord(record)
                    });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorDetail = ex.GetErrorDetails()
                });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while fetching record");

                return StatusCode(500, new
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorDetail = ex.GetErrorDetails()
                });
            }
        }

        #region Classroom attendance records
        [Authorize(Roles = "Teacher")]
        [HttpGet("ClassRoomAttendanceRecords/{classRoomId}")]
        public async Task<IActionResult> ClassRoomAttendanceRecords(long classRoomId)
        {
            var classRoom = await classService.GetClassRoom(classRoomId);
            if (classRoom == null)
            {
                return NotFound();
            }

            return View(ClassRoomVM.FromClassRoom(classRoom));
        }

        [HttpPost("ClassRoomRecordsDataTable/{classRoomId}")]
        public IActionResult ClassRoomRecordsDataTable(long classRoomId)
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var records = recordService.GetRecords().Where(r => r.Student.ClassRoomStudents.FirstOrDefault()?.ClassRoomId == classRoomId)
                .Select(r => AttendanceRecordVM.FromAttendanceRecord(r, clientTimeOffset));

            var parser = new Parser<AttendanceRecordVM>(Request.Form, records.AsQueryable())
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost("ClassRoomBatchUploadRecords/{classRoomId}")]
        public async Task<IActionResult> BatchUploadRecords(BatchUploadAttendanceRecordVM model, long classRoomId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errs = ModelState.Values.Where(v => v.Errors.Count > 0).Select(v => v.Errors.First().ErrorMessage);
                    return StatusCode(400, new { IsSuccess = false, Message = "One or more fields failed validation", ErrorItems = errs });
                }
                else if (model.File == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "No file uploaded!", ErrorItems = new string[] { } });
                }
                else
                {
                    if (!AppUtilities.ValidateFile(model.File, out List<string> errItems, appSettingsDelegate.Value.MaxUploadSize))
                    {
                        return StatusCode(400, new { IsSuccess = false, Message = "Invalid file uploaded.", ErrorItems = errItems });
                    }
                    else
                    {
                        var records = await recordService.ExtractData(model.File);

                        foreach (var r in records)
                        {
                            var student = await studentService.GetStudent(r.StudentId);
                            if (student.ClassRoomStudents.FirstOrDefault()?.ClassRoomId != classRoomId)
                            {
                                throw new AppException($"Studet with admission number '{student.AdmissionNo}' does not exist in your classroom");
                            }
                        }

                        await recordService.BatchCreateRecords(records, model.Session, model.TermId, model.SchoolOpenCount);

                        return Ok(new { IsSuccess = true, Message = "File uploaded and read successfully", ErrorItems = new string[] { } });

                    }
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while adding attendance records in batch");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }


        #endregion Classroom attendance records
    }
}
