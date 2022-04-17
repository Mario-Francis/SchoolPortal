using DataTablesParser;
using Microsoft.AspNetCore.Http;
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

    public class ResultsController : Controller
    {
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;
        private readonly IResultService resultService;
        private readonly IClassService classService;
        private readonly ILoggerService<ResultsController> logger;
        private readonly IGradeService gradeService;

        public ResultsController(IOptionsSnapshot<AppSettings> appSettingsDelegate,
            IResultService resultService,
            IClassService classService,
            ILoggerService<ResultsController> logger,
            IGradeService gradeService)
        {
            this.appSettingsDelegate = appSettingsDelegate;
            this.resultService = resultService;
            this.classService = classService;
            this.logger = logger;
            this.gradeService = gradeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Mid-term results

        public IActionResult MidTermResults()
        {
            return View();
        }

        [HttpPost("[controller]/midterm/ResultsDataTable")]
        public IActionResult MidTermResultsDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var results = resultService.GetMidTermResultViewObjects().Select(r => MidTermResultViewObjectVM.FromMidTermResultViewObject(r, clientTimeOffset, gradeService));

            var parser = new Parser<MidTermResultViewObjectVM>(Request.Form, results.AsQueryable())
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }
        [HttpPost("[controller]/midterm/BatchUploadResults")]
        public async Task<IActionResult> BatchUploadMidTermResults(BatchUploadResultVM model)
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
                    if (!resultService.ValidateFile(model.File, out List<string> errItems))
                    {
                        return StatusCode(400, new { IsSuccess = false, Message = "Invalid file uploaded.", ErrorItems = errItems });
                    }
                    else
                    {
                        var results = await resultService.ExtractMidTermData(model.File);

                        await resultService.BatchCreateMidTermResults(results, model.ExamId, model.SubjectId, model.ClassId);

                        return Ok(new { IsSuccess = true, Message = "File uploaded and read successfully", ErrorItems = new string[] { } });

                    }
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while adding mid-term results in batch");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpPost("[controller]/midterm/AddResult")]
        public async Task<IActionResult> AddMidTermResult(MidTermResultVM resultVM)
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
                    await resultService.CreateMidTermResult(resultVM.ToMidTermResult());
                    return Ok(new { IsSuccess = true, Message = "Mid-term result added succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while creating a new mid-term result");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }


        [HttpPost("[controller]/midterm/UpdateResult")]
        public async Task<IActionResult> UpdateMidTermResult(MidTermResultVM resultVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errs = ModelState.Values.Where(v => v.Errors.Count > 0).Select(v => v.Errors.First().ErrorMessage);
                    return StatusCode(400, new { IsSuccess = false, Message = "One or more fields failed validation", ErrorItems = errs });
                }
                else if (resultVM.Id == 0)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = $"Invalid result id {resultVM.Id}", ErrorItems = new string[] { } });
                }
                else
                {
                    await resultService.UpdateMidTermResult(resultVM.ToMidTermResult());
                    return Ok(new { IsSuccess = true, Message = "Mid-term result updated succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while updating a mid-term result");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpGet("[controller]/midterm/DeleteResult/{id}")]
        public async Task<IActionResult> DeleteMidTermResult(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Mid-term result is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await resultService.DeleteMidTermResult(id.Value);
                    return Ok(new { IsSuccess = true, Message = "Mid-term result deleted succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while deleting a mid-term result");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }
        [HttpGet("[controller]/midterm/GetResult/{id}")]
        public async Task<IActionResult> GetMidTermResult(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Mmid-term result is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    var result = await resultService.GetMidTermResult(id.Value);
                    return Ok(new { IsSuccess = true, Message = "Result retrieved succeessfully", Data = MidTermResultVM.FromMidTermResult(result) });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while fetching a mid-term result");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        #endregion Mid-term results

        #region classroom mid-term results
        [HttpGet("[controller]/midterm/ClassRoomResults/{classRoomId}")]
        public async Task<IActionResult> ClassRoomMidTermResults(long classRoomId)
        {
            var classRoom = await classService.GetClassRoom(classRoomId);
            if (classRoom == null)
            {
                return NotFound();
            }

            var subjects = classRoom.Class.Subjects.Select(s => SubjectVM.FromSubject(s));
            var students = classRoom.ClassRoomStudents.Select(s => StudentVM.FromStudent(s.Student));
            ViewData["subjects"] = subjects;
            ViewData["students"] = students;

            return View(ClassRoomVM.FromClassRoom(classRoom));
        }

        [HttpPost("[controller]/midterm/ClassRoomResultsDataTable/{classRoomId}")]
        public IActionResult ClassRoomMidTermResultsDataTable(long classRoomId)
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var results = resultService.GetMidTermResultViewObjects().Where(r=> r.ClassRoomId==classRoomId).Select(r => MidTermResultViewObjectVM.FromMidTermResultViewObject(r, clientTimeOffset));

            var parser = new Parser<MidTermResultViewObjectVM>(Request.Form, results.AsQueryable())
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        #endregion classroom mid-term results


        #region End-term results

        public IActionResult EndTermResults()
        {
            return View();
        }

        [HttpPost("[controller]/endterm/ResultsDataTable")]
        public IActionResult EndTermResultsDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var results = resultService.GetEndTermResultViewObjects()
                .Select(r => EndTermResultViewObjectVM.FromEndTermResultViewObject(r, clientTimeOffset, gradeService));

            var parser = new Parser<EndTermResultViewObjectVM>(Request.Form, results.AsQueryable())
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost("[controller]/endterm/BatchUploadResults")]
        public async Task<IActionResult> BatchUploadEndTermResults(BatchUploadResultVM model)
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
                    if (!resultService.ValidateFile(model.File, out List<string> errItems))
                    {
                        return StatusCode(400, new { IsSuccess = false, Message = "Invalid file uploaded.", ErrorItems = errItems });
                    }
                    else
                    {
                        var results = await resultService.ExtractEndTermData(model.File);

                        await resultService.BatchCreateEndTermResults(results, model.ExamId, model.SubjectId, model.ClassId);

                        return Ok(new { IsSuccess = true, Message = "File uploaded and read successfully", ErrorItems = new string[] { } });

                    }
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogError("An error was encountered while adding end-term results in batch");
                logger.LogException(ex);

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpPost("[controller]/endterm/AddResult")]
        public async Task<IActionResult> AddEndTermResult(EndTermResultVM resultVM)
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
                    await resultService.CreateEndTermResult(resultVM.ToEndTermResult());
                    return Ok(new { IsSuccess = true, Message = "End-term result added succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while creating a new end-term result");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpPost("[controller]/endterm/UpdateResult")]
        public async Task<IActionResult> UpdateEndTermResult(EndTermResultVM resultVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errs = ModelState.Values.Where(v => v.Errors.Count > 0).Select(v => v.Errors.First().ErrorMessage);
                    return StatusCode(400, new { IsSuccess = false, Message = "One or more fields failed validation", ErrorItems = errs });
                }
                else if (resultVM.Id == 0)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = $"Invalid result id {resultVM.Id}", ErrorItems = new string[] { } });
                }
                else
                {
                    await resultService.UpdateEndTermResult(resultVM.ToEndTermResult());
                    return Ok(new { IsSuccess = true, Message = "End-term result updated succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while updating a end-term result");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        [HttpGet("[controller]/endterm/DeleteResult/{id}")]
        public async Task<IActionResult> DeleteEndTermResult(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "End-term result is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    await resultService.DeleteEndTermResult(id.Value);
                    return Ok(new { IsSuccess = true, Message = "End-term result deleted succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while deleting a end-term result");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }
        [HttpGet("[controller]/endterm/GetResult/{id}")]
        public async Task<IActionResult> GetEndTermResult(long? id)
        {
            try
            {
                if (id == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Mend-term result is not found", ErrorItems = new string[] { } });
                }
                else
                {
                    var result = await resultService.GetEndTermResult(id.Value);
                    return Ok(new { IsSuccess = true, Message = "Result retrieved succeessfully", Data = EndTermResultVM.FromEndTermResult(result) });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while fetching a end-term result");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        #endregion End-term results


        #region classroom end-term results
        [HttpGet("[controller]/endterm/ClassRoomResults/{classRoomId}")]
        public async Task<IActionResult> ClassRoomEndTermResults(long classRoomId)
        {
            var classRoom = await classService.GetClassRoom(classRoomId);
            if (classRoom == null)
            {
                return NotFound();
            }

            var subjects = classRoom.Class.Subjects.Select(s => SubjectVM.FromSubject(s));
            var students = classRoom.ClassRoomStudents.Select(s => StudentVM.FromStudent(s.Student));
            ViewData["subjects"] = subjects;
            ViewData["students"] = students;

            return View(ClassRoomVM.FromClassRoom(classRoom));
        }

        [HttpPost("[controller]/endterm/ClassRoomResultsDataTable/{classRoomId}")]
        public IActionResult ClassRoomEndTermResultsDataTable(long classRoomId)
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var results = resultService.GetEndTermResultViewObjects().Where(r=>r.ClassRoomId == classRoomId).Select(r => EndTermResultViewObjectVM.FromEndTermResultViewObject(r, clientTimeOffset));

            var parser = new Parser<EndTermResultViewObjectVM>(Request.Form, results.AsQueryable())
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }
        #endregion classroom end-term results
    }
}
