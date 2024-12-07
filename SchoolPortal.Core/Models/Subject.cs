using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace SchoolPortal.Core.Models
{
    public class Subject:BaseEntity,IUpdatable
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public long ClassId { get; set; }
        public bool IsActive { get; set; }

        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

        //Navigation Properties
        [JsonIgnore]
        [NotMapped]
        public virtual User UpdatedByUser { get; set; }
        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        public virtual ICollection<MidTermResult> MidTermResults { get; set; }
        public virtual ICollection<EndTermResult> EndTermResults { get; set; }
    }
}
