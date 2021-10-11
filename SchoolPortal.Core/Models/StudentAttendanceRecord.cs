using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SchoolPortal.Core.Models
{
    public class StudentAttendanceRecord:BaseEntity
    {
        public string Session { get; set; }
        public long TermId { get; set; }
        public long StudentId { get; set; }
        public DateTimeOffset ClassDate { get; set; }

        [ForeignKey("TermId")]
        public virtual Term Term { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}
