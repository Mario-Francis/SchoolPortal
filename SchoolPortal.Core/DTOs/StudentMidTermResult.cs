using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Core.DTOs
{
    public class StudentMidTermResult
    {
        //public IEnumerable<StudentResult> Results { get; set; }
        public Term Term { get; set; }
        public string Session { get; set; }
        public ClassRoom ClassRoom { get; set; }
        public Exam Exam { get; set; }
        public ResultCommentObject ResultComment { get; set; }

    }
}
