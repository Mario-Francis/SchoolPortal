using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SchoolPortal.Core.Models.Views
{
    public class EndOfSessionResultViewObject:BaseEntity
    {
        public long? FirstMidTermResultId { get; set; }
        public long? FirstEndTermResultId { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? FirstMidTermTotal { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? FirstEndTermTotal { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? FirstTermTotal { get; set; }

        public long? SecondMidTermResultId { get; set; }
        public long? SecondEndTermResultId { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? SecondMidTermTotal { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? SecondEndTermTotal { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? SecondTermTotal { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TermTotal { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? AverageScore { get; set; }

        public long ExamId { get; set; }
        public long ClassId { get; set; }
        public long ClassRoomId { get; set; }
        public long SubjectId { get; set; }
        public long StudentId { get; set; }
        public long? MidTermResultId { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? MidTermTotal { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal ClassWorkScore { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TestScore { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal ExamScore { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Total { get; set; }

        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public string Session { get; set; }
        public string Term { get; set; }
        public long TermId { get; set; }
        public long ExamTypeId { get; set; }
        public string ExamType { get; set; }
        public string ClassName { get; set; }
        public string RoomCode { get; set; }
        public string SubjectName { get; set; }

        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

        //Navigation Properties
        [NotMapped]
        public virtual User UpdatedByUser { get; set; }
        [NotMapped]
        public virtual Exam Exam { get; set; }
        [NotMapped]
        public virtual Subject Subject { get; set; }
        [NotMapped]
        public virtual Student Student { get; set; }
        [NotMapped]
        public virtual Class Class { get; set; }
        [NotMapped]
        public virtual ClassRoom ClassRoom { get; set; }
    }
}
