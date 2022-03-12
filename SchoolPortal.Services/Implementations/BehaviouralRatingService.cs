using ClosedXML.Excel;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SchoolPortal.Core;
using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Core.Models;
using SchoolPortal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services.Implementations
{
    public class BehaviouralRatingService : IBehaviouralRatingService
    {
        private readonly IRepository<BehaviouralRating> behaviouralRatingRepo;
        private readonly IRepository<BehaviouralResult> behaviouralResultRepo;
        private readonly IRepository<Term> termRepo;
        private readonly IRepository<Student> studentRepo;
        private readonly ILoggerService<BehaviouralRatingService> logger;
        private readonly IHttpContextAccessor accessor;
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;
        private List<string> headers = new List<string>() { "SN", "Student Admission No" };
        private string[] validRatings = new string[] { "Very Good", "Good", "Fair", "Poor", "Very Poor" };

        public BehaviouralRatingService(
            IRepository<BehaviouralRating> behaviouralRatingRepo,
            IRepository<BehaviouralResult> behaviouralResultRepo,
            IRepository<Term> termRepo,
            IRepository<Student> studentRepo,
            ILoggerService<BehaviouralRatingService> logger,
             IHttpContextAccessor accessor,
             IOptionsSnapshot<AppSettings> appSettingsDelegate)
        {
            this.behaviouralRatingRepo = behaviouralRatingRepo;
            this.behaviouralResultRepo = behaviouralResultRepo;
            this.termRepo = termRepo;
            this.studentRepo = studentRepo;
            this.logger = logger;
            this.accessor = accessor;
            this.appSettingsDelegate = appSettingsDelegate;

            var ratings = behaviouralRatingRepo.GetAll().Select(r => r.Name);
            headers.AddRange(ratings);
        }

        // get ratings
        public IEnumerable<BehaviouralRating> GetBehaviouralRatings()
        {
            return behaviouralRatingRepo.GetAll().OrderBy(r => r.Category).ThenBy(r => r.Id);
        }

        public async Task<BehaviouralRating> GetBehaviouralRatingByName(string name)
        {
            var rating = await behaviouralRatingRepo.GetSingleWhere(r => r.Name == name);
            return rating;
        }
        // add
        public async Task CreateBehaviouralResult(BehaviouralResult behaviouralResult)
        {
            if (behaviouralResult == null)
            {
                throw new AppException("Behavioural result object cannot be null");
            }

            if (!await termRepo.AnyAsync(t => t.Id == behaviouralResult.TermId))
            {
                throw new AppException("Invalid term id");
            }

            if (!await studentRepo.AnyAsync(s => s.Id == behaviouralResult.StudentId))
            {
                throw new AppException("Invalid student id");
            }

            if (await behaviouralResultRepo.AnyAsync(r => r.Session == behaviouralResult.Session
            && r.TermId == behaviouralResult.TermId && r.StudentId == behaviouralResult.StudentId
            && r.BehaviouralRatingId == behaviouralResult.BehaviouralRatingId))
            {
                throw new AppException($"A behavioural result already exist for specified session, term and student");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
            behaviouralResult.CreatedBy = currentUser.Username;
            behaviouralResult.CreatedDate = DateTimeOffset.Now;
            behaviouralResult.UpdatedBy = currentUser.Username;
            behaviouralResult.UpdatedDate = DateTimeOffset.Now;

            await behaviouralResultRepo.Insert(behaviouralResult, false);

            //log action
            await logger.LogActivity(ActivityActionType.CREATED_BEHAVIOURAL_RESULT,
                currentUser.Username,
                $"Created new behavioural result for student with id '{behaviouralResult.StudentId}'");
        }
        // add in batch
        public async Task CreateBehaviouralResults(string session, long termId, long studentId, IEnumerable<(long rateId, string score)> scores)
        {
            if (session == null)
            {
                throw new AppException("Session is required");
            }

            if (!await termRepo.AnyAsync(t => t.Id == termId))
            {
                throw new AppException("Invalid term id");
            }

            if (!await studentRepo.AnyAsync(s => s.Id == studentId))
            {
                throw new AppException("Invalid student id");
            }

            var invalidRateIds = scores.Where(s => !behaviouralRatingRepo.Any(r => r.Id == s.rateId));
            if (invalidRateIds.Count() > 0)
            {
                throw new AppException("Invalid rate ids suplied: " + string.Join(", ", invalidRateIds));
            }

            var existingResults = scores.Where(s => behaviouralResultRepo.Any(r => r.Session == session
              && r.TermId == termId && r.StudentId == studentId && r.BehaviouralRatingId == s.rateId));

            if (existingResults.Count() > 0)
            {
                throw new AppException($"One or more behavioural results already exist");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
            var results = scores.Select(s => new BehaviouralResult
            {
                BehaviouralRatingId = s.rateId,
                Score = s.score,
                Session = session,
                TermId = termId,
                StudentId = studentId,
                CreatedBy = currentUser.Username,
                UpdatedBy = currentUser.Username,
                CreatedDate = DateTimeOffset.Now,
                UpdatedDate = DateTimeOffset.Now
            });

            await behaviouralResultRepo.InsertOrUpdateBulk(results);

            //log action
            await logger.LogActivity(ActivityActionType.CREATED_BEHAVIOURAL_RESULT,
                currentUser.Username,
                $"Created behavioural results in batch for student with id '{studentId}'");
        }

        // delete 
        public async Task DeleteBehaviouralResult(long behaviouralResultId)
        {
            var behaviouralResult = await behaviouralResultRepo.GetById(behaviouralResultId);
            if (behaviouralResult == null)
            {
                throw new AppException($"Invalid behavioural result id {behaviouralResultId}");
            }
            else
            {
                var _behaviouralResult = behaviouralResult.Clone<BehaviouralResult>();
                await behaviouralResultRepo.Delete(behaviouralResultId, true);

                var currentUser = accessor.HttpContext.GetUserSession();
                // log activity
                await logger.LogActivity(ActivityActionType.DELETED_BEHAVIOURAL_RESULT, currentUser.Username,
                    behaviouralResultRepo.TableName, _behaviouralResult, new BehaviouralResult(),
                    $"Deleted behavioural result");
            }
        }

        public async Task DeleteBehaviouralResults(string session, long termId, long studentId)
        {
            if (session == null)
            {
                throw new AppException("Session is required");
            }

            if (!await termRepo.AnyAsync(t => t.Id == termId))
            {
                throw new AppException("Invalid term id");
            }

            if (!await studentRepo.AnyAsync(s => s.Id == studentId))
            {
                throw new AppException("Invalid student id");
            }

            await behaviouralResultRepo.DeleteWhere(r=> r.Session==session && r.TermId==termId && r.StudentId==studentId, true);

            var currentUser = accessor.HttpContext.GetUserSession();
            // log activity
            await logger.LogActivity(ActivityActionType.DELETED_BEHAVIOURAL_RESULT, currentUser.Username,
                $"Deleted behavioural result for student with id '{studentId}'");
        }

        // update 
        public async Task UpdateBehaviouralResult(BehaviouralResult behaviouralResult)
        {
            if (behaviouralResult == null)
            {
                throw new AppException("Behavioural result object cannot be null");
            }

            if (!await termRepo.AnyAsync(t => t.Id == behaviouralResult.TermId))
            {
                throw new AppException("Invalid term id");
            }

            if (!await studentRepo.AnyAsync(s => s.Id == behaviouralResult.StudentId))
            {
                throw new AppException("Invalid student id");
            }

            var _behaviouralResult = await behaviouralResultRepo.GetById(behaviouralResult.Id);
            if (_behaviouralResult == null)
            {
                throw new AppException($"Invalid behaviouralResult id {@behaviouralResult.Id}");
            }

            else if (await behaviouralResultRepo.AnyAsync(r => (r.Session == behaviouralResult.Session
            && r.TermId == behaviouralResult.TermId && r.StudentId == behaviouralResult.StudentId && r.BehaviouralRatingId == behaviouralResult.BehaviouralRatingId) &&
            !(_behaviouralResult.Session == behaviouralResult.Session
            && _behaviouralResult.TermId == behaviouralResult.TermId
            && _behaviouralResult.StudentId == behaviouralResult.StudentId && _behaviouralResult.BehaviouralRatingId == behaviouralResult.BehaviouralRatingId)))
            {
                throw new AppException($"A behavioural result already exist for specified session, term and student");
            }
            else
            {
                var currentUser = accessor.HttpContext.GetUserSession();
                var _oldbehaviouralResult = _behaviouralResult.Clone<BehaviouralResult>();

                _behaviouralResult.Session = behaviouralResult.Session;
                _behaviouralResult.TermId = behaviouralResult.TermId;
                _behaviouralResult.StudentId = behaviouralResult.StudentId;
                _behaviouralResult.BehaviouralRatingId = behaviouralResult.BehaviouralRatingId;
                _behaviouralResult.Score = behaviouralResult.Score;
                _behaviouralResult.UpdatedBy = currentUser.Username;
                _behaviouralResult.UpdatedDate = DateTimeOffset.Now;

                await behaviouralResultRepo.Update(_behaviouralResult, false);


                // log activity
                await logger.LogActivity(ActivityActionType.UPDATED_BEHAVIOURAL_RESULT, currentUser.Username,
                    behaviouralResultRepo.TableName, _oldbehaviouralResult, _behaviouralResult,
                     $"Updated behavioural result");
            }
        }

        public async Task UpdateBehaviouralResults(string session, long termId, long studentId, IEnumerable<(long id, long rateId, string score)> scores)
        {
            if (session == null)
            {
                throw new AppException("Session is required");
            }

            if (!await termRepo.AnyAsync(t => t.Id == termId))
            {
                throw new AppException("Invalid term id");
            }

            if (!await studentRepo.AnyAsync(s => s.Id == studentId))
            {
                throw new AppException("Invalid student id");
            }

            var invalidRateIds = scores.Where(s => !behaviouralRatingRepo.Any(r => r.Id == s.rateId));
            if (invalidRateIds.Count() > 0)
            {
                throw new AppException("Invalid rate ids suplied: " + string.Join(", ", invalidRateIds));
            }

            var scoresToInsert = scores.Where(s => s.id == 0).Select(s => (s.rateId, s.score));
            scores = scores.Where(s => s.id > 0);

            var invalidResultIds = scores.Where(s => !behaviouralResultRepo.Any(r => r.Session == session
            && r.TermId == termId && r.StudentId == studentId && r.Id == s.id));
            if (invalidResultIds.Count() > 0)
            {
                throw new AppException("Invalid result ids suplied: " + string.Join(", ", invalidResultIds));
            }

            var idsToUpdate = scores.Select(s => s.id);
            var results = behaviouralResultRepo.GetWhere(r => idsToUpdate.Contains(r.Id)).ToList();

            var currentUser = accessor.HttpContext.GetUserSession();
            results.ForEach(r =>
            {
                r.Score = scores.FirstOrDefault(s => s.id == r.Id).score;
                r.UpdatedBy = currentUser.Username;
                r.UpdatedDate = DateTime.Now;
            });

            // check for duplicate
            var resultIds = scores.Select(s => s.id).ToList();
            var rateIds = scores.Select(s => s.rateId).ToList();
            var existingRecords = behaviouralResultRepo.GetWhere(r => r.Session == session && r.TermId == termId && r.StudentId == studentId
                && rateIds.Contains(r.BehaviouralRatingId) && !resultIds.Contains(r.Id));
            
            if (existingRecords.Count() > 0)
            {
                throw new AppException($"One or more behavioural results already exist");
            }

            await behaviouralResultRepo.UpdateRange(results, false);

            // log activity
            await logger.LogActivity(ActivityActionType.UPDATED_BEHAVIOURAL_RESULT, currentUser.Username,
                 $"Updated behavioural results in batch for student with id '{studentId}'");

            try
            {
                await CreateBehaviouralResults(session, termId, studentId, scoresToInsert);
            }catch(AppException ex)
            {
                throw new AppException($"Behavioural results for update was successful! However, an error was encountered while saving new ones. Error: {ex.Message}");
            }
           
        }

        public IEnumerable<BehaviouralResult> GetBehaviouralResults()
        {
            return behaviouralResultRepo.GetAll();
        }

        public async Task<BehaviouralResult> GetBehaviouralResult(long id)
        {
            return await behaviouralResultRepo.GetById(id);
        }

        public IEnumerable<BehaviouralResult> GetBehaviouralResults(string session, long termId, long studentId)
        {
            return behaviouralResultRepo.GetWhere(r => r.Session == session
            && r.TermId == termId && r.StudentId == studentId);
        }

        // generate batch upload template
        public byte[] GenerateBatchUploadTemaplate()
        {
            var ratings = behaviouralRatingRepo.GetAll().Select(x => x.Name).ToList();

            // create excel
            var workbook = new XLWorkbook(ClosedXML.Excel.XLEventTracking.Disabled);
            // using data table
            var table = new DataTable("Behavioural Results");
            table.Columns.Add("SN", typeof(string));
            table.Columns.Add("Student Admission No", typeof(string));
            
            foreach (var r in ratings)
            {
                table.Columns.Add(r, typeof(string));
            }

            var count = 1;
            for(var i=0;i<2;i++)
            {
                var row = table.NewRow();

                row[0] = count.ToString();
                row[1] = count.ToString().PadLeft(3, '0');
                for(int j = 2; j < (ratings.Count + 2); j++)
                {
                    row[j] = "Good";
                }
                table.Rows.Add(row);
                count++;
            }
            
            workbook.AddWorksheet(table);

            byte[] byteFile = null;
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                byteFile = stream.ToArray();
            }

            return byteFile;
        }

        //=========== Batch Upload ===========
       
        private async Task<(bool isValid, string errorMessage)> ValidateDataRow(int index, DataRow row)
        {
            var err = "";
            var isValid = true;
            if (row[1] == null || Convert.ToString(row[1]).Trim() == "")
            {
                isValid = false;
                err = $"Invalid value for {headers[1]} at row {index}. Field is required.";
            }
            else if (!await studentRepo.AnyAsync(s => s.AdmissionNo == Convert.ToString(row[1]).Trim()))
            {
                isValid = false;
                err = $"Invalid value for {headers[1]} at row {index}. No student exist with {headers[1]} '{Convert.ToString(row[1]).Trim()}'.";
            }
            else
            {

                for (int i = 2; i < headers.Count; i++)
                {
                    if (row[i] == null || Convert.ToString(row[i]).Trim() == "" || !validRatings.Contains(Convert.ToString(row[i]).Trim().Capitalize()))
                    {
                        isValid = false;
                        err = $"Invalid value for {headers[i]} at row {index}. Valid values includes.";
                    }
                    if (!isValid)
                        break;
                }
            }

            return (isValid, err);
        }

        public async Task<IEnumerable<IEnumerable<BehaviouralResult>>> ExtractData(IFormFile file)
        {
            List<List<BehaviouralResult>> behaviouralResults = new List<List<BehaviouralResult>>();
            IExcelDataReader excelReader = null;
            DataSet dataSet = new DataSet();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var fileStream = file.OpenReadStream();

            if (file.FileName.EndsWith(".xls"))
                excelReader = ExcelReaderFactory.CreateBinaryReader(fileStream);
            else if (file.FileName.EndsWith(".xlsx"))
                excelReader = ExcelReaderFactory.CreateReader(fileStream);
            else
                throw new AppException($"Invalid file '{file.FileName}'");

            dataSet = excelReader.AsDataSet();
            excelReader.Close();

            if (dataSet == null || dataSet.Tables.Count == 0)
                throw new AppException($"Unable to read file. Ensure file complies with the specified template.");

            var table = dataSet.Tables[0];
            var header = table.Rows[0];
            if (!AppUtilities.ValidateHeader(header, headers.ToArray(), out string error))
            {
                throw new AppException(error);
            }
            else
            {
                //validate and load data
                var rows = table.Rows;
                for (int i = 1; i < rows.Count; i++)
                {
                    var validationResult = await ValidateDataRow(i, rows[i]);
                    if (!validationResult.isValid)
                    {
                        throw new AppException(validationResult.errorMessage);
                    }
                    else
                    {
                        var ratings = GetBehaviouralRatings().ToList();
                        var results = new List<BehaviouralResult>();
                        var studentId = (await studentRepo.GetSingleWhere(s => s.AdmissionNo == Convert.ToString(rows[i][1]).Trim())).Id;

                        for(int j = 2; j < headers.Count; j++)
                        {

                            var result = new BehaviouralResult
                            {
                                BehaviouralRatingId = ratings.FirstOrDefault(r => r.Name == headers[j]).Id,
                                Score = Convert.ToString(rows[i][j]),
                                StudentId = studentId
                            };
                            results.Add(result);
                        }

                        behaviouralResults.Add(results);
                    }
                }
            }
            fileStream.Dispose();
            return behaviouralResults;
        }

        public async Task BatchCreateBehaviouralResults(IEnumerable<List<BehaviouralResult>> results, string session, long termId)
        {
            if (!AppUtilities.ValidateSession(session))
            {
                throw new AppException($"Session '{session}' is invalid");
            }
            if (!await termRepo.AnyAsync(t => t.Id == termId))
            {
                throw new AppException("Term id is invalid");
            }

            var currentUser = accessor.HttpContext.GetUserSession();

            var _results = new List<BehaviouralResult>();

            foreach (var r in results)
            {
                r.ForEach(v =>
                {
                    v.Session = session;
                    v.TermId = termId;
                    v.CreatedBy = currentUser.Username;
                    v.CreatedDate = DateTimeOffset.Now;
                    v.UpdatedBy = currentUser.Username;
                    v.UpdatedDate = DateTimeOffset.Now;
                });

                // check for duplicate
                var student = await studentRepo.GetById(r.First().StudentId);
                if(_results.Any(re=> re.StudentId == r.First().StudentId)){
                    throw new AppException($"A student with admission number '{student.AdmissionNo}' already have an existing behavioural result on excel");
                }
               
                if (await behaviouralResultRepo.AnyAsync(br => br.Session == session && br.TermId == termId && br.StudentId == student.Id))
                {
                    throw new AppException($"A student with admission number '{student.AdmissionNo}' already have an existing behavioural result");
                }

                _results.AddRange(r);
            }

            await behaviouralResultRepo.InsertBulk(_results);

            // log action
            await logger.LogActivity(ActivityActionType.BATCH_ADDED_BEHAVIOURAL_RESULT,
                currentUser.Username, $"Added behavioural results in batch'");
        }

    }
}
