using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class AttendanceRecordVM
    {
        public long Id { get; set; }
        [Required]
        public int SchoolOpenCount { get; set; }
        [Required]
        public int PresentCount { get; set; }
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

        public AttendanceRecord ToAttendanceRecord()
        {
            return new AttendanceRecord
            {
                Id = Id,
                SchoolOpenCount = SchoolOpenCount,
                PresentCount = PresentCount,

                Session = Session,
                StudentId = StudentId,
                TermId = TermId,

                CreatedDate = DateTimeOffset.Now,
                UpdatedDate = DateTimeOffset.Now,
            };
        }

        public static AttendanceRecordVM FromAttendanceRecordObject(AttendanceRecordObject record)
        {
            return record == null ? null : new AttendanceRecordVM
            {
                Id = record.Id,
                SchoolOpenCount = record.SchoolOpenCount,
                PresentCount = record.PresentCount
            };
        }

        public static AttendanceRecordVM FromAttendanceRecord(AttendanceRecord record, int? clientTimeOffset = null)
        {
            if (record == null)
            {
                return null;
            }
            else
            {
                var student = StudentVM.FromStudent(record.Student);
                return new AttendanceRecordVM
                {
                    Id = record.Id,
                    SchoolOpenCount = record.SchoolOpenCount,
                    PresentCount = record.PresentCount,
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
