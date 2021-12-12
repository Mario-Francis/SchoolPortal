using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class SubjectVM
    {
        public long Id { get; set; }
        [Required]
        public long ClassId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Class { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }


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

        public Subject ToSubject()
        {
            return new Subject
            {
                Id = Id,
                ClassId = ClassId,
                Name=Name,
                Code = Code?.ToUpper(),
                Description=Description,
                CreatedDate = DateTimeOffset.Now,
                UpdatedDate = DateTimeOffset.Now
            };
        }

        public static SubjectVM FromSubject(Subject subject, int? clientTimeOffset = null)
        {
            return new SubjectVM
            {
                Id = subject.Id,
                ClassId = subject.ClassId,
                Name = subject.Name,
                Code=subject.Code,
                IsActive=subject.IsActive,
                Description=subject.Description,
                Class = $"{subject.Class.ClassType.Name} {subject.Class.ClassGrade}",
                UpdatedBy = subject.UpdatedBy,
                CreatedDate = clientTimeOffset == null ? subject.CreatedDate : subject.CreatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                UpdatedDate = clientTimeOffset == null ? subject.UpdatedDate : subject.UpdatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
            };
        }
    }
}
