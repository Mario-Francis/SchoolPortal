using SchoolPortal.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class StudentMidTermResultVM
    {
        public ItemVM Term { get; set; }
        public string Session { get; set; }
        public ClassRoomVM ClassRoom { get; set; }
        public ExamVM Exam { get; set; }
        public ResultCommentVM ResultComment { get; set; }

        public static StudentMidTermResultVM FromStudentMidTermResult(StudentMidTermResult result)
        {
            return new StudentMidTermResultVM
            {
                ClassRoom = ClassRoomVM.FromClassRoom(result.ClassRoom),
                Exam = ExamVM.FromExam(result.Exam),
                ResultComment = ResultCommentVM.FromResultCommentObject(result.ResultComment),
                Session = result.Session,
                Term = new ItemVM { Id = result.Term.Id, Name = result.Term.Name }
            };
        }
    }
}
