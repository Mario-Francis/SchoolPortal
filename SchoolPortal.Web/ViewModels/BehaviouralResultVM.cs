using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class BehaviouralResultVM
    {
        public long Id { get; set; }
        [Required]
        public string Session { get; set; }
        [Required]
        public long TermId { get; set; }
        [Required]
        public long StudentId { get; set; }
        [Required]
        public long BehaviouralRatingId { get; set; }
        [Required]
        public string Score { get; set; }

        public string UpdatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        public ItemVM Term { get; set; }
        public StudentVM Student { get; set; }
        public ItemVM BehaviouralRating { get; set; }

        public string FormattedCreatedDate
        {
            get
            {
                return CreatedDate.ToString("MMM d, yyyy");
            }
        }
        public string FormattedUpdatedDate
        {
            get
            {
                return UpdatedDate.ToString("MMM d, yyyy");
            }
        }

        public BehaviouralResult ToBehaviouralResult()
        {
            return new BehaviouralResult
            {
                Id = Id,
                Session = Session,
                StudentId = StudentId,
                TermId = TermId,
                BehaviouralRatingId=BehaviouralRatingId,
                Score=Score,
                CreatedDate = DateTimeOffset.Now,
                UpdatedDate = DateTimeOffset.Now,
            };
        }

        public static BehaviouralResultVM FromBehaviouralResult(BehaviouralResult result, int? clientTimeOffset = null)
        {
            return result==null?null: new BehaviouralResultVM
            {
                Id = result.Id,
                BehaviouralRatingId = result.BehaviouralRatingId,
                Score = result.Score,
                Session = result.Session,
                TermId = result.TermId,
                StudentId = result.StudentId,
                Student = StudentVM.FromStudent(result.Student),
                Term = new ItemVM { Id = result.Term.Id, Name = result.Term.Name },
                BehaviouralRating = new ItemVM { Id = result.BehaviouralRating.Id, Name = result.BehaviouralRating.Name },
                UpdatedBy = result.UpdatedBy,
                CreatedDate = clientTimeOffset == null ? result.CreatedDate : result.CreatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                UpdatedDate = clientTimeOffset == null ? result.UpdatedDate : result.UpdatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value))
            };
        }
    }
}
