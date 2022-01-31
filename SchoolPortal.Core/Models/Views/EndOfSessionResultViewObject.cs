using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SchoolPortal.Core.Models.Views
{
    public class EndOfSessionResultViewObject:BaseEntity
    {
        public long FirstMidTermResultId { get; set; }
        public long FirstEndTermResultId { get; set; }
        public decimal FirstMidTermTotal { get; set; }
        public decimal FirstEndTermTotal { get; set; }
        public decimal FirstTermTotal { get; set; }

        public long SecondMidTermResultId { get; set; }
        public long SecondEndTermResultId { get; set; }
        public decimal SecondMidTermTotal { get; set; }
        public decimal SecondEndTermTotal { get; set; }
        public decimal SecondTermTotal { get; set; }

        public decimal TermTotal { get; set; }
        public decimal AverageScore { get; set; }

        public long ExamId { get; set; }
        public long ClassId { get; set; }
        public long ClassRoomId { get; set; }
        public long SubjectId { get; set; }
        public long StudentId { get; set; }
        public long MidTermResultId { get; set; }
        public decimal MidTermTotal { get; set; }
        public decimal ClassWorkScore { get; set; }
        public decimal TestScore { get; set; }
        public decimal ExamScore { get; set; }
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
