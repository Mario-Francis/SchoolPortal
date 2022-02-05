using Microsoft.AspNetCore.Http;
using SchoolPortal.Core;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Core.Models;
using SchoolPortal.Data.Repositories;
using System;
using System.Collections.Generic;
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

        public PerformanceRemarkService(
            IRepository<PerformanceRemark> remarkRepo,
            IRepository<Exam> examRepo,
            IRepository<Student> studentRepo,
            ILoggerService<PerformanceRemarkService> logger,
            IHttpContextAccessor accessor)
        {
            this.remarkRepo = remarkRepo;
            this.examRepo = examRepo;
            this.studentRepo = studentRepo;
            this.logger = logger;
            this.accessor = accessor;
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
    }
}
