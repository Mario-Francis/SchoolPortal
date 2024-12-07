using DataTablesParser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
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
    public class BehaviouralRatingsController : Controller
    {
        private readonly IBehaviouralRatingService ratingService;
        private readonly IClassService classService;
        private readonly IStudentService studentService;
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;
        private readonly ILoggerService<BehaviouralRatingsController> logger;

       
        public BehaviouralRatingsController(
            IBehaviouralRatingService ratingService,
            IClassService classService,
            IStudentService studentService,
            IOptionsSnapshot<AppSettings> appSettingsDelegate,
            ILoggerService<BehaviouralRatingsController> logger)
        {
            this.ratingService = ratingService;
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
        [HttpPost("RatingsDataTable")]
        public IActionResult RatingsDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var results = ratingService.GetBehaviouralResults().Select(r => BehaviouralResultVM.FromBehaviouralResult(r, clientTimeOffset));

            var parser = new Parser<BehaviouralResultVM>(Request.Form, results.AsQueryable())
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost("BatchUploadRatings")]
        public async Task<IActionResult> BatchUploadRatings(BatchUploadRatingVM model)
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
                        var results = await ratingService.ExtractData(model.File);

                        await ratingService.BatchCreateBehaviouralResults(results, model.Session, model.TermId);

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


        [HttpPost("AddBehaviouralResult")]
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

        [HttpPost("AddBehaviouralResults")]
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

        [HttpPost("UpdateBehaviouralResult")]
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

        [HttpPost("UpdateBehaviouralResults")]
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
        [HttpGet("DeleteBehaviouralResult/{id}")]
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
        [HttpGet("DeleteBehaviouralResults")]
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

        [HttpGet("DownloadBehaviouralResultsTemplate")]
        public IActionResult DownloadBehaviouralResultsTemplate()
        {
            try
            {
                var file = ratingService.GenerateBatchUploadTemaplate();
                var fileName = "Batch Upload Behavioural Results Template.xlsx";
                new FileExtensionContentTypeProvider().TryGetContentType(fileName, out string contentType);

                return File(file, contentType, fileName);
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
                logger.LogError("An error was encountered while generating behavioural results template");

                return StatusCode(500, new
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorDetail = ex.GetErrorDetails()
                });
            }
        }



        #region Classroom behavioural ratings
        [Authorize(Roles = "Teacher")]
        [HttpGet("ClassRoomBehaviouralRatings/{classRoomId}")]
        public async Task<IActionResult> ClassRoomBehaviouralRatings(long classRoomId)
        {
            var classRoom = await classService.GetClassRoom(classRoomId);
            if (classRoom == null)
            {
                return NotFound();
            }

            return View(ClassRoomVM.FromClassRoom(classRoom));
        }

        [HttpPost("ClassRoomRatingsDataTable/{classRoomId}")]
        public IActionResult ClassRoomRatingsDataTable(long classRoomId)
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var results = ratingService.GetBehaviouralResults().Where(r=>r.Student.ClassRoomStudents.FirstOrDefault()?.ClassRoomId == classRoomId)
                .Select(r => BehaviouralResultVM.FromBehaviouralResult(r, clientTimeOffset));

            var parser = new Parser<BehaviouralResultVM>(Request.Form, results.AsQueryable())
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost("BatchUploadClassRoomRatings/{classRoomId}")]
        public async Task<IActionResult> BatchUploadClassRoomRatings(BatchUploadRatingVM model, long classRoomId)
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
                        var results = await ratingService.ExtractData(model.File);

                        foreach(var r in results)
                        {
                            var student = await studentService.GetStudent(r.First().StudentId);
                            if (student.ClassRoomStudents.FirstOrDefault()?.ClassRoomId != classRoomId)
                            {
                                throw new AppException($"Studet with admission number '{student.AdmissionNo}' does not exist in your classroom");
                            }
                        }

                        await ratingService.BatchCreateBehaviouralResults(results, model.Session, model.TermId);

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


        #endregion Classroom behavioural ratings

    }
}
