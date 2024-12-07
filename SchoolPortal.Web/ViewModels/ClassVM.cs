using SchoolPortal.Core.Extensions;
using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class ClassVM
    {
        public long Id { get; set; }
        [Required]
        public long ClassTypeId { get; set; }
        [Required]
        [Range(1, 6)]
        public int ClassGrade { get; set; }

        public string ClassType { get; set; }
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

        public Class ToClass()
        {
            return new Class
            {
                Id = Id,
                ClassTypeId = ClassTypeId,
                ClassGrade = ClassGrade,
                CreatedDate = DateTimeOffset.Now,
                UpdatedDate = DateTimeOffset.Now
            };
        }

        public static ClassVM FromClass(Class @class, int? clientTimeOffset = null)
        {
            return new ClassVM
            {
                Id = @class.Id,
                ClassTypeId = @class.ClassTypeId,
                ClassGrade=@class.ClassGrade,
                ClassType=@class.ClassType.Name.Capitalize(),
                UpdatedBy = @class.UpdatedBy,
                CreatedDate = clientTimeOffset == null ? @class.CreatedDate : @class.CreatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                UpdatedDate = clientTimeOffset == null ? @class.UpdatedDate : @class.UpdatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value))
            };
        }
    }
}
