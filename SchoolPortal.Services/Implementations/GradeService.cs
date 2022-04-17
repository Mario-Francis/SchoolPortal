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
    public class GradeService: IGradeService
    {
        private readonly IRepository<Grade> gradeRepo;
        private readonly ILoggerService<GradeService> logger;
        private readonly IHttpContextAccessor accessor;

        public GradeService(IRepository<Grade> gradeRepo,
             ILoggerService<GradeService> logger,
            IHttpContextAccessor accessor)
        {
            this.gradeRepo = gradeRepo;
            this.logger = logger;
            this.accessor = accessor;
        }

        // add grade
        public async Task AddGrade(Grade grade)
        {
            if (grade == null)
            {
                throw new AppException("Grade object cannot be null");
            }

            if (await gradeRepo.AnyAsync(g => g.TermSectionId == grade.TermSectionId && g.Code == grade.Code))
            {
                throw new AppException($"A grade with specified code already exist");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
            grade.CreatedBy = currentUser.Username;
            grade.CreatedDate = DateTimeOffset.Now;
            grade.UpdatedBy = currentUser.Username;
            grade.UpdatedDate = DateTimeOffset.Now;

            await gradeRepo.Insert(grade);

            // log action
            await logger.LogActivity(ActivityActionType.ADDED_GRADE, currentUser.Username, gradeRepo.TableName,
                new Grade(), grade, $"Created new grade '{grade.Description} - {grade.Code}'");
        }

        // update grade
        public async Task UpdateGrade(Grade grade)
        {
            if (grade == null)
            {
                throw new AppException("Grade object cannot be null");
            }

            var _grade = await gradeRepo.GetById(grade.Id);
            if (_grade == null)
            {
                throw new AppException("Invalid grade id");
            }

            if (await gradeRepo.AnyAsync(g => g.Id != grade.Id && g.TermSectionId == grade.TermSectionId && g.Code == grade.Code))
            {
                throw new AppException($"A grade with specified code already exist");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
           
            var _oldGrade = _grade.Clone<Grade>();

            _grade.TermSectionId = grade.TermSectionId;
            _grade.Code = grade.Code;
            _grade.Description = grade.Description;
            _grade.From = grade.From;
            _grade.To = grade.To;
            _grade.UpdatedBy = currentUser.Username;
            _grade.UpdatedDate = DateTimeOffset.Now;

            await gradeRepo.Update(_grade);

            // log action
            await logger.LogActivity(ActivityActionType.UPDATED_GRADE, currentUser.Username, gradeRepo.TableName,
                _oldGrade, _grade, $"Updated grade '{grade.Description} - {grade.Code}'");
        }

        // delete grade
        public async Task DeleteGrade(long gradeId)
        {
            var grade = await gradeRepo.GetById(gradeId);
            if (grade == null)
            {
                throw new AppException("Invalid grade id");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
            var _oldGrade = grade.Clone<Grade>();

            await gradeRepo.Delete(gradeId);

            // log action
            await logger.LogActivity(ActivityActionType.DELETED_GRADE, currentUser.Username, gradeRepo.TableName,
                _oldGrade, new Grade(), $"Deleted grade");
        }
        // get  grade
        public async Task<Grade> GetGrade(long gradeId)
        {
            var grade = await gradeRepo.GetById(gradeId);
            return grade;
        }

        public Grade GetGrade(decimal score, TermSections section)
        {
            var grade = gradeRepo.GetSingleWhere(g => g.TermSectionId == 
            (int)section && g.From <= score && g.To >= score);

            return grade;
        }

        public async Task<Grade> GetGradeAsync(decimal score, TermSections section)
        {
            var grade = await gradeRepo.GetSingleWhereAsync(g => g.TermSectionId ==
            (int)section && g.From <= score && g.To >= score);

            return grade;
        }

        // get grades
        public IEnumerable<Grade> GetGrades()
        {
            var grades = gradeRepo.GetAll().OrderByDescending(g=>g.From);

            return grades;
        }

        public IEnumerable<Grade> GetGrades(TermSections section)
        {
            var grades = gradeRepo.GetWhere(g=> g.TermSectionId == (int)section)
                .OrderByDescending(g => g.From);

            return grades;
        }
    }
}
