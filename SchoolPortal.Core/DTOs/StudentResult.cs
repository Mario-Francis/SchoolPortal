using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Core.DTOs
{
    public class StudentResult
    {
        public StudentMidTermResult MidTermResult { get; set; }
        public StudentEndTermResult EndTermResult { get; set; }
    }
}
