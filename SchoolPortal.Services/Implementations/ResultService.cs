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
    public class ResultService:IResultService
    {
        private readonly IRepository<Exam> examRepo;
        private readonly IRepository<MidTermResult> midTermResultRepo;
        private readonly IRepository<MidTermResultViewObject> midTermResultViewObjectRepo;
        private readonly IRepository<EndTermResult> endTermResultRepo;
        private readonly IRepository<PerformanceRemark> performanceRemarkRepo;
        private readonly IRepository<Student> studentRepo;
        private readonly IRepository<Subject> subjectRepo;
        private readonly IRepository<Class> classRepo;
        private readonly ILogger<ResultService> logger;
        private readonly IHttpContextAccessor accessor;
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;
        private string[] midTermHeaders = new string[] { "SN", "Student Admission No", "ClassWork", "Test", "Exam", "Total" };

        public ResultService(
           IRepository<Exam> examRepo,
           IRepository<MidTermResult> midTermResultRepo,
            IRepository<MidTermResultViewObject> midTermResultViewObjectRepo,
           IRepository<EndTermResult> endTermResultRepo,
           IRepository<PerformanceRemark> performanceRemarkRepo,
           IRepository<Student> studentRepo,
           IRepository<Subject> subjectRepo,
           IRepository<Class> classRepo,
           ILogger<ResultService> logger,
           IHttpContextAccessor accessor,
           IOptionsSnapshot<AppSettings> appSettingsDelegate)
        {
            this.examRepo = examRepo;
            this.midTermResultRepo = midTermResultRepo;
            this.midTermResultViewObjectRepo = midTermResultViewObjectRepo;
            this.endTermResultRepo = endTermResultRepo;
            this.performanceRemarkRepo = performanceRemarkRepo;
            this.studentRepo = studentRepo;
            this.subjectRepo = subjectRepo;
            this.classRepo = classRepo;
            this.logger = logger;
            this.accessor = accessor;
            this.appSettingsDelegate = appSettingsDelegate;
        }

        public IEnumerable<MidTermResultViewObject> GetMidTermResultViewObjects()
        {
            return midTermResultViewObjectRepo.GetAll();
        }

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


        //public byte[] ExportUsersToExcel(int id)
        //{
        //    var users = userRepo.GetAll();

        //    // create excel
        //    var workbook = new XLWorkbook(ClosedXML.Excel.XLEventTracking.Disabled);

        //    // using data table
        //    var table = new DataTable("Users");
        //    foreach (var h in headers)
        //    {
        //        table.Columns.Add(h, typeof(string));
        //    }
        //    table.Columns.Add("Is Active?", typeof(string));
        //    table.Columns.Add("Created By", typeof(string));
        //    table.Columns.Add("Date Created", typeof(string));

        //    var count = 1;
        //    foreach (var u in users)
        //    {
        //        var row = table.NewRow();

        //        row[0] = count.ToString();
        //        row[1] = u.FirstName;
        //        row[2] = u.MiddleName;
        //        row[3] = u.Surname;
        //        row[4] = u.Gender;
        //        row[5] = u.DateOfBirth;
        //        row[6] = u.Email;
        //        row[7] = u.PhoneNumber;
        //        row[8] = u.IsActive ? "Yes" : "No";
        //        row[9] = u.CreatedBy;
        //        row[10] = u.CreatedDate.ToString("yyyy-MM-dd hh:mmtt");

        //        table.Rows.Add(row);
        //        count++;
        //    }
        //    workbook.AddWorksheet(table);

        //    byte[] byteFile = null;
        //    using (var stream = new MemoryStream())
        //    {
        //        workbook.SaveAs(stream);
        //        byteFile = stream.ToArray();
        //    }

        //    return byteFile;
        //}

        //=========== End Batch Upload =============

    }
}
