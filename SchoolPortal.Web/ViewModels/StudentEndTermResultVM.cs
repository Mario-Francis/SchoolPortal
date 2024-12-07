using SchoolPortal.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class StudentEndTermResultVM
    {
        public ItemVM Term { get; set; }
        public string Session { get; set; }
        public ClassRoomVM ClassRoom { get; set; }
        public ExamVM Exam { get; set; }
        public ResultCommentVM ResultComment { get; set; }
        public HealthRecordVM HealthRecord { get; set; }
        public AttendanceRecordVM AttendanceRecord { get; set; }
        public IEnumerable<BehaviouralResultVM> BehaviouralResults { get; set; }

        public static StudentEndTermResultVM FromStudentEndTermResult(StudentEndTermResult result)
        {
            if (result == null)
            {
                return null;
            }
            else
            {
                return new StudentEndTermResultVM
                {
                    ClassRoom = ClassRoomVM.FromClassRoom(result.ClassRoom),
                    Exam = ExamVM.FromExam(result.Exam),
                    ResultComment = ResultCommentVM.FromResultCommentObject(result.ResultComment),
                    Session = result.Session,
                    Term = new ItemVM { Id = result.Term.Id, Name = result.Term.Name },
                    HealthRecord = HealthRecordVM.FromHealthRecordObject(result.HealthRecord),
                    AttendanceRecord=AttendanceRecordVM.FromAttendanceRecordObject(result.AttendanceRecord),
                    BehaviouralResults = result.BehaviouralResults.Select(r => BehaviouralResultVM.FromBehaviouralResult(r))
                };
            }
        }
    }
}
