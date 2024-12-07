using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace SchoolPortal.Core.Models
{
    public class Class:BaseEntity,IUpdatable
    {
        public long ClassTypeId { get; set; }
        public int ClassGrade { get; set; }

        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

        //Navigation Properties
        [JsonIgnore]
        [NotMapped]
        public virtual User UpdatedByUser { get; set; }
        [ForeignKey("ClassTypeId")]
        public virtual ClassType ClassType { get; set; }
        public virtual ICollection<ClassRoom> ClassRooms { get; set; }
        public virtual ICollection<Subject> Subjects { get; set; }
    }
}
