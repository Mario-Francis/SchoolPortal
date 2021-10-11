using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Core.Models
{
    public class BehaviouralRating:BaseEntity
    {
        public string Name { get; set; }
        public string Category { get; set; } // Affective_Domain, Psychomotor_Domain
    }
}
