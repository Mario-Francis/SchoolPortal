using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace SchoolPortal.Core.Models
{
    public class HealthRecord:BaseEntity,IUpdatable
    {
        public string Session { get; set; }
        public long TermId { get; set; }
        public long StudentId { get; set; }
        public decimal StartHeight { get; set; }
        public decimal? EndHeight { get; set; }
        public decimal StartWeight { get; set; }
        public decimal? EndWeight { get; set; }

        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

        //Navigation Properties
        [JsonIgnore]
        [NotMapped]
        public virtual User UpdatedByUser { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
        [ForeignKey("TermId")]
        public virtual Term Term { get; set; }
    }
}
