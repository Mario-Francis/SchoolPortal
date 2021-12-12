﻿using DataTablesParser;
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
        private readonly ILogger<ResultsController> logger;

        public ResultsController(IOptionsSnapshot<AppSettings> appSettingsDelegate,
            IResultService resultService,
            ILogger<ResultsController> logger)
        {
            this.appSettingsDelegate = appSettingsDelegate;
            this.resultService = resultService;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult MidTermResults()
        {
            return View();
        }

        [HttpPost("[controller]/midterm/ResultsDataTable")]
        public IActionResult MidTermResultsDataTable()
        {
            var clientTimeOffset = string.IsNullOrEmpty(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]) ?
                appSettingsDelegate.Value.DefaultTimeZoneOffset : Convert.ToInt32(Request.Cookies[Core.Constants.CLIENT_TIMEOFFSET_COOKIE_ID]);

            var results = resultService.GetMidTermResultViewObjects().Select(r => MidTermResultViewObjectVM.FromMidTermResultViewObject(r, clientTimeOffset));

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
                logger.LogError(ex, "An error was encountered while adding mid-term results in batch");
                //await loggerService.LogException(ex);
                //await loggerService.LogError(ex.GetErrorDetails());

                return StatusCode(500, new { IsSuccess = false, Message = ex.Message, ErrorDetail = JsonSerializer.Serialize(ex.InnerException) });
            }
        }

    }
}