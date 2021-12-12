using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class BatchUploadResultVM
    {
        [Required]
        public long ExamId { get; set; }
        [Required]
        public long ClassId { get; set; }
        [Required]
        public long SubjectId { get; set; }
        [Required]
        public IFormFile File { get; set; }
    }
}
