using Microsoft.AspNetCore.Http;
using SchoolPortal.Core;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Core.Models;
using SchoolPortal.Data.Repositories;
using System;
using System.Collections.Generic;
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
    }
}
