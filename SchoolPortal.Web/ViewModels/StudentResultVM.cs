using SchoolPortal.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class StudentResultVM
    {
        public StudentMidTermResultVM MidTermResult { get; set; }
        public StudentEndTermResultVM EndTermResult { get; set; }

        public static StudentResultVM FromStudentResult(StudentResult result)
        {
            return new StudentResultVM
            {
                MidTermResult = StudentMidTermResultVM.FromStudentMidTermResult(result.MidTermResult),
                EndTermResult = StudentEndTermResultVM.FromStudentEndTermResult(result.EndTermResult)
            };
        }
    }
}
