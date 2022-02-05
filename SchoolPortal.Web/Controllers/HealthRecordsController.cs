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
    public class HealthRecordsController : Controller
    {
        private readonly IHealthRecordService recordService;
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;
        private readonly ILoggerService<HealthRecordsController> logger;

        public HealthRecordsController(
            IHealthRecordService recordService,
            IOptionsSnapshot<AppSettings> appSettingsDelegate,
            ILoggerService<HealthRecordsController> logger)
        {
            this.recordService = recordService;
            this.appSettingsDelegate = appSettingsDelegate;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
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


        [HttpPost]
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
    }
}
