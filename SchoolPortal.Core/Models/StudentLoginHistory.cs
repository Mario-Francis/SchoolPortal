using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SchoolPortal.Core.Models
{
    public class StudentLoginHistory:BaseEntity
    {
        public long? StudentId { get; set; }
        public string IPAddress { get; set; }
        public DateTimeOffset LoginDate { get; set; }
        public string Status { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}
