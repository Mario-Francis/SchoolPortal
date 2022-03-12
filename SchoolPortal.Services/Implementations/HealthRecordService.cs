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
    public class HealthRecordService: IHealthRecordService
    {
        private readonly IRepository<HealthRecord> healthRecordRepo;
        private readonly IRepository<Term> termRepo;
        private readonly IRepository<Student> studentRepo;
        private readonly ILoggerService<HealthRecordService> logger;
        private readonly IHttpContextAccessor accessor;
        private string[] headers = new string[] { "SN", "Student Admission No", "Start Height (mtrs)", "End Height (mtrs)", "Start Weight (kg)", "End Weight (kg)" };

        public HealthRecordService(
           IRepository<HealthRecord> healthRecordRepo,
           IRepository<Term> termRepo,
           IRepository<Student> studentRepo,
           ILoggerService<HealthRecordService> logger,
           IHttpContextAccessor accessor)
        {
            this.healthRecordRepo = healthRecordRepo;
            this.termRepo = termRepo;
            this.studentRepo = studentRepo;
            this.logger = logger;
            this.accessor = accessor;
        }

        // add
        public async Task CreateRecord(HealthRecord record)
        {
            if (record == null)
            {
                throw new AppException("Health record object cannot be null");
            }


            if (!await termRepo.AnyAsync(t => t.Id == record.TermId))
            {
                throw new AppException("Invalid term id");
            }

            if (!await studentRepo.AnyAsync(s => s.Id == record.StudentId))
            {
                throw new AppException("Invalid student id");
            }

            if (await healthRecordRepo.AnyAsync(r => r.Session == record.Session 
            && r.TermId == record.TermId && r.StudentId == record.StudentId))
            {
                throw new AppException($"A record already exist for specified session, term and student");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
            record.CreatedBy = currentUser.Username;
            record.CreatedDate = DateTimeOffset.Now;
            record.UpdatedBy = currentUser.Username;
            record.UpdatedDate = DateTimeOffset.Now;

            await healthRecordRepo.Insert(record, false);

            //log action
            await logger.LogActivity(ActivityActionType.CREATED_HEALTH_RECORD,
                currentUser.Username,
                $"Created new health record for student with id '{record.StudentId}'");
        }

        // delete 
        public async Task DeleteRecord(long recordId)
        {
            var record = await healthRecordRepo.GetById(recordId);
            if (record == null)
            {
                throw new AppException($"Invalid health record id {recordId}");
            }
            else
            {
                var _record = record.Clone<HealthRecord>();
                await healthRecordRepo.Delete(recordId, true);

                var currentUser = accessor.HttpContext.GetUserSession();
                // log activity
                await logger.LogActivity(ActivityActionType.DELETED_HEALTH_RECORD, currentUser.Username,
                    healthRecordRepo.TableName, _record, new HealthRecord(),
                    $"Deleted health record");
            }
        }

        // update 
        public async Task UpdateRecord(HealthRecord record)
        {
            var _record = await healthRecordRepo.GetById(record.Id);
            if (_record == null)
            {
                throw new AppException($"Invalid health record id {@record.Id}");
            }

            else if (await healthRecordRepo.AnyAsync(r => (r.Session == record.Session 
            && r.TermId == record.TermId && r.StudentId == record.StudentId) &&
            !(_record.Session == record.Session && _record.TermId==record.TermId && _record.StudentId == record.StudentId)))
            {
                throw new AppException($"A health record already exist for specified session, term and student");
            }
            else
            {
                var currentUser = accessor.HttpContext.GetUserSession();
                var _oldrecord = _record.Clone<HealthRecord>();

                _record.Session = record.Session;
                _record.TermId = record.TermId;
                _record.StudentId = record.StudentId;
                _record.StartHeight = record.StartHeight;
                _record.EndHeight = record.EndHeight;
                _record.StartWeight = record.StartWeight;
                 _record.EndWeight = record.EndWeight;
                _record.UpdatedBy = currentUser.Username;
                _record.UpdatedDate = DateTimeOffset.Now;

                await healthRecordRepo.Update(_record, false);


                // log activity
                await logger.LogActivity(ActivityActionType.UPDATED_HEALTH_RECORD, currentUser.Username,
                    healthRecordRepo.TableName, _oldrecord, _record,
                     $"Updated health record");
            }
        }

        public IEnumerable<HealthRecord> GetRecords()
        {
            return healthRecordRepo.GetAll();
        }

        public async Task<HealthRecord> GetRecord(long id)
        {
            return await healthRecordRepo.GetById(id);
        }

        public async Task<HealthRecord> GetRecord(string session, long termId, long studentId)
        {
            return await healthRecordRepo.GetSingleWhere(r => r.Session == session
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
            }else if (!await studentRepo.AnyAsync(s => s.AdmissionNo == Convert.ToString(row[1]).Trim()))
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

        public async Task<IEnumerable<HealthRecord>> ExtractData(IFormFile file)
        {
            List<HealthRecord> records = new List<HealthRecord>();
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
                        var record = new HealthRecord()
                        {
                            StudentId = studentId,
                            StartHeight = Convert.ToDecimal(Convert.ToString(rows[i][2]).Trim()),
                            EndHeight = Convert.ToDecimal(Convert.ToString(rows[i][3]).Trim()),
                            StartWeight = Convert.ToDecimal(Convert.ToString(rows[i][4]).Trim()),
                            EndWeight = Convert.ToDecimal(Convert.ToString(rows[i][5]).Trim()),
                        };
                        records.Add(record);
                    }
                }
            }
            fileStream.Dispose();
            return records;
        }

        public async Task BatchCreateRecords(IEnumerable<HealthRecord> records, string session, long termId)
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

            var _records = new List<HealthRecord>();

            foreach (var r in records)
            {
                var student = await studentRepo.GetById(r.StudentId);

                r.Session = session;
                r.TermId = termId;
                r.CreatedBy = currentUser.Username;
                r.UpdatedBy = currentUser.Username;
                r.CreatedDate = DateTimeOffset.Now;
                r.UpdatedDate = DateTimeOffset.Now;

                // check for duplicate
                if (_records.Any(re => re.StudentId == r.StudentId))
                {
                    throw new AppException($"A student with admission number '{student.AdmissionNo}' already have an existing health record on excel");
                }


                if (await healthRecordRepo.AnyAsync(hr => hr.Session == session && hr.TermId == termId && hr.StudentId == r.StudentId))
                {
                    throw new AppException($"A student with admission number '{student.AdmissionNo}' already have an existing health record");
                }

                _records.Add(r);
            }

            await healthRecordRepo.InsertRange(_records);

            // log action
            await logger.LogActivity(ActivityActionType.BATCH_ADDED_HEALTH_RECORD,
                currentUser.Username, $"Added health records in batch'");

        }

    }
}
