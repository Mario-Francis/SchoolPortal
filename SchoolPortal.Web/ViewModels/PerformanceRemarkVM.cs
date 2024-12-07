using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class PerformanceRemarkVM
    {
        public long Id { get; set; }
        [Required]
        public long ExamId { get; set; }
        [Required]
        public long StudentId { get; set; }
        public string TeacherRemark { get; set; }
        public string HeadTeacherRemark { get; set; }

        public string UpdatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        public  ExamVM Exam { get; set; }
        public StudentVM Student { get; set; }
        public string AdmissionNo { get; set; }
        public string Class { get; set; }
        public string StudentName { get; set; }
        public string ExamName { get; set; }

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

        public PerformanceRemark ToPerformanceRemark()
        {
            return new PerformanceRemark
            {
                Id = Id,
                ExamId = ExamId,
                StudentId = StudentId,
                TeacherRemark = TeacherRemark,
                HeadTeacherRemark = HeadTeacherRemark,
                CreatedDate = DateTimeOffset.Now,
                UpdatedDate = DateTimeOffset.Now
            };
        }

        public static PerformanceRemarkVM FromPerformanceRemark(PerformanceRemark remark, int? clientTimeOffset = null)
        {
            if (remark == null)
            {
                return null;
            }
            else
            {
                var student = StudentVM.FromStudent(remark.Student);
                var exam = ExamVM.FromExam(remark.Exam);
                return new PerformanceRemarkVM
                {
                    Id = remark.Id,
                    ExamId = remark.ExamId,
                    Exam = exam,
                    ExamName = $"{exam.Session} {exam.Term} Term ({exam.ExamType})",
                    StudentId = remark.StudentId,
                    Student = student,
                    StudentName = student.FullName,
                    AdmissionNo = student.AdmissionNo,
                    Class = student.ClassRoom.Class + " " + student.ClassRoom.RoomCode,
                    TeacherRemark = remark.TeacherRemark,
                    HeadTeacherRemark = remark.HeadTeacherRemark,
                    UpdatedBy = remark.UpdatedBy,
                    CreatedDate = clientTimeOffset == null ? remark.CreatedDate : remark.CreatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                    UpdatedDate = clientTimeOffset == null ? remark.UpdatedDate : remark.UpdatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value))
                };
            }
        }
    }
}
