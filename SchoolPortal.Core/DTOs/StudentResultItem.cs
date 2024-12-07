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
        public decimal? MidTermTotal { get; set; }
        public decimal ClassWorkScore { get; set; }
        public decimal TestScore { get; set; }
        public decimal ExamScore { get; set; }
        public decimal Total { get; set; }

        public string SubjectName { get; set; }
        public virtual Subject Subject { get; set; }

        #region End of session fields
        //public long? FirstMidTermResultId { get; set; }
        //public long? FirstEndTermResultId { get; set; }
        //public decimal? FirstMidTermTotal { get; set; }
        //public decimal? FirstEndTermTotal { get; set; }
        public decimal? FirstTermTotal { get; set; }

        //public long? SecondMidTermResultId { get; set; }
        //public long? SecondEndTermResultId { get; set; }
        //public decimal? SecondMidTermTotal { get; set; }
        //public decimal? SecondEndTermTotal { get; set; }
        public decimal? SecondTermTotal { get; set; }

        public decimal? TermTotal { get; set; }
        public decimal? AverageScore { get; set; }

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
                TermTotal = result.MidTermTotal != null ? (result.MidTermTotal + result.Total) : Math.Round((result.Total / 60) * 100, MidpointRounding.AwayFromZero)
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
                TermTotal = result.MidTermTotal != null ? (result.MidTermTotal + result.Total) : Math.Round((result.Total / 60) * 100, MidpointRounding.AwayFromZero),
                FirstTermTotal = GetFirstTermTotal(result),
                SecondTermTotal = GetSecondTermTotal(result),
                AverageScore = result.AverageScore != null ? Math.Round(result.AverageScore.Value, 0, MidpointRounding.AwayFromZero) : GetAverageScore(result)
            };
        }

        public static StudentResultItem FromEndOfSecondTermResultViewObject(EndOfSecondTermResultViewObject result)
        {
            var termTotal = result.MidTermTotal != null ? (result.MidTermTotal + result.Total) : Math.Round((result.Total / 60) * 100, MidpointRounding.AwayFromZero);
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
                TermTotal = termTotal,
                FirstTermTotal = GetFirstTermTotal(result),
                SecondTermTotal = termTotal,
                AverageScore = result.AverageScore!=null? Math.Round(result.AverageScore.Value, 0, MidpointRounding.AwayFromZero) :GetAverageScore(result)
            };
        }

        private static decimal? GetFirstTermTotal(EndOfSessionResultViewObject result)
        {
            if (result.FirstEndTermTotal == null)
            {
                return null;
            }
            else
            {
                return result.FirstMidTermTotal != null ?
                    (result.FirstMidTermTotal + result.FirstEndTermTotal) :
                    Math.Round((result.FirstEndTermTotal.Value / 60) * 100, 0, MidpointRounding.AwayFromZero);
            }
        }

        private static decimal? GetFirstTermTotal(EndOfSecondTermResultViewObject result)
        {
            if (result.FirstEndTermTotal == null)
            {
                return null;
            }
            else
            {
                return result.FirstMidTermTotal != null ?
                    (result.FirstMidTermTotal + result.FirstEndTermTotal) :
                    Math.Round((result.FirstEndTermTotal.Value / 60) * 100, 0, MidpointRounding.AwayFromZero);
            }
        }

        private static decimal? GetSecondTermTotal(EndOfSessionResultViewObject result)
        {
            if (result.SecondEndTermTotal == null)
            {
                return null;
            }
            else
            {
                return result.SecondMidTermTotal != null ?
                    (result.SecondMidTermTotal + result.SecondEndTermTotal) :
                    Math.Round((result.SecondEndTermTotal.Value / 60) * 100, 0,MidpointRounding.AwayFromZero);
            }
        }

        private static decimal? GetAverageScore(EndOfSessionResultViewObject result)
        {
            var termTotal = result.MidTermTotal != null ? (result.MidTermTotal + result.Total) : Math.Round((result.Total / 60) * 100, 0, MidpointRounding.AwayFromZero);
            var first = GetFirstTermTotal(result);
            var second = GetSecondTermTotal(result);
            if (first==null && second==null)
            {
                return termTotal;
            }
            else if(first == null)
            {
                return Math.Round(((second + termTotal) / 2).Value, 0, MidpointRounding.AwayFromZero);
            }
            else if(second==null)
            {
                return Math.Round(((first + termTotal) / 2).Value, 0, MidpointRounding.AwayFromZero);
            }
            else
            {
                return Math.Round(((first + second + termTotal) / 3).Value, 0, MidpointRounding.AwayFromZero);
            }
        }

        private static decimal? GetAverageScore(EndOfSecondTermResultViewObject result)
        {
            var termTotal = result.MidTermTotal != null ? (result.MidTermTotal + result.Total) : Math.Round((result.Total / 60) * 100, 0, MidpointRounding.AwayFromZero);
            var first = GetFirstTermTotal(result);
            if (first == null)
            {
                return termTotal;
            }
            else
            {
                return Math.Round(((first + termTotal) / 2).Value, 0, MidpointRounding.AwayFromZero);
            }
        }

    }
}
