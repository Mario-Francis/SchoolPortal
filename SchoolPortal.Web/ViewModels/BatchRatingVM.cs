using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class BatchRatingVM
    {
        [Required]
        public string Session { get; set; }
        [Required]
        public long? TermId { get; set; }
        [Required]
        public long? StudentId { get; set; }
        public IEnumerable<RatingVM> Ratings { get; set; }
    }
}
