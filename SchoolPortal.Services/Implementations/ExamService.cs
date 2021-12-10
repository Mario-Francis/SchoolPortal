using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SchoolPortal.Core;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Core.Models;
using SchoolPortal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services.Implementations
{
    public class ExamService:IExamService
    {
        private readonly IRepository<ExamType> examTypeRepo;
        private readonly IRepository<Exam> examRepo;
        private readonly IRepository<MidTermResult> midTermResultRepo;
        private readonly IRepository<EndTermResult> endTermResultRepo;
        private readonly IRepository<PerformanceRemark> performanceRemarkRepo;
        private readonly ILogger<ExamService> logger;
        private readonly IHttpContextAccessor accessor;

        public ExamService(IRepository<ExamType> examTypeRepo,
           IRepository<Exam> examRepo,
            IRepository<MidTermResult> midTermResultRepo,
            IRepository<EndTermResult> endTermResultRepo,
            IRepository<PerformanceRemark> performanceRemarkRepo,
           ILogger<ExamService> logger,
           IHttpContextAccessor accessor)
        {
            this.examTypeRepo = examTypeRepo;
            this.examRepo = examRepo;
            this.midTermResultRepo = midTermResultRepo;
            this.endTermResultRepo = endTermResultRepo;
            this.performanceRemarkRepo = performanceRemarkRepo;
            this.logger = logger;
            this.accessor = accessor;
        }

        // add exam
        public async Task CreateExam(Exam exam)
        {
            if (exam == null)
            {
                throw new AppException("Exam object cannot be null");
            }

            if (!AppUtilities.ValidateSession(exam.Session))
            {
                throw new AppException("Session is invalid");
            }

            if(exam.StartDate > exam.EndDate)
            {
                throw new AppException("Start date should be earlier than end date");
            }

            if (await examRepo.Any(e => e.ExamTypeId == exam.ExamTypeId && e.Session == exam.Session && e.TermId==e.TermId))
            {
                throw new AppException($"An exam for same term and session already exist");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
            exam.CreatedBy = currentUser.Username;
            exam.CreatedDate = DateTimeOffset.Now;
            exam.UpdatedBy = currentUser.Username;
            exam.UpdatedDate = DateTimeOffset.Now;

            await examRepo.Insert(exam, true);

            //log action
            //await loggerService.LogActivity(ActivityActionType.CREATED_ASSESSMENT,
            //    currentUser.PersonNumber,
            //    $"Created new assessment of type '{((Core.AssessmentType)((int)assessment.AssessmentTypeId)).ToString()}' for {assessment.FromDate.ToString("dd-MM-yyyy")} to {assessment.ToDate.ToString("dd-MM-yyyy")}");
        }

        // delete exams - without results
        public async Task DeleteExam(long examId)
        {
            var exam = await examRepo.GetById(examId);
            if (exam == null)
            {
                throw new AppException($"Invalid exam id {examId}");
            }
            else
            {
                if (await midTermResultRepo.CountWhere(r=>r.ExamId==examId) > 0 || 
                    await endTermResultRepo.CountWhere(r => r.ExamId == examId) > 0 || 
                    await performanceRemarkRepo.CountWhere(r => r.ExamId == examId) > 0)
                {
                    throw new AppException("Exam cannot be deleted as it has one or more entities attached to it");
                }
                else
                {
                    var _exam = exam.Clone<Exam>();
                    await examRepo.Delete(examId, true);

                    var currentUser = accessor.HttpContext.GetUserSession();
                    // log activity
                    //await loggerService.LogActivity(ActivityActionType.DELETED_ASSESSMENT, currentUser.PersonNumber,
                    //    $"Deleted assessment of type '{((Core.AssessmentType)((int)_assessment.AssessmentTypeId)).ToString()}' for {_assessment.FromDate.ToString("dd-MM-yyyy")} to {_assessment.ToDate.ToString("dd-MM-yyyy")}");
                }
            }
        }

        // update exam
        public async Task UpdateExam(Exam exam)
        {
            var _exam = await examRepo.GetById(exam.Id);
            if (_exam == null)
            {
                throw new AppException($"Invalid exam id {exam.Id}");
            }
            if (!AppUtilities.ValidateSession(exam.Session))
            {
                throw new AppException("Session is invalid");
            }
            if (exam.StartDate > exam.EndDate)
            {
                throw new AppException("Start date should be earlier than end date");
            }
            else if (await examRepo.Any(e => (e.ExamTypeId == exam.ExamTypeId && e.Session == exam.Session && e.TermId == e.TermId) &&
            !(_exam.ExamTypeId == exam.ExamTypeId && _exam.Session == exam.Session && _exam.TermId == exam.TermId)))
            {
                throw new AppException($"An exam for same term and session already exist");
            }
            else
            {
                var currentUser = accessor.HttpContext.GetUserSession();
                var _oldExam = _exam.Clone<Exam>();

                _exam.ExamTypeId = exam.ExamTypeId;
                _exam.Session = exam.Session;
                _exam.TermId = exam.TermId;
                _exam.StartDate = exam.StartDate;
                _exam.EndDate = exam.EndDate;
                _exam.UpdatedBy = currentUser.Username;
                _exam.UpdatedDate = DateTimeOffset.Now;

                await examRepo.Update(_exam, true);


                // log activity
                //await loggerService.LogActivity(ActivityActionType.UPDATED_exam, currentUser.PersonNumber,
                //    examRepo.TableName, _oldExam, _exam,
                //     $"Updated class of type '{((Core.classType)((int)_exam.classTypeId)).ToString()}' for {_exam.FromDate.ToString("dd-MM-yyyy")} to {_exam.ToDate.ToString("dd-MM-yyyy")}");

            }
        }

        public IEnumerable<Exam> GetExams()
        {
            return examRepo.GetAll().OrderBy(e => e.Session).ThenBy(e => e.Term).ThenBy(e => e.ExamTypeId);
        }

        public async Task<Exam> GetExam(long id)
        {
            return await examRepo.GetById(id);
        }

    }
}
