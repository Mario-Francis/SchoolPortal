using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class HealthRecordVM
    {
        public long Id { get; set; }
        [Required]
        public decimal StartHeight { get; set; }
        [Required]
        public decimal? EndHeight { get; set; }
        [Required]
        public decimal StartWeight { get; set; }
        [Required]
        public decimal? EndWeight { get; set; }
        [Required]
        public long StudentId { get; set; }
        [Required]
        public long TermId { get; set; }
        [Required]
        public string Session { get; set; }

        public string UpdatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        public ItemVM Term { get; set; }
        public StudentVM Student { get; set; }
        public string AdmissionNo { get; set; }
        public string Class { get; set; }
        public string StudentName { get; set; }
        public string TermName { get; set; }

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

        public HealthRecord ToHealthRecord()
        {
            return new HealthRecord
            {
                Id = Id,
                StartHeight = StartHeight,
                EndHeight = EndHeight,
                StartWeight = StartWeight,
                EndWeight = EndWeight,

                Session = Session,
                StudentId = StudentId,
                TermId = TermId,

                CreatedDate=DateTimeOffset.Now,
                UpdatedDate=DateTimeOffset.Now,
            };
        }

        public static HealthRecordVM FromHealthRecordObject(HealthRecordObject record)
        {
            return record == null ? null : new HealthRecordVM
            {
                Id = record.Id,
                StartHeight = record.StartHeight,
                EndHeight = record.EndHeight,
                StartWeight = record.StartWeight,
                EndWeight = record.EndWeight
            };
        }

        public static HealthRecordVM FromHealthRecord(HealthRecord record, int? clientTimeOffset = null)
        {
            if (record == null)
            {
                return null;
            }
            else
            {
                var student = StudentVM.FromStudent(record.Student);
                return new HealthRecordVM
                {
                    Id = record.Id,
                    StartHeight = record.StartHeight,
                    EndHeight = record.EndHeight,
                    StartWeight = record.StartWeight,
                    EndWeight = record.EndWeight,
                    Session = record.Session,
                    TermId = record.TermId,
                    Term = new ItemVM { Id = record.Term.Id, Name = record.Term.Name },
                    TermName = record.Term.Name,
                    StudentId = record.StudentId,
                    Student = student,
                    StudentName = student.FullName,
                    AdmissionNo = student.AdmissionNo,
                    Class = student.ClassRoom.Class + " " + student.ClassRoom.RoomCode,
                    UpdatedBy = record.UpdatedBy,
                    CreatedDate = clientTimeOffset == null ? record.CreatedDate : record.CreatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                    UpdatedDate = clientTimeOffset == null ? record.UpdatedDate : record.UpdatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value))
                };
            }
        }
    }
}
