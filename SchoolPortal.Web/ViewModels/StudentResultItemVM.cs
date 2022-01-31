using SchoolPortal.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class StudentResultItemVM
    {
        public long Id { get; set; }
        public long SubjectId { get; set; }
        public decimal MidTermTotal { get; set; }
        public decimal ClassWorkScore { get; set; }
        public decimal TestScore { get; set; }
        public decimal ExamScore { get; set; }
        public decimal Total { get; set; }

        public string SubjectName { get; set; }
        public SubjectVM Subject { get; set; }

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
        public static StudentResultItemVM FromStudentResultItem(StudentResultItem item)
        {
            return new StudentResultItemVM
            {
                Id = item.Id,
                SubjectId = item.SubjectId,
                ClassWorkScore = item.ClassWorkScore,
                TestScore = item.TestScore,
                ExamScore = item.ExamScore,
                Total = item.Total,
                Subject = SubjectVM.FromSubject(item.Subject),
                SubjectName = item.SubjectName,
                AverageScore = item.AverageScore,
                FirstTermTotal = item.FirstTermTotal,
                MidTermTotal = item.MidTermTotal,
                SecondTermTotal = item.SecondTermTotal,
                TermTotal = item.TermTotal
            };
        }
       
    }
}
