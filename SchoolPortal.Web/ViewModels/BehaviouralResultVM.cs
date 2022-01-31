using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class BehaviouralResultVM
    {
        public long Id { get; set; }
        public string Session { get; set; }
        public long TermId { get; set; }
        public long StudentId { get; set; }
        public long BehaviouralRatingId { get; set; }
        public string Score { get; set; }

        public StudentVM Student { get; set; }

        public static BehaviouralResultVM FromBehaviouralResult(BehaviouralResult result)
        {
            return new BehaviouralResultVM
            {
                Id = result.Id,
                BehaviouralRatingId = result.BehaviouralRatingId,
                Score = result.Score,
                Session = result.Session,
                TermId = result.TermId,
                StudentId = result.StudentId,
                Student = StudentVM.FromStudent(result.Student)
            };
        }
    }
}
