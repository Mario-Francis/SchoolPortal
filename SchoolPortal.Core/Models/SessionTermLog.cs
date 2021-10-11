using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SchoolPortal.Core.Models
{
    public class SessionTermLog:BaseEntity
    {
        public string Session { get; set; }
        public long TermId { get; set; }
        public long TermSectionId { get; set; } //Mid-Term, End-Term
        public bool IsCurrent { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }

        //Navigation Properties
        [ForeignKey("TermId")]
        public virtual Term Term { get; set; }
        [ForeignKey("TermSectionId")]
        public virtual TermSection TermSection { get; set; }
    }
}
