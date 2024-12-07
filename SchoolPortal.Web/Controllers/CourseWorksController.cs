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
    public class CourseWorksController : Controller
    {
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;
        private readonly ICourseWorkService courseWorkService;
        private readonly ILoggerService<CourseWorksController> logger;

        public CourseWorksController(
            IOptionsSnapshot<AppSettings> appSettingsDelegate,
            ICourseWorkService courseWorkService,
            ILoggerService<CourseWorksController> logger)
        {
            this.appSettingsDelegate = appSettingsDelegate;
            this.courseWorkService = courseWorkService;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("CourseWorksDataTable")]
        public IActionResult CourseWorksDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var courseWorks = courseWorkService.GetCourseWorks().Select(c => CourseWorkVM.FromCourseWork(c, clientTimeOffset));

            var parser = new Parser<CourseWorkVM>(Request.Form, courseWorks.AsQueryable())
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.From, x => x.From.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.To, x => x.To.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost("ClassRoomCourseWorksDataTable/{classRoomId}")]
        public IActionResult ClassRoomCourseWorksDataTable(long classRoomId)
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var courseWorks = courseWorkService.GetCourseWorks(classRoomId).Select(c => CourseWorkVM.FromCourseWork(c, clientTimeOffset));

            var parser = new Parser<CourseWorkVM>(Request.Form, courseWorks.AsQueryable())
                  .SetConverter(x => x.UpdatedDate, x => x.UpdatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.CreatedDate, x => x.CreatedDate.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.From, x => x.From.ToString("MMM d, yyyy"))
                   .SetConverter(x => x.To, x => x.To.ToString("MMM d, yyyy"));

            return Ok(parser.Parse());
        }

        [HttpPost("AddCourseWork")]
        public async Task<IActionResult> AddCourseWork(CourseWorkVM model)
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

                    await courseWorkService.AddCourseWork(model.ToCourseWork(), model.File);
                    return Ok(new { IsSuccess = true, Message = "Course work added succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails(), ErrorItems = ex.ErrorItems });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while adding course work");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }

        [HttpPost("UpdateCourseWork")]
        public async Task<IActionResult> UpdateCourseWork(CourseWorkVM model)
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
                    await courseWorkService.UpdateCourseWork(model.ToCourseWork());
                    return Ok(new { IsSuccess = true, Message = "Course work updated succeessfully", ErrorItems = new string[] { } });
                }
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails(), ErrorItems = ex.ErrorItems });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while updating course work");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }

        [HttpDelete("{courseWorkId}/DeleteCourseWork")]
        public async Task<IActionResult> DeleteCourseWork(long courseWorkId)
        {
            try
            {

                await courseWorkService.DeleteCourseWork(courseWorkId);
                return Ok(new { IsSuccess = true, Message = "Course work deleted succeessfully", ErrorItems = new string[] { } });
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails(), ErrorItems = ex.ErrorItems });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while deleting course work");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }

        [HttpGet("{courseWorkId}/GetCourseWork")]
        public async Task<IActionResult> GetCourseWork(long courseWorkId)
        {
            try
            {
                var courseWork = await courseWorkService.GetCourseWork(courseWorkId);
                return Ok(new { IsSuccess = true, Message = "course work retrieved succeessfully", Data = CourseWorkVM.FromCourseWork(courseWork) });
            }
            catch (AppException ex)
            {
                return StatusCode(400, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails(), ErrorItems = ex.ErrorItems });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogError("An error was encountered while retrieving course work");

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = ex.GetErrorDetails() });
            }
        }


    }
}
