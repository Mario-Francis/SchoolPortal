using Microsoft.AspNetCore.Mvc;
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
    public class RemarksController : Controller
    {
        private readonly IPerformanceRemarkService remarkService;
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;
        private readonly ILoggerService<RemarksController> logger;

        public RemarksController(
            IPerformanceRemarkService remarkService,
            IOptionsSnapshot<AppSettings> appSettingsDelegate,
            ILoggerService<RemarksController> logger)
        {
            this.remarkService = remarkService;
            this.appSettingsDelegate = appSettingsDelegate;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
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


        [HttpPost]
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

    }
}
