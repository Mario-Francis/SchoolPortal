﻿using SchoolPortal.Core.Models;
using SchoolPortal.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class EndTermResultVM
    {
        public long Id { get; set; }
        [Required]
        public long ExamId { get; set; }
        [Required]
        public long ClassId { get; set; }
        [Required]
        public long SubjectId { get; set; }
        [Required]
        public long StudentId { get; set; }
        [Required]
        [Range(0, 10)]
        public decimal ClassWorkScore { get; set; }
        [Required]
        [Range(0, 10)]
        public decimal TestScore { get; set; }
        [Required]
        [Range(0, 40)]
        public decimal ExamScore { get; set; }
        [Required]
        [Range(0, 60)]
        public decimal Total { get; set; }

        public string ExamName { get; set; }
        public string ClassName { get; set; }
        public string SubjectName { get; set; }
        public string StudentName { get; set; }
        public string RoomCode { get; set; }
        public string Grade { get; set; }

        public ExamVM Exam { get; set; }
        public SubjectVM Subject { get; set; }
        public StudentItemVM Student { get; set; }

        public string UpdatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }


        public string FormattedCreatedDate
        {
            get
            {
                return CreatedDate.ToString("MMM d, yyyy");
            }
        }
        public string FormattedUpdatedDate
        {
            get
            {
                return UpdatedDate.ToString("MMM d, yyyy");
            }
        }

        public EndTermResult ToEndTermResult()
        {
            return new EndTermResult
            {
                Id = Id,
                ExamId=ExamId,
                SubjectId=SubjectId,
                StudentId=StudentId,
                ClassId=ClassId,
                ClassWorkScore=ClassWorkScore,
                TestScore=TestScore,
                ExamScore=ExamScore,
                Total=Total,
                CreatedDate = DateTimeOffset.Now,
                UpdatedDate = DateTimeOffset.Now
            };
        }

        public static EndTermResultVM FromEndTermResult(EndTermResult result, int? clientTimeOffset = null, IGradeService gradeService = null)
        {
            return new EndTermResultVM
            {
                Id = result.Id,
                ExamId= result.ExamId,
                SubjectId=result.SubjectId,
                StudentId= result.StudentId,
                ClassWorkScore= result.ClassWorkScore,
                TestScore=result.TestScore,
                ExamScore=result.ExamScore,
                Total=result.Total,
                ClassId=result.ClassId,
                ClassName=$"{result.Class.ClassType.Name} {result.Class.ClassGrade}",
                RoomCode=result.ClassRoom.RoomCode,
                Exam=ExamVM.FromExam(result.Exam),
                ExamName=$"{result.Exam.ExamType.Name} ({result.Exam.StartDate.ToString("MMM d, yyyy")} - {result.Exam.EndDate.ToString("MMM d, yyyy")})",
                Student=StudentItemVM.FromStudent(result.Student),
                StudentName= StudentVM.FromStudent(result.Student).FullName,
                Subject=SubjectVM.FromSubject(result.Subject),
                SubjectName=$"{result.Subject.Name} {(result.Subject.Code==null?"":$"({result.Subject.Code})")}".Trim(),
                UpdatedBy = result.UpdatedBy,
                CreatedDate = clientTimeOffset == null ? result.CreatedDate : result.CreatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                UpdatedDate = clientTimeOffset == null ? result.UpdatedDate : result.UpdatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                Grade = gradeService == null ? result.Total.ToString() : gradeService.GetGrade(result.Total, Core.TermSections.FIRST_HALF).Code
            };
        }
    }
}
