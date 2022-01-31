using SchoolPortal.Core.Models;
using SchoolPortal.Core.Models.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Core.DTOs
{
    public class StudentResultItem
    {
        public long Id { get; set; }
        public long SubjectId { get; set; }
        public decimal MidTermTotal { get; set; }
        public decimal ClassWorkScore { get; set; }
        public decimal TestScore { get; set; }
        public decimal ExamScore { get; set; }
        public decimal Total { get; set; }

        public string SubjectName { get; set; }
        public virtual Subject Subject { get; set; }

        #region End of session fields
        //public long FirstMidTermResultId { get; set; }
        //public long FirstEndTermResultId { get; set; }
        //public decimal FirstMidTermTotal { get; set; }
        //public decimal FirstEndTermTotal { get; set; }
        public decimal FirstTermTotal { get; set; }

        //public long SecondMidTermResultId { get; set; }
        //public long SecondEndTermResultId { get; set; }
        //public decimal SecondMidTermTotal { get; set; }
        //public decimal SecondEndTermTotal { get; set; }
        public decimal SecondTermTotal { get; set; }

        public decimal TermTotal { get; set; }
        public decimal AverageScore { get; set; }

        #endregion End of session fields
        public static StudentResultItem FromMidTermResult(MidTermResult result)
        {
            return new StudentResultItem
            {
                Id = result.Id,
                SubjectId = result.SubjectId,
                ClassWorkScore = result.ClassWorkScore,
                TestScore = result.TestScore,
                ExamScore = result.ExamScore,
                Total = result.Total,
                Subject = result.Subject,
                SubjectName=result.Subject.Name
            };
        }

        public static StudentResultItem FromEndTermResultViewObject(EndTermResultViewObject result)
        {
            return new StudentResultItem
            {
                Id = result.Id,
                SubjectId = result.SubjectId,
                ClassWorkScore = result.ClassWorkScore,
                TestScore = result.TestScore,
                ExamScore = result.ExamScore,
                Total = result.Total,
                SubjectName = result.SubjectName,
                Subject = result.Subject,
                MidTermTotal = result.MidTermTotal,
                TermTotal = result.MidTermTotal + result.Total
            };
        }

        public static StudentResultItem FromEndOfSessionResultViewObject(EndOfSessionResultViewObject result)
        {
            return new StudentResultItem
            {
                Id = result.Id,
                SubjectId = result.SubjectId,
                ClassWorkScore = result.ClassWorkScore,
                TestScore = result.TestScore,
                ExamScore = result.ExamScore,
                Total = result.Total,
                SubjectName = result.SubjectName,
                Subject = result.Subject,
                MidTermTotal = result.MidTermTotal,
                TermTotal = result.TermTotal,
                FirstTermTotal = result.FirstTermTotal,
                SecondTermTotal = result.SecondTermTotal,
                AverageScore = result.AverageScore
            };
        }
    }
}
