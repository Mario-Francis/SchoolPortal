using DataTablesParser;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    [Route("[controller]")]
    public class StudentResultsController : Controller
    {
        private readonly IStudentService studentService;
        private readonly IStudentResultService studentResultService;
        private readonly IResultService resultService;
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;
        private readonly IBehaviouralRatingService behaviouralRatingService;
        private readonly ILoggerService<StudentResultsController> logger;

        public StudentResultsController(
            IStudentService studentService,
            IStudentResultService studentResultService,
            IResultService resultService,
            IOptionsSnapshot<AppSettings> appSettingsDelegate,
            IBehaviouralRatingService behaviouralRatingService,
            ILoggerService<StudentResultsController> logger)
        {
            this.studentService = studentService;
            this.studentResultService = studentResultService;
            this.resultService = resultService;
            this.appSettingsDelegate = appSettingsDelegate;
            this.behaviouralRatingService = behaviouralRatingService;
            this.logger = logger;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpGet("{studentId}")]
        public async Task<IActionResult> StudentResults(long? studentId)
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

            var behaviouralRatings = behaviouralRatingService.GetBehaviouralRatings();
            ViewData["BehaviouralRatings"] = behaviouralRatings;

            return View(StudentVM.FromStudent(student));
        }


        // get session terms
        [HttpGet("{studentId}/GetSessionTerms")]
        public async Task<IActionResult> GetSessionTerms(long? studentId, string session)
        {
            try
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
                if (session == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Session is required", ErrorItems = new string[] { } });
                }
                else
                {
                    var terms = studentResultService.GetResultSessionTerms(studentId.Value, session).Select(t => new ItemVM { Id = t.Id, Name = t.Name });
                    return Ok(new { IsSuccess = true, Message = "Terms retrieved succeessfully", Data = terms });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while fetching session terms");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

        // get student term result
        [HttpGet("{studentId}/GetResult")]
        public async Task<IActionResult> GetResult(long? studentId, string session, long? termId)
        {
            try
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
                if (session == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Session is required", ErrorItems = new string[] { } });
                }

                if (termId == null)
                {
                    return StatusCode(400, new { IsSuccess = false, Message = "Term id is required", ErrorItems = new string[] { } });
                }

                var result = await studentResultService.GetStudentResult(studentId.Value, session, termId.Value);
                    return Ok(new { IsSuccess = true, Message = "Result retrieved succeessfully", Data = StudentResultVM.FromStudentResult(result) });
                
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while fetching student result");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }


        [HttpPost("{studentId}/MidTermResultsDataTable")]
        public IActionResult MidTermResultsDataTable(long? studentId, string session, long? termId)
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var results = studentResultService.GetMidTermResults(studentId.Value, session, termId.Value).Select(r => StudentResultItemVM.FromStudentResultItem(r));

            var parser = new Parser<StudentResultItemVM>(Request.Form, results.AsQueryable());
            var dtResults = StudentResultDataTableResultVM.FromDTResults(parser.Parse());

            dtResults.TotalScoreObtained = results.Select(r => r.Total).Sum();
            dtResults.TotalScoreObtainable = 40 * results.Count();
            dtResults.Percentage = Math.Round(dtResults.TotalScoreObtained / results.Count(), MidpointRounding.AwayFromZero);

            return Ok(dtResults);
        }

        [HttpPost("{studentId}/EndTermResultsDataTable")]
        public IActionResult EndTermResultsDataTable(long? studentId, string session, long? termId)
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var results = studentResultService.GetEndTermResults(studentId.Value, session, termId.Value).Select(r=> StudentResultItemVM.FromStudentResultItem(r));

            var parser = new Parser<StudentResultItemVM>(Request.Form, results.AsQueryable());
            var dtResults = StudentResultDataTableResultVM.FromDTResults(parser.Parse());

            dtResults.TotalScoreObtained = results.Select(r => r.TermTotal).Sum();
            dtResults.TotalScoreObtainable = 100 * results.Count();
            dtResults.Percentage = Math.Round(dtResults.TotalScoreObtained / results.Count(), MidpointRounding.AwayFromZero);

            return Ok(dtResults);
        }

        [HttpPost("{studentId}/EndOfSessionResultsDataTable")]
        public IActionResult EndOfSessionResultsDataTable(long? studentId, string session)
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var results = studentResultService.GetEndOfSessionResults(studentId.Value, session).Select(r => StudentResultItemVM.FromStudentResultItem(r));

            var parser = new Parser<StudentResultItemVM>(Request.Form, results.AsQueryable());
            var dtResults = StudentResultDataTableResultVM.FromDTResults(parser.Parse());

            dtResults.TotalScoreObtained = results.Select(r => r.AverageScore).Sum();
            dtResults.TotalScoreObtainable = 100 * results.Count();
            dtResults.Percentage = Math.Round(dtResults.TotalScoreObtained / results.Count(), MidpointRounding.AwayFromZero);

            return Ok(dtResults);
        }


    }
}
