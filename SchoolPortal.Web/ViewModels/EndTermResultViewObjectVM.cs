using SchoolPortal.Core.Models.Views;
using SchoolPortal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class EndTermResultViewObjectVM
    {
        public long Id { get; set; }
        public long ExamId { get; set; }
        public long ClassId { get; set; }
        public long ClassRoomId { get; set; }
        public long SubjectId { get; set; }
        public long StudentId { get; set; }
        public long? MidTermResultId { get; set; }
        public decimal? MidTermTotal { get; set; }
        public decimal ClassWorkScore { get; set; }
        public decimal TestScore { get; set; }
        public decimal ExamScore { get; set; }
        public decimal Total { get; set; }
        public decimal TermTotal { get; set; }

        public string Student { get; set; }
        public string AdmissionNo { get; set; }
        public string Session { get; set; }
        public string Term { get; set; }
        public long ExamTypeId { get; set; }
        public string ExamType { get; set; }
        public string Class { get; set; }
        public string RoomCode { get; set; }
        public string Subject { get; set; }
        public string Grade { get; set; }

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


        public static EndTermResultViewObjectVM FromEndTermResultViewObject(EndTermResultViewObject result, 
            int? clientTimeOffset = null, IGradeService gradeService = null)
        {
            if (result == null)
                return null;
            else
            {
                var termTotal = result.MidTermTotal != null ? (result.MidTermTotal + result.Total) : Math.Round(((result.Total / 60) * 100), MidpointRounding.AwayFromZero);
                return new EndTermResultViewObjectVM
                {
                    Id = result.Id,
                    AdmissionNo = result.AdmissionNo,
                    Class = result.ClassName,
                    ClassId = result.ClassId,
                    ClassRoomId = result.ClassRoomId,
                    ClassWorkScore = result.ClassWorkScore,
                    ExamId = result.ExamId,
                    ExamScore = result.ExamScore,
                    ExamType = result.ExamType,
                    ExamTypeId = result.ExamTypeId,
                    RoomCode = result.RoomCode,
                    Session = result.Session,
                    Student = result.StudentName,
                    StudentId = result.StudentId,
                    Subject = result.SubjectName,
                    SubjectId = result.SubjectId,
                    Term = result.Term,
                    TestScore = result.TestScore,
                    Total = result.Total,
                    MidTermTotal = result.MidTermTotal,
                    MidTermResultId = result.MidTermResultId,
                    TermTotal = termTotal.Value,
                    UpdatedBy = result.UpdatedBy,
                    CreatedDate = clientTimeOffset == null ? result.CreatedDate : result.CreatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                    UpdatedDate = clientTimeOffset == null ? result.UpdatedDate : result.UpdatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                    Grade = gradeService == null ? (result.MidTermTotal + result.Total).ToString() : gradeService.GetGrade(termTotal.Value, Core.TermSections.SECOND_HALF).Code
                };
            }
               
        }
    }
}
