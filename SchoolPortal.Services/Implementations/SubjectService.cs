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
    public class SubjectService:ISubjectService
    {
        private readonly IRepository<Subject> subjectRepo;
        private readonly ILogger<SubjectService> logger;
        private readonly IHttpContextAccessor accessor;

        public SubjectService(IRepository<Subject> subjectRepo,
            ILogger<SubjectService> logger,
            IHttpContextAccessor accessor)
        {
            this.subjectRepo = subjectRepo;
            this.logger = logger;
            this.accessor = accessor;
        }

        // add Subject
        public async Task CreateSubject(Subject subject)
        {
            if (subject == null)
            {
                throw new AppException("Subject object cannot be null");
            }

            if (await subjectRepo.Any(s=> s.ClassId ==subject.ClassId && s.Name.ToLower() == subject.Name.ToLower()))
            {
                throw new AppException($"A Subject of same class with name '{subject.Name}' already exist");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
            subject.CreatedBy = currentUser.Username;
            subject.CreatedDate = DateTimeOffset.Now;
            subject.UpdatedBy = currentUser.Username;
            subject.UpdatedDate = DateTimeOffset.Now;

            await subjectRepo.Insert(subject, true);

            //log action
            //await loggerService.LogActivity(ActivityActionType.CREATED_ASSESSMENT,
            //    currentUser.PersonNumber,
            //    $"Created new assessment of type '{((Core.AssessmentType)((int)assessment.AssessmentTypeId)).ToString()}' for {assessment.FromDate.ToString("dd-MM-yyyy")} to {assessment.ToDate.ToString("dd-MM-yyyy")}");
        }

        // delete Subject - without results
        public async Task DeleteSubject(long SubjectId)
        {
            var subject = await subjectRepo.GetById(SubjectId);
            if (subject == null)
            {
                throw new AppException($"Invalid Subject id {SubjectId}");
            }
            else
            {
                if (subject.MidTermResults.Count > 0 || subject.EndTermResults.Count > 0)
                {
                    throw new AppException("Subject cannot be deleted as it has one or more results attached to it");
                }
                else
                {
                    var _subject = subject.Clone<Subject>();
                    await subjectRepo.Delete(SubjectId, true);

                    var currentUser = accessor.HttpContext.GetUserSession();
                    // log activity
                    //await loggerService.LogActivity(ActivityActionType.DELETED_ASSESSMENT, currentUser.PersonNumber,
                    //    $"Deleted assessment of type '{((Core.AssessmentType)((int)_assessment.AssessmentTypeId)).ToString()}' for {_assessment.FromDate.ToString("dd-MM-yyyy")} to {_assessment.ToDate.ToString("dd-MM-yyyy")}");
                }
            }
        }

        // update Subject
        public async Task UpdateSubject(Subject subject)
        {
            var _subject = await subjectRepo.GetById(subject.Id);
            if (_subject == null)
            {
                throw new AppException($"Invalid Subject id {subject.Id}");
            }
            else if (await subjectRepo.Any(c => (c.ClassId == subject.ClassId && c.Name.ToLower() == subject.Name.ToLower()) &&
            !(_subject.ClassId == subject.ClassId && _subject.Name.ToLower() == subject.Name.ToLower())))
            {
                throw new AppException($"A Subject of same class with name '{subject.Name}' already exist");
            }
            else
            {
                var currentUser = accessor.HttpContext.GetUserSession();
                var _oldSubject = _subject.Clone<Subject>();

                _subject.ClassId = subject.ClassId;
                _subject.Name = subject.Name;
                _subject.Code = subject.Code;
                _subject.Description = subject.Description;
                _subject.UpdatedBy = currentUser.Username;
                _subject.UpdatedDate = DateTimeOffset.Now;

                await subjectRepo.Update(_subject, true);


                // log activity
                //await loggerService.LogActivity(ActivityActionType.UPDATED_subject, currentUser.PersonNumber,
                //    subjectRepo.TableName, _oldSubject, _subject,
                //     $"Updated Subject of type '{((Core.SubjectType)((int)_subject.SubjectTypeId)).ToString()}' for {_subject.FromDate.ToString("dd-MM-yyyy")} to {_subject.ToDate.ToString("dd-MM-yyyy")}");

            }
        }

        public IEnumerable<Subject> GetSubjects()
        {
            return subjectRepo.GetAll().OrderBy(c => c.ClassId).ThenBy(c => c.Name);
        }
        // get by Subject id
        public IEnumerable<Subject> GetSubjects(long classId)
        {
            return subjectRepo.GetWhere(s=>s.ClassId == classId).OrderBy(c => c.Name);
        }

        public async Task<Subject> GetSubject(long id)
        {
            return await subjectRepo.GetById(id);
        }

    }
}
