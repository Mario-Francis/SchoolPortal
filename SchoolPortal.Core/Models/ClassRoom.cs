using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace SchoolPortal.Core.Models
{
    public class ClassRoom:BaseEntity,IUpdatable
    {
        public long ClassId { get; set; }
        public string RoomCode { get; set; }
        public bool IsActive { get; set; }

        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

        //Navigation Properties
        [JsonIgnore]
        [NotMapped]
        public virtual User UpdatedByUser { get; set; }
        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }
        public virtual ICollection<ClassRoomStudent> ClassRoomStudents { get; set; }
        public virtual ICollection<ClassRoomTeacher> ClassRoomTeachers { get; set; }
        public virtual ICollection<Class> CourseWorks { get; set; }
    }

}
