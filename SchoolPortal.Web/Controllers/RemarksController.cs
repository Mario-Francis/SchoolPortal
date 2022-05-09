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
using System.Text.Json;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class RemarksController : Controller
    {
        private readonly IPerformanceRemarkService remarkService;
        private readonly IClassService classService;
        private readonly IStudentService studentService;
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;
        private readonly ILoggerService<RemarksController> logger;

        public RemarksController(
            IPerformanceRemarkService remarkService,
            IClassService classService,
            IStudentService studentService,
            IOptionsSnapshot<AppSettings> appSettingsDelegate,
            ILoggerService<RemarksController> logger)
        {
            this.remarkService = remarkService;
            this.classService = classService;
            this.studentService = studentService;
            this.appSettingsDelegate = appSettingsDelegate;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("RemarksDataTable")]
        public IActionResult RemarksDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var remarks = remarkService.GetRemarks().Select(r => PerformanceRemarkVM.FromPerformanceRemark(r, clientTimeOffset));

            var parser = new Parser<PerformanceRemarkVM>(Request.Form, remarks.AsQueryable())
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost("BatchUploadRemarks")]
        public async Task<IActionResult> BatchUploadRemarks(BatchUploadRemarkVM model)
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
                        var remarks = await remarkService.ExtractData(model.File);

                        await remarkService.BatchCreateRemarks(remarks, model.ExamId);

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
                logger.LogError("An error was encountered while adding behavioural results in batch");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }


        [HttpPost("AddRemark")]
        public async Task<IActionResult> AddRemark(PerformanceRemarkVM remarkVM)
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
                    if (remarkVM.ExamId == 0)
                    {
                        throw new AppException($"Exam id is required");
                    }
                    if (remarkVM.StudentId == 0)
                    {
                        throw new AppException($"Student id is required");
                    }

                    await remarkService.CreateRemark(remarkVM.ToPerformanceRemark());
                    return Ok(new { IsSuccess = true, Message = "Remark added succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while adding remark");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }


        [HttpPost("UpdateRemark")]
        public async Task<IActionResult> UpdateRemark(PerformanceRemarkVM remarkVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errs = ModelState.Values.Where(v => v.Errors.Count > 0).Select(v => v.Errors.First().ErrorMessage);
                    return StatusCode(400, new { IsSuccess = false, Message = "One or more fields failed validation", ErrorItems = errs });
                }
                else if (remarkVM.Id == 0)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = $"Invalid remark id {remarkVM.Id}", ErrorItems = new string[] { } });
                }
                else
                {
                    if (remarkVM.ExamId== 0)
                    {
                        throw new AppException($"Exam id is required");
                    }

                    if (remarkVM.StudentId == 0)
                    {
                        throw new AppException($"Student id is required");
                    }

                    await remarkService.UpdateRemark(remarkVM.ToPerformanceRemark());
                    return Ok(new { IsSuccess = true, Message = "Remark updated succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while updating remark");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpGet("DeleteRemark/{id}")]
        public async Task<IActionResult> DeleteRemark(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Remark is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await remarkService.DeleteRemark(id.Value);
                    return Ok(new { IsSuccess = true, Message = "Remark deleted succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while deleting remark");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpGet("GetRemark/{id}")]
        public async Task<IActionResult> GetRemark(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, 
                        Message = "Remark is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    var remark = await remarkService.GetRemark(id.Value);
                    return Ok(new { IsSuccess = true, Message = "Remark retrieved succeessfully", 
                        Data = PerformanceRemarkVM.FromPerformanceRemark(remark) });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, 
                    ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while fetching remark");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, 
                    ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpGet("GetRemark")]
        public async Task<IActionResult> GetRemark(long? examId, long? studentId)
        {
            try
            {
                if (examId == null || studentId==null)
                {
                    return StatusCode(400, new
                    {
                        IsSuccess = false,
                        Message = "Remark is not found",
                        ErrorItems = new string[] { }
                    });
                }
                else
                {
                    var remark = await remarkService.GetRemark(examId.Value, studentId.Value);
                    return Ok(new
                    {
                        IsSuccess = true,
                        Message = "Remark retrieved succeessfully",
                        Data = PerformanceRemarkVM.FromPerformanceRemark(remark)
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
                logger.LogError("An error was encountered while fetching remark");

                return StatusCode(500, new
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorDetail = JsonSerializer.Serialize(ex.InnerException)
                });
            }
        }

        #region Classroom remarks
        [HttpGet("ClassRoomRemarks/{classRoomId}")]
        public async Task<IActionResult> ClassRoomRemarks(long classRoomId)
        {
            var classRoom = await classService.GetClassRoom(classRoomId);
            if (classRoom == null)
            {
                return NotFound();
            }

            return View(ClassRoomVM.FromClassRoom(classRoom));
        }

        [HttpPost("ClassRoomRemarksDataTable/{classRoomId}")]
        public IActionResult ClassRoomRemarksDataTable(long classRoomId)
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var remarks = remarkService.GetRemarks()
                .Where(r=>r.Student.ClassRoomStudents.FirstOrDefault()?.ClassRoomId==classRoomId)
                .Select(r => PerformanceRemarkVM.FromPerformanceRemark(r, clientTimeOffset));

            var parser = new Parser<PerformanceRemarkVM>(Request.Form, remarks.AsQueryable())
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost("ClassRoomBatchUploadRemarks/{classRoomId}")]
        public async Task<IActionResult> ClassRoomBatchUploadRemarks(BatchUploadRemarkVM model, long classRoomId)
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
                        var remarks = await remarkService.ExtractData(model.File);

                        foreach (var r in remarks)
                        {
                            var student = await studentService.GetStudent(r.StudentId);
                            if (student.ClassRoomStudents.FirstOrDefault()?.ClassRoomId != classRoomId)
                            {
                                throw new AppException($"Student with admission number '{student.AdmissionNo}' does not exist in your classroom");
                            }
                        }

                        await remarkService.BatchCreateRemarks(remarks, model.ExamId);

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
                logger.LogError("An error was encountered while adding behavioural results in batch");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }

        #endregion Classroom remarks
    }
}
