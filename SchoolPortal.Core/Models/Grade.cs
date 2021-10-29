using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Core.Models
{
    public class Grade:BaseEntity
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public long TermSectionId { get; set; }

        //Niavigationn Property
        public virtual TermSection TermSection { get; set; }
    }
}
