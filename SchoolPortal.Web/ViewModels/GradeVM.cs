using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class GradeVM
    {
        public long Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int From { get; set; }
        [Required]
        public int To { get; set; }
        [Required]
        public long TermSectionId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        public ItemVM TermSection { get; set; }

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

        public Grade ToGrade()
        {
            return new Grade
            {
                Id = Id,
                TermSectionId = TermSectionId,
                Code=Code,
                Description=Description,
                From=From,
                To=To,
                CreatedDate = DateTimeOffset.Now,
                UpdatedDate = DateTimeOffset.Now
            };
        }

        public static GradeVM FromGrade(Grade grade, int? clientTimeOffset = null)
        {
            return new GradeVM
            {
                Id =grade.Id,
                TermSectionId = grade.TermSectionId,
                TermSection = new ItemVM { Id=grade.TermSection.Id, Name=grade.TermSection.Name},
                Code=grade.Code,
                Description=grade.Description,
                From=grade.From,
                To=grade.To,
                UpdatedBy = grade.UpdatedBy,
                CreatedDate = clientTimeOffset == null ? grade.CreatedDate : grade.CreatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                UpdatedDate = clientTimeOffset == null ? grade.UpdatedDate : grade.UpdatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value))
            };
        }

    }
}
