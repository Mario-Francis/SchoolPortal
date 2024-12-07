using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace SchoolPortal.Core.Models
{
    public class MidTermResult:BaseEntity,IUpdatable
    {
        public long ExamId { get; set; }
        public long ClassId { get; set; }
        public long ClassRoomId { get; set; }
        public long SubjectId { get; set; }
        public long StudentId { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal ClassWorkScore { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TestScore { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal ExamScore { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Total { get; set; }

        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

        //Navigation Properties
        [JsonIgnore]
        [NotMapped]
        public virtual User UpdatedByUser { get; set; }
        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }
        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }
        [ForeignKey("ClassRoomId")]
        public virtual ClassRoom ClassRoom { get; set; }
    }
}
