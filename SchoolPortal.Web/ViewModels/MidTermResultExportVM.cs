using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class MidTermResultExportVM
    {
        // result
        public IEnumerable<StudentResultItemVM> ResultItems { get; set; }
        public decimal TotalScoreObtained { get; set; }
        public decimal TotalScoreObtainable { get; set; }
        public decimal Percentage { get; set; }
        public string PercentageGrade { get; set; }

        // student
        public StudentVM Student { get; set; }

        // Class room
        public ClassRoomVM ClassRoom { get; set; }
        // exam
        public ExamVM Exam { get; set; }

        // grade system
        public IEnumerable<GradeVM> Grades { get; set; }

        // comments
        public string TeacherComment { get; set; }
        public string HeadTeacherComment { get; set; }
    }
}
