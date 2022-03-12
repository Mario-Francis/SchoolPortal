using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using SchoolPortal.Core;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Core.Models;
using SchoolPortal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Services.Implementations
{
    public class PerformanceRemarkService: IPerformanceRemarkService
    {
        private readonly IRepository<PerformanceRemark> remarkRepo;
        private readonly IRepository<Exam> examRepo;
        private readonly IRepository<Student> studentRepo;
        private readonly ILoggerService<PerformanceRemarkService> logger;
        private readonly IHttpContextAccessor accessor;
        private readonly IRepository<MidTermResult> midTermResultRepo;
        private readonly IRepository<EndTermResult> endTermResultRepo;
        private string[] headers = new string[] { "SN", "Student Admission No", "Teacher's Remark", "Head Teacher's Remark" };

        public PerformanceRemarkService(
            IRepository<PerformanceRemark> remarkRepo,
            IRepository<Exam> examRepo,
            IRepository<Student> studentRepo,
            ILoggerService<PerformanceRemarkService> logger,
            IHttpContextAccessor accessor,
            IRepository<MidTermResult> midTermResultRepo,
            IRepository<EndTermResult> endTermResultRepo)
        {
            this.remarkRepo = remarkRepo;
            this.examRepo = examRepo;
            this.studentRepo = studentRepo;
            this.logger = logger;
            this.accessor = accessor;
            this.midTermResultRepo = midTermResultRepo;
            this.endTermResultRepo = endTermResultRepo;
        }

        // add
        public async Task CreateRemark(PerformanceRemark remark)
        {
            if (remark == null)
            {
                throw new AppException("Remark object cannot be null");
            }

            if (!await examRepo.AnyAsync(e => e.Id == remark.ExamId))
            {
                throw new AppException("Invalid exam id");
            }

            if (!await studentRepo.AnyAsync(s => s.Id == remark.StudentId))
            {
                throw new AppException("Invalid student id");
            }

            if (await remarkRepo.AnyAsync(r => r.ExamId == remark.ExamId && r.StudentId == remark.StudentId))
            {
                throw new AppException($"A remark already exist for specified exam and student");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
            remark.CreatedBy = currentUser.Username;
            remark.CreatedDate = DateTimeOffset.Now;
            remark.UpdatedBy = currentUser.Username;
            remark.UpdatedDate = DateTimeOffset.Now;

            await remarkRepo.Insert(remark, false);

            //log action
            await logger.LogActivity(ActivityActionType.CREATED_REMARK,
                currentUser.Username,
                $"Created new remark for student with id '{remark.StudentId}'");
        }

        // delete 
        public async Task DeleteRemark(long remarkId)
        {
            var remark = await remarkRepo.GetById(remarkId);
            if (remark == null)
            {
                throw new AppException($"Invalid remark id {remarkId}");
            }
            else
            {

                var _remark = remark.Clone<PerformanceRemark>();
                await remarkRepo.Delete(remarkId, true);

                var currentUser = accessor.HttpContext.GetUserSession();
                // log activity
                await logger.LogActivity(ActivityActionType.DELETED_REMARK, currentUser.Username,
                    remarkRepo.TableName, _remark, new PerformanceRemark(),
                    $"Deleted remark");
            }
        }


        // update 
        public async Task UpdateRemark(PerformanceRemark remark)
        {
            var _remark = await remarkRepo.GetById(remark.Id);
            if (_remark == null)
            {
                throw new AppException($"Invalid remark id {@remark.Id}");
            }

            else if (await remarkRepo.AnyAsync(r => (r.ExamId == remark.ExamId && r.StudentId == remark.StudentId) &&
            !(_remark.ExamId == remark.ExamId && _remark.StudentId == remark.StudentId)))
            {
                throw new AppException($"A remark already exist for specified exam and student");
            }
            else
            {
                var currentUser = accessor.HttpContext.GetUserSession();
                var _oldremark = _remark.Clone<PerformanceRemark>();

                _remark.ExamId = remark.ExamId;
                _remark.StudentId = remark.StudentId;
                _remark.TeacherRemark = remark.TeacherRemark;
                _remark.HeadTeacherRemark = remark.HeadTeacherRemark;
                _remark.UpdatedBy = currentUser.Username;
                _remark.UpdatedDate = DateTimeOffset.Now;

                await remarkRepo.Update(_remark, false);


                // log activity
                await logger.LogActivity(ActivityActionType.UPDATED_REMARK, currentUser.Username,
                    remarkRepo.TableName, _oldremark, _remark,
                     $"Updated remark");
            }
        }

        public IEnumerable<PerformanceRemark> GetRemarks()
        {
            return remarkRepo.GetAll();
        }

        public async Task<PerformanceRemark> GetRemark(long id)
        {
            return await remarkRepo.GetById(id);
        }

        public async Task<PerformanceRemark> GetRemark(long examId, long studentId)
        {
            return await remarkRepo.GetSingleWhere(r => r.ExamId == examId && r.StudentId == studentId);
        }
        // ============ Batch Upload ==================
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
                for (int i = 2; i < headers.Length; i++)
                {
                    if (row[i] == null || Convert.ToString(row[i]).Trim() == "")
                    {
                        isValid = false;
                        err = $"Invalid value for {headers[i]} at row {index}. Field is required.";
                    }
                    if (!isValid)
                        break;
                }
            }


            return (isValid, err);
        }

        public async Task<IEnumerable<PerformanceRemark>> ExtractData(IFormFile file)
        {
            List<PerformanceRemark> remarks = new List<PerformanceRemark>();
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
            if (!AppUtilities.ValidateHeader(header, headers, out string error))
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
                        var studentId = (await studentRepo.GetSingleWhere(s => s.AdmissionNo == Convert.ToString(rows[i][1]).Trim())).Id;
                        var record = new PerformanceRemark()
                        {
                            StudentId = studentId,
                            TeacherRemark = Convert.ToString(rows[i][2]).Trim(),
                            HeadTeacherRemark = Convert.ToString(rows[i][3]).Trim()
                        };
                        remarks.Add(record);
                    }
                }
            }
            fileStream.Dispose();
            return remarks;
        }

        public async Task BatchCreateRemarks(IEnumerable<PerformanceRemark> remarks, long examId)
        {   
            if (!await examRepo.AnyAsync(e => e.Id == examId))
            {
                throw new AppException("Exam id is invalid");
            }

            var currentUser = accessor.HttpContext.GetUserSession();

            var _remarks = new List<PerformanceRemark>();

            foreach (var r in remarks)
            {
                var student = await studentRepo.GetById(r.StudentId);

                r.ExamId = examId;
                r.CreatedBy = currentUser.Username;
                r.UpdatedBy = currentUser.Username;
                r.CreatedDate = DateTimeOffset.Now;
                r.UpdatedDate = DateTimeOffset.Now;

                // check for duplicate
                if (_remarks.Any(re => re.StudentId == r.StudentId))
                {
                    throw new AppException($"A student with admission number '{student.AdmissionNo}' already have an existing performance remark on excel");
                }

                if(await midTermResultRepo.AnyAsync(m=> m.ExamId == examId))
                {
                    if (!await midTermResultRepo.AnyAsync(mr => mr.ExamId == examId && mr.StudentId == student.Id))
                    {
                        throw new AppException($"A student with admission number '{student.AdmissionNo}' have no mid-term result for specified session and term");
                    }
                }
                else if (await endTermResultRepo.AnyAsync(m => m.ExamId == examId))
                {
                    if (!await endTermResultRepo.AnyAsync(er => er.ExamId == examId && er.StudentId == student.Id))
                    {
                        throw new AppException($"A student with admission number '{student.AdmissionNo}' have no end-term result for specified session and term");
                    }
                }

                if (await remarkRepo.AnyAsync(pr => pr.ExamId == examId && pr.StudentId == r.StudentId))
                {
                    throw new AppException($"A student with admission number '{student.AdmissionNo}' already have an existing performance remark");
                }

                _remarks.Add(r);
            }

            await remarkRepo.InsertRange(_remarks);

            // log action
            await logger.LogActivity(ActivityActionType.BATCH_ADDED_REMARK,
                currentUser.Username, $"Added performance remarks in batch");
        }

    }
}
