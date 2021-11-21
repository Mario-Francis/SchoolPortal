using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class StudentVM
    {
        public long Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string Surname { get; set; }
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTimeOffset DateOfBirth { get; set; }
        [Required]
        public string AdmissionNo { get; set; }
        [Required]
        public DateTimeOffset EnrollmentDate { get; set; }
        [Required]
        public string EntrySession { get; set; }
        [Required]
        public long EntryTermId { get; set; }
        [Required]
        public long EntryClassId { get; set; }
        public long? ClassId { get; set; }
        public long? ClassRoomId { get; set; }
        


        public string FullName
        {
            get
            {
                return $"{FirstName} {MiddleName} {Surname}".Replace("  ", " ");
            }
        }

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

        public Student ToStudent()
        {
            return new Student
            {
                Id = Id,
                FirstName = FirstName,
                MiddleName = MiddleName,
                Surname = Surname,
                Email = Email,
                PhoneNumber = PhoneNumber,
                Gender = Gender,
                DateOfBirth = DateOfBirth,
                EnrollmentDate=EnrollmentDate,
                AdmissionNo=AdmissionNo,
                EntryClassId=EntryClassId,
                EntryTermId=EntryTermId,
                EntrySession=EntrySession,
                IsActive = IsActive,
                IsPasswordChanged = IsPasswordChanged,
                CreatedDate = DateTimeOffset.Now,
                UpdatedDate = DateTimeOffset.Now
            };
        }

        public static StudentVM FromStudent(Student student, int? clientTimeOffset = null)
        {
            var classRoom = student.ClassRoomStudents.FirstOrDefault().ClassRoom;
            return new StudentVM
            {
                Id = student.Id,
                FirstName = student.FirstName,
                MiddleName = student.MiddleName,
                Surname = student.Surname,
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
