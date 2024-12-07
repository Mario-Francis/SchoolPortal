using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class ExamVM
    {
        public long Id { get; set; }
        [Required]
        public long ExamTypeId { get; set; }
        [Required]
        public string Session { get; set; }
        [Required]
        public long TermId { get; set; }
        [Required]
        public DateTimeOffset StartDate { get; set; }
        [Required]
        public DateTimeOffset EndDate { get; set; }
        

        public string ExamType { get; set; }
        public string Term { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }


        public string FormattedStartDate
        {
            get
            {
                return StartDate.ToString("MMM d, yyyy");
            }
        }
        public string FormattedEndDate
        {
            get
            {
                return EndDate.ToString("MMM d, yyyy");
            }
        }
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

        public Exam ToExam()
        {
            return new Exam
            {
                Id = Id,
                ExamTypeId = ExamTypeId,
                Session = Session,
                TermId=TermId,
                StartDate=StartDate,
                EndDate=EndDate,
                CreatedDate = DateTimeOffset.Now,
                UpdatedDate = DateTimeOffset.Now
            };
        }

        public static ExamVM FromExam(Exam exam, int? clientTimeOffset = null)
        {
            return new ExamVM
            {
                Id =exam.Id,
                ExamTypeId = exam.ExamTypeId,
                ExamType=exam.ExamType.Name,
                Session = exam.Session,
                TermId=exam.TermId,
                Term = exam.Term.Name,
                UpdatedBy = exam.UpdatedBy,
                StartDate = clientTimeOffset == null ? exam.StartDate : exam.StartDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                EndDate = clientTimeOffset == null ? exam.EndDate : exam.EndDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                CreatedDate = clientTimeOffset == null ? exam.CreatedDate : exam.CreatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                UpdatedDate = clientTimeOffset == null ? exam.UpdatedDate : exam.UpdatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value))
            };
        }
    }
}
