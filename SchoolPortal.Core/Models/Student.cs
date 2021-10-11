using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace SchoolPortal.Core.Models
{
    public class Student:BaseEntity,IUpdatable
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string EmailVerificationToken { get; set; }
        public string PasswordRecoveryToken { get; set; }
        public string AdmissionNo { get; set; }
        public DateTimeOffset EnrollmentDate { get; set; }
        public int Status { get; set; }
        public string EntrySession { get; set; }
        public long EntryTermId { get; set; }
        public long EntryClassId { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedByType { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

        //Navigation Properties
        [JsonIgnore]
        [NotMapped]
        public virtual User UpdatedByUser { get; set; }

        [ForeignKey("EntryTermId")]
        public virtual Term EntryTerm { get; set; }
        [ForeignKey("EntryClassId")]
        public virtual Class EntryClass { get; set; }
        public virtual ICollection<MidTermResult> MidTermResults { get; set; }
        public virtual ICollection<EndTermResult> EndTermResults { get; set; }
        public virtual ICollection<StudentAttendanceRecord> StudentAttendanceRecords { get; set; }
        public virtual ICollection<StudentGuardian> StudentGuardians { get; set; }
        public virtual ICollection<StudentLoginHistory> StudentLoginHistories { get; set; }
        public virtual ICollection<ClassRoomStudent> ClassRoomStudents { get; set; }
    }
}
