using DataTablesParser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
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
        private readonly IGradeService gradeService;
        private readonly IPerformanceRemarkService remarkService;
        private readonly IPdfGeneratorService pdfGeneratorService;
        private readonly IHealthRecordService healthRecordService;

        public StudentResultsController(
            IStudentService studentService,
            IStudentResultService studentResultService,
            IResultService resultService,
            IOptionsSnapshot<AppSettings> appSettingsDelegate,
            IBehaviouralRatingService behaviouralRatingService,
            ILoggerService<StudentResultsController> logger,
            IGradeService gradeService,
            IPerformanceRemarkService remarkService,
            IPdfGeneratorService pdfGeneratorService,
            IHealthRecordService healthRecordService
            )
        {
            this.studentService = studentService;
            this.studentResultService = studentResultService;
            this.resultService = resultService;
            this.appSettingsDelegate = appSettingsDelegate;
            this.behaviouralRatingService = behaviouralRatingService;
            this.logger = logger;
            this.gradeService = gradeService;
            this.remarkService = remarkService;
            this.pdfGeneratorService = pdfGeneratorService;
            this.healthRecordService = healthRecordService;
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
                return NotFound(new { IsSuccess = true, Message = "Student is not found", ErrorItems = new string[] { } });
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
                    return NotFound(new { IsSuccess = true, Message = "Student is not found", ErrorItems = new string[] { } });
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
                    return NotFound(new { IsSuccess = true, Message = "Student is not found", ErrorItems = new string[] { } });
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

            var results = studentResultService.GetMidTermResults(studentId.Value, session, termId.Value)
                .Select(r => StudentResultItemVM.FromStudentResultItem(r, TermSections.FIRST_HALF, gradeService));

            var parser = new Parser<StudentResultItemVM>(Request.Form, results.AsQueryable());
            var dtResults = StudentResultDataTableResultVM.FromDTResults(parser.Parse());

            dtResults.TotalScoreObtained = Math.Round(results.Select(r => r.Total).Sum(), 0, MidpointRounding.AwayFromZero);
            dtResults.TotalScoreObtainable = 40 * results.Count();
            dtResults.Percentage = Math.Round((dtResults.TotalScoreObtained / dtResults.TotalScoreObtainable) * 100, MidpointRounding.AwayFromZero);
            dtResults.PercentageGrade = gradeService.GetGrade(dtResults.Percentage, TermSections.SECOND_HALF).Code;

            return Ok(dtResults);
        }

        [HttpPost("{studentId}/EndTermResultsDataTable")]
        public IActionResult EndTermResultsDataTable(long? studentId, string session, long? termId)
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var results = studentResultService.GetEndTermResults(studentId.Value, session, termId.Value)
                .Select(r=> StudentResultItemVM.FromStudentResultItem(r, TermSections.SECOND_HALF, gradeService));

            var parser = new Parser<StudentResultItemVM>(Request.Form, results.AsQueryable());
            var dtResults = StudentResultDataTableResultVM.FromDTResults(parser.Parse());

            dtResults.TotalScoreObtained = Math.Round(results.Select(r => r.TermTotal).Sum(), 0, MidpointRounding.AwayFromZero);
            dtResults.TotalScoreObtainable = 100 * results.Count();
            dtResults.Percentage = Math.Round((dtResults.TotalScoreObtained / dtResults.TotalScoreObtainable) * 100, MidpointRounding.AwayFromZero);
            dtResults.PercentageGrade = gradeService.GetGrade(dtResults.Percentage, TermSections.SECOND_HALF).Code;

            return Ok(dtResults);
        }

        [HttpPost("{studentId}/EndOfSessionResultsDataTable")]
        public IActionResult EndOfSessionResultsDataTable(long? studentId, string session)
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var results = studentResultService.GetEndOfSessionResults(studentId.Value, session)
                .Select(r => StudentResultItemVM.FromStudentResultItem(r, TermSections.SECOND_HALF, gradeService));

            var parser = new Parser<StudentResultItemVM>(Request.Form, results.AsQueryable());
            var dtResults = StudentResultDataTableResultVM.FromDTResults(parser.Parse());

            dtResults.TotalScoreObtained = Math.Round(results.Select(r => r.AverageScore).Sum(), 0, MidpointRounding.AwayFromZero);
            dtResults.TotalScoreObtainable = 100 * results.Count();
            dtResults.Percentage = Math.Round((dtResults.TotalScoreObtained / dtResults.TotalScoreObtainable) * 100, MidpointRounding.AwayFromZero);
            dtResults.PercentageGrade = gradeService.GetGrade(dtResults.Percentage, TermSections.SECOND_HALF).Code;

            return Ok(dtResults);
        }

        [HttpPost("{studentId}/EndOfSecondTermResultsDataTable")]
        public IActionResult EndOfSecondTermResultsDataTable(long? studentId, string session)
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var results = studentResultService.GetEndOfSecondTermResults(studentId.Value, session)
                .Select(r => StudentResultItemVM.FromStudentResultItem(r, TermSections.SECOND_HALF, gradeService));

            var parser = new Parser<StudentResultItemVM>(Request.Form, results.AsQueryable());
            var dtResults = StudentResultDataTableResultVM.FromDTResults(parser.Parse());

            dtResults.TotalScoreObtained = Math.Round(results.Select(r => r.AverageScore).Sum(), 0, MidpointRounding.AwayFromZero);
            dtResults.TotalScoreObtainable = 100 * results.Count();
            dtResults.Percentage = Math.Round((dtResults.TotalScoreObtained / dtResults.TotalScoreObtainable) * 100, MidpointRounding.AwayFromZero);
            dtResults.PercentageGrade = gradeService.GetGrade(dtResults.Percentage, TermSections.SECOND_HALF).Code;

            return Ok(dtResults);
        }

        // For viewing
        [HttpGet("{studentId}/View")]
        public async Task<IActionResult> ViewStudentResults(long? studentId)
        {
            if (studentId == null)
            {
                return NotFound(new { IsSuccess = true, Message = "Student is not found", ErrorItems = new string[] { } });
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


        // Export Views
        [AllowAnonymous]
        [HttpGet("{studentId}/MidTermResult/ExportView")]
        public async Task<IActionResult> MidTermResultExportView(long? studentId, string session, long? termId)
        {
            if (studentId == null)
            {
                return NotFound(new { IsSuccess = true, Message = "Student is not found", ErrorItems = new string[] { } });
            }
            var student = await studentService.GetStudent(studentId.Value);
            if (student == null)
            {
                return NotFound("Student is not found");
            }

            var results = studentResultService.GetMidTermResults(studentId.Value, session, termId.Value)
               .Select(r => StudentResultItemVM.FromStudentResultItem(r, TermSections.FIRST_HALF, gradeService));

            var totalScoreObtained = Math.Round(results.Select(r => r.Total).Sum(), 0, MidpointRounding.AwayFromZero);
            var totalScoreObtainable = 40 * results.Count();
            var percentage = Math.Round((totalScoreObtained / totalScoreObtainable) * 100, MidpointRounding.AwayFromZero);
            var percentageGrade = gradeService.GetGrade(percentage, TermSections.SECOND_HALF).Code;

            var midTermResult = await resultService.GetMidTermResult(results.First().Id);
            var classRoom = midTermResult.ClassRoom;
            var exam = midTermResult.Exam;

            var grades = gradeService.GetGrades(TermSections.FIRST_HALF);

            // remark 
            var remark = await remarkService.GetRemark(exam.Id, student.Id);

            var exportViewData = new MidTermResultExportVM
            {
                ResultItems = results,
                Percentage = percentage,
                PercentageGrade = percentageGrade,
                TotalScoreObtained = totalScoreObtained,
                TotalScoreObtainable = totalScoreObtainable,
                Exam = ExamVM.FromExam(exam),
                Student = StudentVM.FromStudent(student),
                ClassRoom=ClassRoomVM.FromClassRoom(classRoom),
                Grades = grades.Select(g => GradeVM.FromGrade(g)),
                HeadTeacherComment = remark?.HeadTeacherRemark,
                TeacherComment = remark?.TeacherRemark
            };

            return View(exportViewData);
        }

        [AllowAnonymous]
        [HttpGet("{studentId}/EndTermResult/ExportView")]
        public async Task<IActionResult> EndTermResultExportView(long? studentId, string session, long? termId)
        {
            if (studentId == null)
            {
                return NotFound(new { IsSuccess = true, Message = "Student is not found", ErrorItems = new string[] { } });
            }
            var student = await studentService.GetStudent(studentId.Value);
            if (student == null)
            {
                return NotFound("Student is not found");
            }

            var results = studentResultService.GetEndTermResults(studentId.Value, session, termId.Value)
               .Select(r => StudentResultItemVM.FromStudentResultItem(r, TermSections.SECOND_HALF, gradeService));

            var totalScoreObtained = Math.Round(results.Select(r => r.TermTotal).Sum(), 0, MidpointRounding.AwayFromZero);
            var totalScoreObtainable = 100 * results.Count();
            var percentage = Math.Round((totalScoreObtained / totalScoreObtainable) * 100, MidpointRounding.AwayFromZero);
            var percentageGrade = gradeService.GetGrade(percentage, TermSections.SECOND_HALF).Code;

            var endTermResult = await resultService.GetEndTermResult(results.First().Id);
            var classRoom = endTermResult.ClassRoom;
            var exam = endTermResult.Exam;

            var grades = gradeService.GetGrades(TermSections.SECOND_HALF);

            // remark 
            var remark = await remarkService.GetRemark(exam.Id, student.Id);

            // behavioural ratings
            var affective = behaviouralRatingService.GetBehaviouralResults(session, termId.Value, student.Id)
                .Where(r => r.BehaviouralRating.Category == BehaviouralRatingCategory.Affective.ToString());

            var psychomotor = behaviouralRatingService.GetBehaviouralResults(session, termId.Value, student.Id)
                .Where(r => r.BehaviouralRating.Category == BehaviouralRatingCategory.Psychomotor.ToString());

            var healthRecord = await healthRecordService.GetRecord(session, termId.Value, student.Id);

            var exportViewData = new EndTermResultExportVM
            {
                ResultItems = results,
                Percentage = percentage,
                PercentageGrade = percentageGrade,
                TotalScoreObtained = totalScoreObtained,
                TotalScoreObtainable = totalScoreObtainable,
                Exam = ExamVM.FromExam(exam),
                Student = StudentVM.FromStudent(student),
                ClassRoom = ClassRoomVM.FromClassRoom(classRoom),
                Grades = grades.Select(g => GradeVM.FromGrade(g)),
                HeadTeacherComment = remark?.HeadTeacherRemark,
                TeacherComment = remark?.TeacherRemark,
                AffectiveDomainBehaviouralRatings = affective.Select(r => BehaviouralResultVM.FromBehaviouralResult(r)),
                PsychoMotorDomainBehaviouralRatings = psychomotor.Select(r => BehaviouralResultVM.FromBehaviouralResult(r)),
                HealthRecord = HealthRecordVM.FromHealthRecord(healthRecord)
            };

            return View(exportViewData);
        }

        [AllowAnonymous]
        [HttpGet("{studentId}/EndSessionResult/ExportView")]
        public async Task<IActionResult> EndSessionResultExportView(long? studentId, string session, long? termId= (int)Terms.THIRD)
        {
            if (studentId == null)
            {
                return NotFound(new { IsSuccess = true, Message = "Student is not found", ErrorItems = new string[] { } });
            }
            var student = await studentService.GetStudent(studentId.Value);
            if (student == null)
            {
                return NotFound("Student is not found");
            }

            var results = studentResultService.GetEndOfSessionResults(studentId.Value, session)
               .Select(r => StudentResultItemVM.FromStudentResultItem(r, TermSections.SECOND_HALF, gradeService));

            var totalScoreObtained = Math.Round(results.Select(r => r.AverageScore).Sum(), 0, MidpointRounding.AwayFromZero);
            var totalScoreObtainable = 100 * results.Count();
            var percentage = Math.Round((totalScoreObtained / totalScoreObtainable) * 100, MidpointRounding.AwayFromZero);
            var percentageGrade = gradeService.GetGrade(percentage, TermSections.SECOND_HALF).Code;

            var endTermResult = await resultService.GetEndTermResult(results.First().Id);
            var classRoom = endTermResult.ClassRoom;
            var exam = endTermResult.Exam;

            var grades = gradeService.GetGrades(TermSections.SECOND_HALF);

            // remark 
            var remark = await remarkService.GetRemark(exam.Id, student.Id);

            // behavioural ratings
            var affective = behaviouralRatingService.GetBehaviouralResults(session, termId.Value, student.Id)
                .Where(r => r.BehaviouralRating.Category == BehaviouralRatingCategory.Affective.ToString());

            var psychomotor = behaviouralRatingService.GetBehaviouralResults(session, termId.Value, student.Id)
                .Where(r => r.BehaviouralRating.Category == BehaviouralRatingCategory.Psychomotor.ToString());

            var healthRecord = await healthRecordService.GetRecord(session, termId.Value, student.Id);

            var exportViewData = new EndTermResultExportVM
            {
                ResultItems = results,
                Percentage = percentage,
                PercentageGrade = percentageGrade,
                TotalScoreObtained = totalScoreObtained,
                TotalScoreObtainable = totalScoreObtainable,
                Exam = ExamVM.FromExam(exam),
                Student = StudentVM.FromStudent(student),
                ClassRoom = ClassRoomVM.FromClassRoom(classRoom),
                Grades = grades.Select(g => GradeVM.FromGrade(g)),
                HeadTeacherComment = remark?.HeadTeacherRemark,
                TeacherComment = remark?.TeacherRemark,
                AffectiveDomainBehaviouralRatings = affective.Select(r => BehaviouralResultVM.FromBehaviouralResult(r)),
                PsychoMotorDomainBehaviouralRatings = psychomotor.Select(r => BehaviouralResultVM.FromBehaviouralResult(r)),
                HealthRecord = HealthRecordVM.FromHealthRecord(healthRecord)
            };

            return View(exportViewData);
        }

        [AllowAnonymous]
        [HttpGet("{studentId}/EndSecondTermResult/ExportView")]
        public async Task<IActionResult> EndSecondTermResultExportView(long? studentId, string session, long? termId = (int)Terms.SECOND)
        {
            if (studentId == null)
            {
                return NotFound(new { IsSuccess = true, Message = "Student is not found", ErrorItems = new string[] { } });
            }
            var student = await studentService.GetStudent(studentId.Value);
            if (student == null)
            {
                return NotFound("Student is not found");
            }

            var results = studentResultService.GetEndOfSecondTermResults(studentId.Value, session)
               .Select(r => StudentResultItemVM.FromStudentResultItem(r, TermSections.SECOND_HALF, gradeService));

            var totalScoreObtained = Math.Round(results.Select(r => r.AverageScore).Sum(), MidpointRounding.AwayFromZero);
            var totalScoreObtainable = 100 * results.Count();
            var percentage = Math.Round((totalScoreObtained / totalScoreObtainable) * 100, MidpointRounding.AwayFromZero);
            var percentageGrade = gradeService.GetGrade(percentage, TermSections.SECOND_HALF).Code;

            var endTermResult = await resultService.GetEndTermResult(results.First().Id);
            var classRoom = endTermResult.ClassRoom;
            var exam = endTermResult.Exam;

            var grades = gradeService.GetGrades(TermSections.SECOND_HALF);

            // remark 
            var remark = await remarkService.GetRemark(exam.Id, student.Id);

            // behavioural ratings
            var affective = behaviouralRatingService.GetBehaviouralResults(session, termId.Value, student.Id)
                .Where(r => r.BehaviouralRating.Category == BehaviouralRatingCategory.Affective.ToString());

            var psychomotor = behaviouralRatingService.GetBehaviouralResults(session, termId.Value, student.Id)
                .Where(r => r.BehaviouralRating.Category == BehaviouralRatingCategory.Psychomotor.ToString());

            var healthRecord = await healthRecordService.GetRecord(session, termId.Value, student.Id);

            var exportViewData = new EndTermResultExportVM
            {
                ResultItems = results,
                Percentage = percentage,
                PercentageGrade = percentageGrade,
                TotalScoreObtained = totalScoreObtained,
                TotalScoreObtainable = totalScoreObtainable,
                Exam = ExamVM.FromExam(exam),
                Student = StudentVM.FromStudent(student),
                ClassRoom = ClassRoomVM.FromClassRoom(classRoom),
                Grades = grades.Select(g => GradeVM.FromGrade(g)),
                HeadTeacherComment = remark?.HeadTeacherRemark,
                TeacherComment = remark?.TeacherRemark,
                AffectiveDomainBehaviouralRatings = affective.Select(r => BehaviouralResultVM.FromBehaviouralResult(r)),
                PsychoMotorDomainBehaviouralRatings = psychomotor.Select(r => BehaviouralResultVM.FromBehaviouralResult(r)),
                HealthRecord = HealthRecordVM.FromHealthRecord(healthRecord)
            };

            return View(exportViewData);
        }


        // Export Mid-term result
        [AllowAnonymous]
        [HttpGet("{studentId}/MidTermResult/Export")]
        public async Task<IActionResult> MidTermResultExport(long? studentId, string session, long? termId)
        {
            if (studentId == null)
            {
                return NotFound(new { IsSuccess = true, Message = "Student is not found", ErrorItems = new string[] { } });
            }
            var student = await studentService.GetStudent(studentId.Value);
            if (student == null)
            {
                return NotFound("Student is not found");
            }

            var url = $"{appSettingsDelegate.Value.LocalBaseUrl}StudentResults/{student.Id}/MidTermResult/ExportView?session={session}&termId={termId}";
            var pdfBuffer = pdfGeneratorService.GeneratePdfFromUrl(url);
            var fileName = $"{student.FirstName} {student.MiddleName} {student.Surname} ({student.AdmissionNo}) Mid-Term Result for {session} {((Terms)termId).ToString()} Term - {DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss")}.pdf".ToLower().Replace(" ", "_");

            return File(pdfBuffer, MimeTypes.GetMimeType(fileName), fileName);
        }

        [AllowAnonymous]
        [HttpGet("{studentId}/EndTermResult/Export")]
        public async Task<IActionResult> EndTermResultExport(long? studentId, string session, long? termId)
        {
            if (studentId == null)
            {
                return NotFound(new { IsSuccess = true, Message = "Student is not found", ErrorItems = new string[] { } });
            }
            var student = await studentService.GetStudent(studentId.Value);
            if (student == null)
            {
                return NotFound("Student is not found");
            }

            var url = $"{appSettingsDelegate.Value.LocalBaseUrl}StudentResults/{student.Id}/EndTermResult/ExportView?session={session}&termId={termId}";
            var pdfBuffer = pdfGeneratorService.GeneratePdfFromUrl(url);
            var fileName = $"{student.FirstName} {student.MiddleName} {student.Surname} ({student.AdmissionNo}) End-Term Result for {session} {((Terms)termId).ToString()} Term - {DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss")}.pdf".ToLower().Replace(" ", "_");

            return File(pdfBuffer, MimeTypes.GetMimeType(fileName), fileName);
        }

        [AllowAnonymous]
        [HttpGet("{studentId}/EndSessionResult/Export")]
        public async Task<IActionResult> EndSessionResultExport(long? studentId, string session, long? termId = (int)Terms.THIRD)
        {
            if (studentId == null)
            {
                return NotFound(new { IsSuccess = true, Message = "Student is not found", ErrorItems = new string[] { } });
            }
            var student = await studentService.GetStudent(studentId.Value);
            if (student == null)
            {
                return NotFound("Student is not found");
            }

            var url = $"{appSettingsDelegate.Value.LocalBaseUrl}StudentResults/{student.Id}/EndSessionResult/ExportView?session={session}&termId={termId}";
            var pdfBuffer = pdfGeneratorService.GeneratePdfFromUrl(url);
            var fileName = $"{student.FirstName} {student.MiddleName} {student.Surname} ({student.AdmissionNo}) End-Session Result for {session} - {DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss")}.pdf".ToLower().Replace(" ", "_");

            return File(pdfBuffer, MimeTypes.GetMimeType(fileName), fileName);
        }

        [AllowAnonymous]
        [HttpGet("{studentId}/EndSecondTermResult/Export")]
        public async Task<IActionResult> EndSecondTermResultExport(long? studentId, string session, long? termId = (int)Terms.SECOND)
        {
            if (studentId == null)
            {
                return NotFound(new { IsSuccess = true, Message = "Student is not found", ErrorItems = new string[] { } });
            }
            var student = await studentService.GetStudent(studentId.Value);
            if (student == null)
            {
                return NotFound("Student is not found");
            }

            var url = $"{appSettingsDelegate.Value.LocalBaseUrl}StudentResults/{student.Id}/EndSecondTermResult/ExportView?session={session}&termId={termId}";
            var pdfBuffer = pdfGeneratorService.GeneratePdfFromUrl(url);
            var fileName = $"{student.FirstName} {student.MiddleName} {student.Surname} ({student.AdmissionNo}) End-Second Term Result for {session} - {DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss")}.pdf".ToLower().Replace(" ", "_");

            return File(pdfBuffer, MimeTypes.GetMimeType(fileName), fileName);
        }

    }
}
