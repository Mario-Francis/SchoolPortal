using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace SchoolPortal.Core.Models
{
    public class ClassRoomTeacher:BaseEntity,IUpdatable
    {
        public long ClassRoomId { get; set; }
        public long TeacherId { get; set; }

        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

        //Navigation Properties
        [JsonIgnore]
        [NotMapped]
        public virtual User UpdatedByUser { get; set; }
        [ForeignKey("ClassRoomId")]
        public virtual ClassRoom ClassRoom { get; set; }
        [ForeignKey("TeacherId")]
        public virtual User Teacher { get; set; }
    }
}
