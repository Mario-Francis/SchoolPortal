using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace SchoolPortal.Core.Models
{
    public class Grade:BaseEntity, IUpdatable
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public long TermSectionId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

        //Navigation Properties
        [JsonIgnore]
        [NotMapped]
        public virtual User UpdatedByUser { get; set; }
        public virtual TermSection TermSection { get; set; }
    }
}
