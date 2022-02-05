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
    public class BehaviouralRatingsController : Controller
    {
        private readonly IBehaviouralRatingService ratingService;
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;
        private readonly ILoggerService<BehaviouralRatingsController> logger;

        public BehaviouralRatingsController(
            IBehaviouralRatingService ratingService,
            IOptionsSnapshot<AppSettings> appSettingsDelegate,
            ILoggerService<BehaviouralRatingsController> logger)
        {
            this.ratingService = ratingService;
            this.appSettingsDelegate = appSettingsDelegate;
            this.logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBehaviouralResult(BehaviouralResultVM behaviouralResultVM)
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
                    if (behaviouralResultVM.TermId == 0)
                    {
                        throw new AppException($"Term id is required");
                    }
                    if (behaviouralResultVM.StudentId == 0)
                    {
                        throw new AppException($"Student id is required");
                    }

                    await ratingService.CreateBehaviouralResult(behaviouralResultVM.ToBehaviouralResult());
                    return Ok(new { IsSuccess = true, Message = "Behavioural result added succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while adding behavioural result");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddBehaviouralResults(BatchRatingVM model)
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
                    var ratings = model.Ratings.Select(r => (r.RatingId, r.Score));
                    await ratingService.CreateBehaviouralResults(model.Session, model.TermId.Value, model.StudentId.Value, ratings);
                    return Ok(new { IsSuccess = true, Message = "Behavioural results added succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while adding behavioural results in batch");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBehaviouralResult(BehaviouralResultVM behaviouralResultVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errs = ModelState.Values.Where(v => v.Errors.Count > 0).Select(v => v.Errors.First().ErrorMessage);
                    return StatusCode(400, new { IsSuccess = false, Message = "One or more fields failed validation", ErrorItems = errs });
                }
                else if (behaviouralResultVM.Id == 0)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = $"Invalid behavioural result id {behaviouralResultVM.Id}", ErrorItems = new string[] { } });
                }
                else
                {
                    if (behaviouralResultVM.TermId == 0)
                    {
                        throw new AppException($"Term id is required");
                    }

                    if (behaviouralResultVM.StudentId == 0)
                    {
                        throw new AppException($"Student id is required");
                    }

                    await ratingService.UpdateBehaviouralResult(behaviouralResultVM.ToBehaviouralResult());
                    return Ok(new { IsSuccess = true, Message = "Behavioural result updated succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while updating behavioural result");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBehaviouralResults(BatchRatingVM model)
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
                    var ratings = model.Ratings.Select(r => (r.Id, r.RatingId, r.Score));
                    await ratingService.UpdateBehaviouralResults(model.Session, model.TermId.Value, model.StudentId.Value, ratings);
                    return Ok(new { IsSuccess = true, Message = "Behavioural results updated succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while updating behavioural results in batch");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }
        public async Task<IActionResult> DeleteBehaviouralResult(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Behavioural result is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await ratingService.DeleteBehaviouralResult(id.Value);
                    return Ok(new { IsSuccess = true, Message = "Behavioural result deleted succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while deleting behavioural result");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        public async Task<IActionResult> DeleteBehaviouralResults(string session, long? termId, long? studentId)
        {
            try
            {
                if (session == null || termId == null || studentId == null)
                {
                    return StatusCode(400, new
                    {
                        IsSuccess = false,
                        Message = "Behavioural result is not found",
                        ErrorItems = new string[] { }
                    });
                }
                else
                {
                    await ratingService.DeleteBehaviouralResults(session, termId.Value, studentId.Value);
                    return Ok(new { IsSuccess = true, Message = "Behavioural results deleted succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while deleting behavioural results");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpGet("GetBehaviouralResult/{id}")]
        public async Task<IActionResult> GetBehaviouralResult(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new
                    {
                        IsSuccess = false,
                        Message = "Behavioural result is not found",
                        ErrorItems = new string[] { }
                    });
                }
                else
                {
                    var behaviouralResult = await ratingService.GetBehaviouralResult(id.Value);
                    return Ok(new
                    {
                        IsSuccess = true,
                        Message = "Behavioural result retrieved succeessfully",
                        Data = BehaviouralResultVM.FromBehaviouralResult(behaviouralResult)
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
                logger.LogError("An error was encountered while fetching behavioural result");

                return StatusCode(500, new
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorDetail = JsonSerializer.Serialize(ex.InnerException)
                });
            }
        }

        [HttpGet("GetBehaviouralResults")]
        public async Task<IActionResult> GetBehaviouralResults(string session, long? termId, long? studentId)
        {
            try
            {
                if (session == null || termId==null || studentId == null)
                {
                    return StatusCode(400, new
                    {
                        IsSuccess = false,
                        Message = "Behavioural result is not found",
                        ErrorItems = new string[] { }
                    });
                }
                else
                {
                    await Task.Delay(0);
                    var behaviouralResults = ratingService.GetBehaviouralResults(session, termId.Value, studentId.Value).Select(r => BehaviouralResultVM.FromBehaviouralResult(r));
                    return Ok(new
                    {
                        IsSuccess = true,
                        Message = "Behavioural results retrieved succeessfully",
                        Data = behaviouralResults
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
                logger.LogError("An error was encountered while fetching behavioural results");

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
