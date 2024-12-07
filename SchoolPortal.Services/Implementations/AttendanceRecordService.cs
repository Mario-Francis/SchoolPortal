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
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services.Implementations
{
    public class AttendanceRecordService:IAttendanceRecordService
    {
        private readonly IRepository<AttendanceRecord> attendanceRecordRepo;
        private readonly IRepository<Term> termRepo;
        private readonly IRepository<Student> studentRepo;
        private readonly ILoggerService<AttendanceRecordService> logger;
        private readonly IHttpContextAccessor accessor;
        private readonly IRepository<EndTermResult> endTermResultRepo;
        private string[] headers = new string[] { "SN", "Student Admission No", "No. of times present" };

        public AttendanceRecordService(
           IRepository<AttendanceRecord> attendanceRecordRepo,
           IRepository<Term> termRepo,
           IRepository<Student> studentRepo,
           ILoggerService<AttendanceRecordService> logger,
           IHttpContextAccessor accessor,
           IRepository<EndTermResult> endTermResultRepo)
        {
            this.attendanceRecordRepo = attendanceRecordRepo;
            this.termRepo = termRepo;
            this.studentRepo = studentRepo;
            this.logger = logger;
            this.accessor = accessor;
            this.endTermResultRepo = endTermResultRepo;
        }

        // add
        public async Task CreateRecord(AttendanceRecord record)
        {
            if (record == null)
            {
                throw new AppException("Attendance record object cannot be null");
            }


            if (!await termRepo.AnyAsync(t => t.Id == record.TermId))
            {
                throw new AppException("Invalid term id");
            }

            if (!await studentRepo.AnyAsync(s => s.Id == record.StudentId))
            {
                throw new AppException("Invalid student id");
            }

            if (await attendanceRecordRepo.AnyAsync(r => r.Session == record.Session
            && r.TermId == record.TermId && r.StudentId == record.StudentId))
            {
                throw new AppException($"A record already exist for specified session, term and student");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
            record.CreatedBy = currentUser.Username;
            record.CreatedDate = DateTimeOffset.Now;
            record.UpdatedBy = currentUser.Username;
            record.UpdatedDate = DateTimeOffset.Now;

            await attendanceRecordRepo.Insert(record, false);

            //log action
            await logger.LogActivity(ActivityActionType.CREATED_ATTENDANCE_RECORD,
                currentUser.Username,
                $"Created new attendance record for student with id '{record.StudentId}'");
        }

        // delete 
        public async Task DeleteRecord(long recordId)
        {
            var record = await attendanceRecordRepo.GetById(recordId);
            if (record == null)
            {
                throw new AppException($"Invalid attendance record id {recordId}");
            }
            else
            {
                var _record = record.Clone<AttendanceRecord>();
                await attendanceRecordRepo.Delete(recordId, true);

                var currentUser = accessor.HttpContext.GetUserSession();
                // log activity
                await logger.LogActivity(ActivityActionType.DELETED_ATTENDANCE_RECORD, currentUser.Username,
                    attendanceRecordRepo.TableName, _record, new AttendanceRecord(),
                    $"Deleted attendance record");
            }
        }

        // update 
        public async Task UpdateRecord(AttendanceRecord record)
        {
            var _record = await attendanceRecordRepo.GetById(record.Id);
            if (_record == null)
            {
                throw new AppException($"Invalid attendance record id {@record.Id}");
            }

            else if (await attendanceRecordRepo.AnyAsync(r => (r.Session == record.Session
            && r.TermId == record.TermId && r.StudentId == record.StudentId) &&
            !(_record.Session == record.Session && _record.TermId == record.TermId && _record.StudentId == record.StudentId)))
            {
                throw new AppException($"A attendance record already exist for specified session, term and student");
            }
            else
            {
                var currentUser = accessor.HttpContext.GetUserSession();
                var _oldrecord = _record.Clone<AttendanceRecord>();

                _record.Session = record.Session;
                _record.TermId = record.TermId;
                _record.StudentId = record.StudentId;
                _record.SchoolOpenCount = record.SchoolOpenCount;
                _record.PresentCount = record.PresentCount;
                _record.UpdatedBy = currentUser.Username;
                _record.UpdatedDate = DateTimeOffset.Now;

                await attendanceRecordRepo.Update(_record, false);


                // log activity
                await logger.LogActivity(ActivityActionType.UPDATED_ATTENDANCE_RECORD, currentUser.Username,
                    attendanceRecordRepo.TableName, _oldrecord, _record,
                     $"Updated attendance record");
            }
        }

        public IEnumerable<AttendanceRecord> GetRecords()
        {
            return attendanceRecordRepo.GetAll();
        }

        public async Task<AttendanceRecord> GetRecord(long id)
        {
            return await attendanceRecordRepo.GetById(id);
        }

        public async Task<AttendanceRecord> GetRecord(string session, long termId, long studentId)
        {
            return await attendanceRecordRepo.GetSingleWhereAsync(r => r.Session == session
            && r.TermId == termId && r.StudentId == studentId);
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

                    if (!AppUtilities.IsValidDecimal(Convert.ToString(row[i]).Trim()))
                    {
                        isValid = false;
                        err = $"Invalid value for {headers[i]} at row {index}. Field is not a valid decimal value.";
                    }
                    if (!isValid)
                        break;
                }
            }


            return (isValid, err);
        }

        public async Task<IEnumerable<AttendanceRecord>> ExtractData(IFormFile file)
        {
            List<AttendanceRecord> records = new List<AttendanceRecord>();
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
                        var studentId = (await studentRepo.GetSingleWhereAsync(s => s.AdmissionNo == Convert.ToString(rows[i][1]).Trim())).Id;
                        var record = new AttendanceRecord()
                        {
                            StudentId = studentId,
                            PresentCount = Convert.ToInt32(Convert.ToString(rows[i][2]).Trim())
                        };
                        records.Add(record);
                    }
                }
            }
            fileStream.Dispose();
            return records;
        }

        public async Task BatchCreateRecords(IEnumerable<AttendanceRecord> records, string session, long termId, int schoolOpenCount)
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

            var _records = new List<AttendanceRecord>();

            foreach (var r in records)
            {
                var student = await studentRepo.GetById(r.StudentId);

                r.Session = session;
                r.TermId = termId;
                r.SchoolOpenCount = schoolOpenCount;
                r.CreatedBy = currentUser.Username;
                r.UpdatedBy = currentUser.Username;
                r.CreatedDate = DateTimeOffset.Now;
                r.UpdatedDate = DateTimeOffset.Now;

                // check for duplicate
                if (_records.Any(re => re.StudentId == r.StudentId))
                {
                    throw new AppException($"A student with admission number '{student.AdmissionNo}' already have an existing attendance record on excel");
                }

                if (!await endTermResultRepo.AnyAsync(er => er.Exam.Session == session && er.Exam.TermId == termId && er.StudentId == student.Id))
                {
                    throw new AppException($"A student with admission number '{student.AdmissionNo}' have no end-term result for specified session and term");
                }

                if (await attendanceRecordRepo.AnyAsync(hr => hr.Session == session && hr.TermId == termId && hr.StudentId == r.StudentId))
                {
                    throw new AppException($"A student with admission number '{student.AdmissionNo}' already have an existing attendance record");
                }

                _records.Add(r);
            }

            await attendanceRecordRepo.InsertRange(_records);

            // log action
            await logger.LogActivity(ActivityActionType.BATCH_ADDED_ATTENDANCE_RECORD,
                currentUser.Username, $"Added attendance records in batch'");

        }

    }
}
