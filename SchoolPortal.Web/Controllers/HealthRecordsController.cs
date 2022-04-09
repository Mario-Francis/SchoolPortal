using DataTablesParser;
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
using System.Text.Json;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Controllers
{
    [Route("[controller]")]
    public class HealthRecordsController : Controller
    {
        private readonly IHealthRecordService recordService;
        private readonly IClassService classService;
        private readonly IStudentService studentService;
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;
        private readonly ILoggerService<HealthRecordsController> logger;

        public HealthRecordsController(
            IHealthRecordService recordService,
            IClassService classService,
            IStudentService studentService,
            IOptionsSnapshot<AppSettings> appSettingsDelegate,
            ILoggerService<HealthRecordsController> logger)
        {
            this.recordService = recordService;
            this.classService = classService;
            this.studentService = studentService;
            this.appSettingsDelegate = appSettingsDelegate;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("RecordsDataTable")]
        public IActionResult RecordsDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var records = recordService.GetRecords().Select(r => HealthRecordVM.FromHealthRecord(r, clientTimeOffset));

            var parser = new Parser<HealthRecordVM>(Request.Form, records.AsQueryable())
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost("BatchUploadRecords")]
        public async Task<IActionResult> BatchUploadRecords(BatchUploadHealthRecordVM model)
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

                        await recordService.BatchCreateRecords(records, model.Session, model.TermId);

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
                logger.LogError("An error was encountered while adding health records in batch");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }

        [HttpPost("AddRecord")]
        public async Task<IActionResult> AddRecord(HealthRecordVM recordVM)
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

                    await recordService.CreateRecord(recordVM.ToHealthRecord());
                    return Ok(new { IsSuccess = true, Message = "Record added succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while adding record");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpPost("UpdateRecord")]
        public async Task<IActionResult> UpdateRecord(HealthRecordVM recordVM)
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

                    await recordService.UpdateRecord(recordVM.ToHealthRecord());
                    return Ok(new { IsSuccess = true, Message = "Record updated succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while updating record");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
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
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while deleting record");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
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
                        Data = HealthRecordVM.FromHealthRecord(record)
                    });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorDetail = JsonSerializer.Serialize(ex.InnerException)
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
                    ErrorDetail = JsonSerializer.Serialize(ex.InnerException)
                });
            }
        }

        [HttpGet("GetRecord")]
        public async Task<IActionResult> GetRecord(string session, long? termId, long? studentId)
        {
            try
            {
                if (session == null || termId==null || studentId == null)
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
                        Data = HealthRecordVM.FromHealthRecord(record)
                    });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorDetail = JsonSerializer.Serialize(ex.InnerException)
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
                    ErrorDetail = JsonSerializer.Serialize(ex.InnerException)
                });
            }
        }

        #region Classroom health records

        [HttpGet("ClassRoomHealthRecords/{classRoomId}")]
        public async Task<IActionResult> ClassRoomHealthRecords(long classRoomId)
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

            var records = recordService.GetRecords().Where(r=>r.Student.ClassRoomStudents.FirstOrDefault()?.ClassRoomId==classRoomId)
                .Select(r => HealthRecordVM.FromHealthRecord(r, clientTimeOffset));

            var parser = new Parser<HealthRecordVM>(Request.Form, records.AsQueryable())
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost("ClassRoomBatchUploadRecords/{classRoomId}")]
        public async Task<IActionResult> BatchUploadRecords(BatchUploadHealthRecordVM model, long classRoomId)
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

                        await recordService.BatchCreateRecords(records, model.Session, model.TermId);

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
                logger.LogError("An error was encountered while adding health records in batch");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }


        #endregion Classroom health records
    }
}
