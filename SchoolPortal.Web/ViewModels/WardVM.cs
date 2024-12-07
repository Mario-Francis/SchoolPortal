using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class WardVM
    {
        public long Id { get; set; }
        [Required]
        public long StudentId { get; set; }
        [Required]
        public long RelationshipId { get; set; }
        [Required]
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string AdmissionNo { get; set; }
        public DateTimeOffset EnrollmentDate { get; set; }
        public string EntrySession { get; set; }
        public long EntryTermId { get; set; }
        public long EntryClassId { get; set; }
        public long? ClassId { get; set; }
        public long? ClassRoomId { get; set; }

        public string FullName { get; set; }

        public string PhotoPath { get; set; }
        public bool IsGraduated { get; set; }
        public bool IsActive { get; set; }
        public bool IsPasswordChanged { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public ClassRoomVM ClassRoom { get; set; }
        public string FormattedClassRoom { get; set; }
        public string EntryClass { get; set; }
        public string EntryTerm { get; set; }
        public string Relationship { get; set; }
        public string FormattedEnrollmentDate
        {
            get
            {
                return EnrollmentDate.ToString("MMM d, yyyy");
            }
        }
        public string FormattedDateOfBirth
        {
            get
            {
                return DateOfBirth.ToString("MMM d, yyyy");
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

        public StudentGuardian ToStudentGuardian()
        {
            return new StudentGuardian
            {
                StudentId = StudentId,
                GuardianId = UserId,
                RelationshipId = RelationshipId,
                CreatedDate = DateTimeOffset.Now,
                UpdatedDate = DateTimeOffset.Now
            };
        }

        public static WardVM FromStudentGuardian(StudentGuardian studentGuardian, int? clientTimeOffset = null)
        {
            var student = studentGuardian.Student;
            var classRoom = student.ClassRoomStudents.FirstOrDefault().ClassRoom;
            return new WardVM
            {
                Id=studentGuardian.Id,
                StudentId = student.Id,
                UserId=studentGuardian.GuardianId,
                RelationshipId=studentGuardian.RelationshipId,
                Relationship = studentGuardian.Relationship.Name,
                FirstName = student.FirstName,
                MiddleName = student.MiddleName,
                Surname = student.Surname,
                FullName= $"{student.FirstName} {student.MiddleName} {student.Surname}".Replace("  ", " "),
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                Gender = student.Gender,
                PhotoPath = student.PhotoPath,
                EnrollmentDate = clientTimeOffset == null ? student.EnrollmentDate : student.EnrollmentDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                AdmissionNo = student.AdmissionNo,
                EntryTermId = student.EntryTermId,
                EntryTerm = student.EntryTerm.Name,
                EntryClassId = student.EntryClassId,
                EntryClass = $"{student.EntryClass.ClassType.Name} {student.EntryClass.ClassGrade}",
                EntrySession = student.EntrySession,
                IsGraduated = student.IsGraduated,
                ClassId = classRoom.ClassId,
                ClassRoomId = classRoom.Id,
                ClassRoom = ClassRoomVM.FromClassRoom(classRoom),
                IsActive = student.IsActive,
                IsPasswordChanged = student.IsPasswordChanged,
                Username = student.Username,
                FormattedClassRoom = $"{classRoom?.Class.ClassType.Name} {classRoom?.Class.ClassGrade} {classRoom?.RoomCode}".Trim(),
                DateOfBirth = clientTimeOffset == null ? student.DateOfBirth : student.DateOfBirth.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                UpdatedBy = student.UpdatedBy,
                CreatedDate = clientTimeOffset == null ? student.CreatedDate : student.CreatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                UpdatedDate = clientTimeOffset == null ? student.UpdatedDate : student.UpdatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
            };
        }
    }
}
