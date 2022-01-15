using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchoolPortal.Core;
using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Core.Models;
using SchoolPortal.Core.Models.Views;
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
    public class ResultService : IResultService
    {
        private readonly IRepository<Exam> examRepo;
        private readonly IRepository<MidTermResult> midTermResultRepo;
        private readonly IRepository<MidTermResultViewObject> midTermResultViewObjectRepo;
        private readonly IRepository<EndTermResult> endTermResultRepo;
        private readonly IRepository<EndTermResultViewObject> endTermResultViewObjectRepo;
        private readonly IRepository<PerformanceRemark> performanceRemarkRepo;
        private readonly IRepository<Student> studentRepo;
        private readonly IRepository<Subject> subjectRepo;
        private readonly IRepository<Class> classRepo;
        private readonly ILoggerService<ResultService> logger;
        private readonly IHttpContextAccessor accessor;
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;
        private string[] midTermHeaders = new string[] { "SN", "Student Admission No", "ClassWork", "Test", "Exam", "Total" };
        private string[] endTermHeaders = new string[] { "SN", "Student Admission No", "ClassWork", "Test", "Exam", "Total" };

        public ResultService(
           IRepository<Exam> examRepo,
           IRepository<MidTermResult> midTermResultRepo,
            IRepository<MidTermResultViewObject> midTermResultViewObjectRepo,
           IRepository<EndTermResult> endTermResultRepo,
            IRepository<EndTermResultViewObject> endTermResultViewObjectRepo,
           IRepository<PerformanceRemark> performanceRemarkRepo,
           IRepository<Student> studentRepo,
           IRepository<Subject> subjectRepo,
           IRepository<Class> classRepo,
           ILoggerService<ResultService> logger,
           IHttpContextAccessor accessor,
           IOptionsSnapshot<AppSettings> appSettingsDelegate)
        {
            this.examRepo = examRepo;
            this.midTermResultRepo = midTermResultRepo;
            this.midTermResultViewObjectRepo = midTermResultViewObjectRepo;
            this.endTermResultRepo = endTermResultRepo;
            this.endTermResultViewObjectRepo = endTermResultViewObjectRepo;
            this.performanceRemarkRepo = performanceRemarkRepo;
            this.studentRepo = studentRepo;
            this.subjectRepo = subjectRepo;
            this.classRepo = classRepo;
            this.logger = logger;
            this.accessor = accessor;
            this.appSettingsDelegate = appSettingsDelegate;
        }


        #region Mid-Term Results
        // add result
        public async Task CreateMidTermResult(MidTermResult result)
        {
            if (result == null)
            {
                throw new AppException("Result object cannot be null");
            }

            if (!await subjectRepo.Any(s => s.Id == result.SubjectId))
            {
                throw new AppException("Subject id is invalid");
            }

            if (!await examRepo.Any(e => e.Id == result.ExamId))
            {
                throw new AppException("Exam id is invalid");
            }
            if (!await classRepo.Any(c => c.Id == result.ClassId))
            {
                throw new AppException("Class id is invalid");
            }
            var student = await studentRepo.GetById(result.StudentId);
            if (student == null)
            {
                throw new AppException("Student id is invalid");
            }
            var total = result.ClassWorkScore + result.TestScore + result.ExamScore;
            if (result.Total != total)
            {
                throw new AppException("Total score is invalid");
            }

            if (student.ClassRoomStudents.FirstOrDefault().ClassRoom.ClassId != result.ClassId)
            {
                throw new AppException($"A student with admission number '{student.AdmissionNo}' does not belong to specified class");
            }
            if (await midTermResultRepo.Any(mr => mr.ExamId == result.ExamId && mr.SubjectId == result.SubjectId && mr.StudentId == result.StudentId))
            {
                throw new AppException($"A student with admission number '{student.AdmissionNo}' already have an existing result");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
            result.ClassRoomId = student.ClassRoomStudents.FirstOrDefault().ClassRoom.Id;
            result.CreatedBy = currentUser.Username;
            result.CreatedDate = DateTimeOffset.Now;
            result.UpdatedBy = currentUser.Username;
            result.UpdatedDate = DateTimeOffset.Now;

            await midTermResultRepo.Insert(result, true);

            //log action
            //await loggerService.LogActivity(ActivityActionType.CREATED_ASSESSMENT,
            //    currentUser.PersonNumber,
            //    $"Created new assessment of type '{((Core.AssessmentType)((int)assessment.AssessmentTypeId)).ToString()}' for {assessment.FromDate.ToString("dd-MM-yyyy")} to {assessment.ToDate.ToString("dd-MM-yyyy")}");
        }

        // delete class - without class rooms
        public async Task DeleteMidTermResult(long resultId)
        {
            var result = await midTermResultRepo.GetById(resultId);
            if (result == null)
            {
                throw new AppException($"Invalid result id {resultId}");
            }
            else
            {
                if (await endTermResultRepo.Any(r => r.Exam.Session == result.Exam.Session && r.Exam.TermId == result.Exam.TermId
                && r.ClassId == result.ClassId && r.SubjectId == result.SubjectId && r.StudentId == result.StudentId))
                {
                    throw new AppException("End-term result cannot be deleted as it is already assocciated with a corresponding end-term result");
                }
                else
                {
                    var _result = result.Clone<MidTermResult>();
                    await midTermResultRepo.Delete(resultId, true);

                    var currentUser = accessor.HttpContext.GetUserSession();
                    // log activity
                    //await loggerService.LogActivity(ActivityActionType.DELETED_ASSESSMENT, currentUser.PersonNumber,
                    //    $"Deleted assessment of type '{((Core.AssessmentType)((int)_assessment.AssessmentTypeId)).ToString()}' for {_assessment.FromDate.ToString("dd-MM-yyyy")} to {_assessment.ToDate.ToString("dd-MM-yyyy")}");
                }
            }
        }

        // update class
        public async Task UpdateMidTermResult(MidTermResult result)
        {
            var _result = await midTermResultRepo.GetById(result.Id);
            if (_result == null)
            {
                throw new AppException($"Invalid result id {result.Id}");
            }

            if (!await subjectRepo.Any(s => s.Id == result.SubjectId))
            {
                throw new AppException("Subject id is invalid");
            }

            if (!await examRepo.Any(e => e.Id == result.ExamId))
            {
                throw new AppException("Exam id is invalid");
            }
            if (!await classRepo.Any(c => c.Id == result.ClassId))
            {
                throw new AppException("Class id is invalid");
            }
            var student = await studentRepo.GetById(result.StudentId);
            if (student == null)
            {
                throw new AppException("Student id is invalid");
            }

            var total = result.ClassWorkScore + result.TestScore + result.ExamScore;
            if (result.Total != total)
            {
                throw new AppException("Total score is invalid");
            }

            if (student.ClassRoomStudents.FirstOrDefault().ClassRoom.ClassId != result.ClassId)
            {
                throw new AppException($"A student with admission number '{student.AdmissionNo}' does not belong to specified class");
            }

            if (await midTermResultRepo.Any(mr => mr.ExamId == result.ExamId && mr.SubjectId == result.SubjectId && mr.StudentId == result.StudentId &&
            !(_result.ExamId == result.ExamId && _result.SubjectId == result.SubjectId && _result.StudentId == result.StudentId)))
            {
                throw new AppException($"A student with admission number '{student.AdmissionNo}' already have an existing result");
            }
            var currentUser = accessor.HttpContext.GetUserSession();
            var _oldresult = _result.Clone<MidTermResult>();

            _result.ExamId = result.ExamId;
            _result.SubjectId = result.SubjectId;
            _result.StudentId = result.StudentId;
            _result.ClassId = result.ClassId;
            _result.ClassRoomId = student.ClassRoomStudents.FirstOrDefault().ClassRoom.Id;
            _result.ClassWorkScore = result.ClassWorkScore;
            _result.TestScore = result.TestScore;
            _result.ExamScore = result.ExamScore;
            _result.Total = result.Total;
            _result.UpdatedBy = currentUser.Username;
            _result.UpdatedDate = DateTimeOffset.Now;

            await midTermResultRepo.Update(_result, true);


            // log activity
            //await loggerService.LogActivity(ActivityActionType.UPDATED_result, currentUser.PersonNumber,
            //    resultRepo.TableName, _oldresult, _result,
            //     $"Updated result of type '{((Core.resultType)((int)_result.resultTypeId)).ToString()}' for {_result.FromDate.ToString("dd-MM-yyyy")} to {_result.ToDate.ToString("dd-MM-yyyy")}");


        }

        public IEnumerable<MidTermResult> GetMidTermResults()
        {
            return midTermResultRepo.GetAll().OrderByDescending(r => r.ExamId);
        }
        public IEnumerable<MidTermResultViewObject> GetMidTermResultViewObjects()
        {
            return midTermResultViewObjectRepo.GetAll().OrderByDescending(r => r.ExamId);
        }

        public async Task<MidTermResult> GetMidTermResult(long id)
        {
            return await midTermResultRepo.GetById(id);
        }

        public async Task<MidTermResultViewObject> GetMidTermResultViewObject(long id)
        {
            return await midTermResultViewObjectRepo.GetSingleWhere(r => r.Id == id);
        }

       
        private async Task<(bool isValid, string errorMessage)> ValidateMidTermDataRow(int index, DataRow row)
        {
            var err = "";
            var isValid = true;
            if (row[1] == null || Convert.ToString(row[1]).Trim() == "")
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[1]} at row {index}. Field is required.";
            }
            if (!await studentRepo.Any(s => s.AdmissionNo == Convert.ToString(row[1]).Trim()))
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[1]} at row {index}. No student exist with {midTermHeaders[1]} '{Convert.ToString(row[1]).Trim()}'.";
            }
            else if (row[2] == null || Convert.ToString(row[2]).Trim() == "")
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[2]} at row {index}. Field is required.";
            }
            else if (!(int.TryParse(Convert.ToString(row[2]).Trim(), out int _) && Convert.ToInt32(Convert.ToString(row[2]).Trim()) >= 0 && Convert.ToInt32(Convert.ToString(row[2]).Trim()) <= 10))
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[2]} at row {index}. Field should be a value ranging from 0 to 10.";
            }
            else if (row[3] == null || Convert.ToString(row[3]).Trim() == "")
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[3]} at row {index}. Field is required.";
            }
            else if (!(int.TryParse(Convert.ToString(row[3]).Trim(), out int _) && Convert.ToInt32(Convert.ToString(row[3]).Trim()) >= 0 && Convert.ToInt32(Convert.ToString(row[3]).Trim()) <= 10))
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[3]} at row {index}. Field should be a value ranging from 0 to 10.";
            }
            else if (row[4] == null || Convert.ToString(row[4]).Trim() == "")
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[4]} at row {index}. Field is required.";
            }
            else if (!(int.TryParse(Convert.ToString(row[4]).Trim(), out int _) && Convert.ToInt32(Convert.ToString(row[4]).Trim()) >= 0 && Convert.ToInt32(Convert.ToString(row[4]).Trim()) <= 20))
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[4]} at row {index}. Field should be a value ranging from 0 to 20.";
            }
            else if (row[5] == null || Convert.ToString(row[5]).Trim() == "")
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[5]} at row {index}. Field is required.";
            }
            else if (!(int.TryParse(Convert.ToString(row[5]).Trim(), out int _) && Convert.ToInt32(Convert.ToString(row[5]).Trim()) >= 0 && Convert.ToInt32(Convert.ToString(row[5]).Trim()) <= 40))
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[5]} at row {index}. Field should be a value ranging from 0 to 40.";
            }
            else if (Math.Round((double)(Convert.ToInt32(Convert.ToString(row[2]).Trim()) + Convert.ToInt32(Convert.ToString(row[3]).Trim()) + Convert.ToInt32(Convert.ToString(row[4]).Trim())), MidpointRounding.AwayFromZero) != Convert.ToInt32(Convert.ToString(row[5]).Trim()))
            {
                isValid = false;
                err = $"Inaccurate summation value for {midTermHeaders[5]} at row {index}. ";
            }
            return (isValid, err);
        }

        public async Task<IEnumerable<MidTermResult>> ExtractMidTermData(IFormFile file)
        {
            List<MidTermResult> results = new List<MidTermResult>();
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
            if (!ValidateHeader(header, midTermHeaders, out string error))
            {
                throw new AppException(error);
            }
            else
            {
                //validate and load data
                var rows = table.Rows;
                for (int i = 1; i < rows.Count; i++)
                {
                    var validationResult = await ValidateMidTermDataRow(i, rows[i]);
                    if (!validationResult.isValid)
                    {
                        throw new AppException(validationResult.errorMessage);
                    }
                    else
                    {
                        var studentId = (await studentRepo.GetSingleWhere(s => s.AdmissionNo == Convert.ToString(rows[i][1]).Trim())).Id;
                        var result = new MidTermResult()
                        {
                            StudentId = studentId,
                            ClassWorkScore = Convert.ToDecimal(Convert.ToString(rows[i][2]).Trim()),
                            TestScore = Convert.ToDecimal(Convert.ToString(rows[i][3]).Trim()),
                            ExamScore = Convert.ToDecimal(Convert.ToString(rows[i][4]).Trim()),
                            Total = Convert.ToDecimal(Convert.ToString(rows[i][5]).Trim()),
                        };
                        results.Add(result);
                    }
                }
            }
            fileStream.Dispose();
            return results;
        }

        public async Task BatchCreateMidTermResults(IEnumerable<MidTermResult> results, long examId, long subjectId, long classId)
        {
            if (!await subjectRepo.Any(s => s.Id == subjectId))
            {
                throw new AppException("Subject id is invalid");
            }
            if (!await examRepo.Any(e => e.Id == examId))
            {
                throw new AppException("Exam id is invalid");
            }
            if (!await classRepo.Any(c => c.Id == classId))
            {
                throw new AppException("Class id is invalid");
            }

            var currentUser = accessor.HttpContext.GetUserSession();

            var _results = new List<MidTermResult>();

            foreach (var r in results)
            {
                var student = await studentRepo.GetById(r.StudentId);

                if (student.ClassRoomStudents.FirstOrDefault().ClassRoom.ClassId != classId)
                {
                    throw new AppException($"A student with admission number '{student.AdmissionNo}' does not belong to specified class");
                }

                r.ExamId = examId;
                r.SubjectId = subjectId;
                r.ClassId = classId;
                r.ClassRoomId = student.ClassRoomStudents.FirstOrDefault().ClassRoomId;
                r.CreatedBy = currentUser.Username;
                r.UpdatedBy = currentUser.Username;
                r.CreatedDate = DateTimeOffset.Now;
                r.UpdatedDate = DateTimeOffset.Now;

                //  validate admission no for duplicate
                if (_results.Any(re => re.StudentId == r.StudentId))
                {
                    throw new AppException($"A student with admission number '{student.AdmissionNo}' already have an existing result on excel");
                }
                if (await midTermResultRepo.Any(mr => mr.ExamId == examId && mr.SubjectId == subjectId && mr.StudentId == r.StudentId))
                {
                    throw new AppException($"A student with admission number '{student.AdmissionNo}' already have an existing result");
                }

                _results.Add(r);
            }

            await midTermResultRepo.InsertRange(_results);

            //// log action
            //await loggerService.LogActivity(
            //    $"Created new user with email '{user.Email}'",
            //    ActivityType.CREATE_USER,
            //    currentUser.UserId);

        }

        #endregion Mid-Term Results

        #region End-Term Results
        // add result
        public async Task CreateEndTermResult(EndTermResult result)
        {
            if (result == null)
            {
                throw new AppException("Result object cannot be null");
            }

            if (!await subjectRepo.Any(s => s.Id == result.SubjectId))
            {
                throw new AppException("Subject id is invalid");
            }

            if (!await examRepo.Any(e => e.Id == result.ExamId))
            {
                throw new AppException("Exam id is invalid");
            }
            if (!await classRepo.Any(c => c.Id == result.ClassId))
            {
                throw new AppException("Class id is invalid");
            }
            var student = await studentRepo.GetById(result.StudentId);
            if (student == null)
            {
                throw new AppException("Student id is invalid");
            }
            var total = result.ClassWorkScore + result.TestScore + result.ExamScore;
            if (result.Total != total)
            {
                throw new AppException("Total score is invalid");
            }

            if (student.ClassRoomStudents.FirstOrDefault().ClassRoom.ClassId != result.ClassId)
            {
                throw new AppException($"A student with admission number '{student.AdmissionNo}' does not belong to specified class");
            }

            var exam = await examRepo.GetById(result.ExamId);
            if (!await midTermResultRepo.Any(er => er.Exam.Session == exam.Session && er.Exam.TermId == exam.TermId
            && er.ClassId == result.ClassId && er.SubjectId == result.SubjectId && er.StudentId == result.StudentId))
            {
                throw new AppException($"There is no corresponding mid-term result for this end-term result. Please upload the mid-term result first");
            }

            if (await endTermResultRepo.Any(mr => mr.ExamId == result.ExamId && mr.SubjectId == result.SubjectId && mr.StudentId == result.StudentId))
            {
                throw new AppException($"A student with admission number '{student.AdmissionNo}' already have an existing result");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
            result.ClassRoomId = student.ClassRoomStudents.FirstOrDefault().ClassRoom.Id;
            result.CreatedBy = currentUser.Username;
            result.CreatedDate = DateTimeOffset.Now;
            result.UpdatedBy = currentUser.Username;
            result.UpdatedDate = DateTimeOffset.Now;

            await endTermResultRepo.Insert(result, false);

            //log action
            await logger.LogActivity(ActivityActionType.ADDED_END_TERM_RESULT,
                currentUser.Username,
                $"Added end term result for student with admission number '{student.AdmissionNo}'");
        }

        // delete 
        public async Task DeleteEndTermResult(long resultId)
        {
            var result = await endTermResultRepo.GetById(resultId);
            if (result == null)
            {
                throw new AppException($"Invalid result id {resultId}");
            }
            else
            {
                var _result = result.Clone<EndTermResult>();
                await endTermResultRepo.Delete(resultId, false);

                var currentUser = accessor.HttpContext.GetUserSession();
                // log activity
                await logger.LogActivity(ActivityActionType.DELETED_END_TERM_RESULT,
                currentUser.Username, endTermResultRepo.TableName, _result, new EndTermResult(), $"Deleted end term result with id '{resultId}'");
            }
        }

        // update 
        public async Task UpdateEndTermResult(EndTermResult result)
        {
            var _result = await endTermResultRepo.GetById(result.Id);
            if (_result == null)
            {
                throw new AppException($"Invalid result id {result.Id}");
            }

            if (!await subjectRepo.Any(s => s.Id == result.SubjectId))
            {
                throw new AppException("Subject id is invalid");
            }

            if (!await examRepo.Any(e => e.Id == result.ExamId))
            {
                throw new AppException("Exam id is invalid");
            }
            if (!await classRepo.Any(c => c.Id == result.ClassId))
            {
                throw new AppException("Class id is invalid");
            }
            var student = await studentRepo.GetById(result.StudentId);
            if (student == null)
            {
                throw new AppException("Student id is invalid");
            }

            var total = result.ClassWorkScore + result.TestScore + result.ExamScore;
            if (result.Total != total)
            {
                throw new AppException("Total score is invalid");
            }

            if (student.ClassRoomStudents.FirstOrDefault().ClassRoom.ClassId != result.ClassId)
            {
                throw new AppException($"A student with admission number '{student.AdmissionNo}' does not belong to specified class");
            }

            if (await endTermResultRepo.Any(mr => mr.ExamId == result.ExamId && mr.SubjectId == result.SubjectId && mr.StudentId == result.StudentId &&
            !(_result.ExamId == result.ExamId && _result.SubjectId == result.SubjectId && _result.StudentId == result.StudentId)))
            {
                throw new AppException($"A student with admission number '{student.AdmissionNo}' already have an existing result");
            }
            var currentUser = accessor.HttpContext.GetUserSession();
            var _oldresult = _result.Clone<EndTermResult>();

            _result.ExamId = result.ExamId;
            _result.SubjectId = result.SubjectId;
            _result.StudentId = result.StudentId;
            _result.ClassId = result.ClassId;
            _result.ClassRoomId = student.ClassRoomStudents.FirstOrDefault().ClassRoom.Id;
            _result.ClassWorkScore = result.ClassWorkScore;
            _result.TestScore = result.TestScore;
            _result.ExamScore = result.ExamScore;
            _result.Total = result.Total;
            _result.UpdatedBy = currentUser.Username;
            _result.UpdatedDate = DateTimeOffset.Now;

            await endTermResultRepo.Update(_result, false);

            // log activity
            await logger.LogActivity(ActivityActionType.UPDATED_END_TERM_RESULT,
                currentUser.Username, endTermResultRepo.TableName, _oldresult, _result, $"Updated end term result with id '{result.Id}'");

        }

        public IEnumerable<EndTermResult> GetEndTermResults()
        {
            return endTermResultRepo.GetAll().OrderByDescending(r => r.ExamId);
        }
        public IEnumerable<EndTermResultViewObject> GetEndTermResultViewObjects()
        {
            return endTermResultViewObjectRepo.GetAll().OrderByDescending(r => r.ExamId);
        }

        public async Task<EndTermResult> GetEndTermResult(long id)
        {
            return await endTermResultRepo.GetById(id);
        }

        public async Task<EndTermResultViewObject> GetEndTermResultViewObject(long id)
        {
            return await endTermResultViewObjectRepo.GetSingleWhere(r => r.Id == id);
        }

        private async Task<(bool isValid, string errorMessage)> ValidateEndTermDataRow(int index, DataRow row)
        {
            var err = "";
            var isValid = true;
            if (row[1] == null || Convert.ToString(row[1]).Trim() == "")
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[1]} at row {index}. Field is required.";
            }
            if (!await studentRepo.Any(s => s.AdmissionNo == Convert.ToString(row[1]).Trim()))
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[1]} at row {index}. No student exist with {midTermHeaders[1]} '{Convert.ToString(row[1]).Trim()}'.";
            }
            else if (row[2] == null || Convert.ToString(row[2]).Trim() == "")
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[2]} at row {index}. Field is required.";
            }
            else if (!(int.TryParse(Convert.ToString(row[2]).Trim(), out int _) && Convert.ToInt32(Convert.ToString(row[2]).Trim()) >= 0 && Convert.ToInt32(Convert.ToString(row[2]).Trim()) <= 10))
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[2]} at row {index}. Field should be a value ranging from 0 to 10.";
            }
            else if (row[3] == null || Convert.ToString(row[3]).Trim() == "")
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[3]} at row {index}. Field is required.";
            }
            else if (!(int.TryParse(Convert.ToString(row[3]).Trim(), out int _) && Convert.ToInt32(Convert.ToString(row[3]).Trim()) >= 0 && Convert.ToInt32(Convert.ToString(row[3]).Trim()) <= 10))
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[3]} at row {index}. Field should be a value ranging from 0 to 10.";
            }
            else if (row[4] == null || Convert.ToString(row[4]).Trim() == "")
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[4]} at row {index}. Field is required.";
            }
            else if (!(int.TryParse(Convert.ToString(row[4]).Trim(), out int _) && Convert.ToInt32(Convert.ToString(row[4]).Trim()) >= 0 && Convert.ToInt32(Convert.ToString(row[4]).Trim()) <= 20))
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[4]} at row {index}. Field should be a value ranging from 0 to 20.";
            }
            else if (row[5] == null || Convert.ToString(row[5]).Trim() == "")
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[5]} at row {index}. Field is required.";
            }
            else if (!(int.TryParse(Convert.ToString(row[5]).Trim(), out int _) && Convert.ToInt32(Convert.ToString(row[5]).Trim()) >= 0 && Convert.ToInt32(Convert.ToString(row[5]).Trim()) <= 40))
            {
                isValid = false;
                err = $"Invalid value for {midTermHeaders[5]} at row {index}. Field should be a value ranging from 0 to 40.";
            }
            else if (Math.Round((double)(Convert.ToInt32(Convert.ToString(row[2]).Trim()) + Convert.ToInt32(Convert.ToString(row[3]).Trim()) + Convert.ToInt32(Convert.ToString(row[4]).Trim())), MidpointRounding.AwayFromZero) != Convert.ToInt32(Convert.ToString(row[5]).Trim()))
            {
                isValid = false;
                err = $"Inaccurate summation value for {midTermHeaders[5]} at row {index}. ";
            }
            return (isValid, err);
        }

        public async Task<IEnumerable<EndTermResult>> ExtractEndTermData(IFormFile file)
        {
            List<EndTermResult> results = new List<EndTermResult>();
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
            if (!ValidateHeader(header, endTermHeaders, out string error))
            {
                throw new AppException(error);
            }
            else
            {
                //validate and load data
                var rows = table.Rows;
                for (int i = 1; i < rows.Count; i++)
                {
                    var validationResult = await ValidateEndTermDataRow(i, rows[i]);
                    if (!validationResult.isValid)
                    {
                        throw new AppException(validationResult.errorMessage);
                    }
                    else
                    {
                        var studentId = (await studentRepo.GetSingleWhere(s => s.AdmissionNo == Convert.ToString(rows[i][1]).Trim())).Id;
                        var result = new EndTermResult()
                        {
                            StudentId = studentId,
                            ClassWorkScore = Convert.ToDecimal(Convert.ToString(rows[i][2]).Trim()),
                            TestScore = Convert.ToDecimal(Convert.ToString(rows[i][3]).Trim()),
                            ExamScore = Convert.ToDecimal(Convert.ToString(rows[i][4]).Trim()),
                            Total = Convert.ToDecimal(Convert.ToString(rows[i][5]).Trim()),
                        };
                        results.Add(result);
                    }
                }
            }
            fileStream.Dispose();
            return results;
        }

        public async Task BatchCreateEndTermResults(IEnumerable<EndTermResult> results, long examId, long subjectId, long classId)
        {
            if (!await subjectRepo.Any(s => s.Id == subjectId))
            {
                throw new AppException("Subject id is invalid");
            }
            if (!await examRepo.Any(e => e.Id == examId))
            {
                throw new AppException("Exam id is invalid");
            }
            if (!await classRepo.Any(c => c.Id == classId))
            {
                throw new AppException("Class id is invalid");
            }

            var currentUser = accessor.HttpContext.GetUserSession();

            var _results = new List<EndTermResult>();

            foreach (var r in results)
            {
                var student = await studentRepo.GetById(r.StudentId);

                if (student.ClassRoomStudents.FirstOrDefault().ClassRoom.ClassId != classId)
                {
                    throw new AppException($"A student with admission number '{student.AdmissionNo}' does not belong to specified class");
                }

                r.ExamId = examId;
                r.SubjectId = subjectId;
                r.ClassId = classId;
                r.ClassRoomId = student.ClassRoomStudents.FirstOrDefault().ClassRoomId;
                r.CreatedBy = currentUser.Username;
                r.UpdatedBy = currentUser.Username;
                r.CreatedDate = DateTimeOffset.Now;
                r.UpdatedDate = DateTimeOffset.Now;

                //  validate admission no for duplicate
                if (_results.Any(re => re.StudentId == r.StudentId))
                {
                    throw new AppException($"A student with admission number '{student.AdmissionNo}' already have an existing result on excel");
                }
                if (await midTermResultRepo.Any(mr => mr.ExamId == examId && mr.SubjectId == subjectId && mr.StudentId == r.StudentId))
                {
                    throw new AppException($"A student with admission number '{student.AdmissionNo}' already have an existing result");
                }

                _results.Add(r);
            }

            await endTermResultRepo.InsertRange(_results);

            // log action
            await logger.LogActivity(ActivityActionType.BATCH_ADDED_END_TERM_RESULT,
                currentUser.Username, $"Added end term results in batch'");

        }

        #endregion End-Term Results


        //=========== Batch Upload ===========
        public bool ValidateFile(IFormFile file, out List<string> errorItems)
        {
            bool isValid = true;
            List<string> errList = new List<string>();
            var maxUploadSize = appSettingsDelegate.Value.MaxUploadSize;
            if (file == null)
            {
                isValid = false;
                errList.Add("No file uploaded.");
            }
            else
            {
                if (file.Length > (maxUploadSize * 1024 * 1024))
                {
                    isValid = false;
                    errList.Add($"Max upload size exceeded. Max size is {maxUploadSize}MB");
                }
                var ext = Path.GetExtension(file.FileName);
                if (ext != ".xls" && ext != ".xlsx")
                {
                    isValid = false;
                    errList.Add($"Invalid file format. Supported file formats include .xls and .xlsx");
                }
            }
            errorItems = errList;
            return isValid;
        }

        private bool ValidateHeader(DataRow row, string[] headers, out string errorMessage)
        {
            var err = "";
            var isValid = true;
            for (int i = 0; i < headers.Length; i++)
            {
                if (row[i] == null || Convert.ToString(row[i]).Trim().ToLower() != headers[i].ToLower())
                {
                    isValid = false;
                    err = $"Invalid header value at column {i + 1}. Expected value is {headers[i]}";
                    break;
                }
            }
            errorMessage = err;
            return isValid;
        }


    }
}
