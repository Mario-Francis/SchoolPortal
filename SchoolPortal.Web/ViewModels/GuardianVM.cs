using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class GuardianVM
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
        public string Relationship { get; set; }
        public string FullName{ get;set; }

        public string PhotoPath { get; set; }
        public bool IsActive { get; set; }

        public StudentGuardian ToStudentGuardian()
        {
            return new StudentGuardian
            {
                GuardianId = UserId,
                StudentId = StudentId,
                RelationshipId = RelationshipId
            };
        }

        public static GuardianVM FromStudentGuardian(StudentGuardian studentGuardian, int? clientTimeOffset = null)
        {
            var user = studentGuardian.Guardian;
            return new GuardianVM
            {
                Id = studentGuardian.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                Surname = user.Surname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                PhotoPath = user.PhotoPath,
                IsActive = user.IsActive,
                Username = user.Username,
                UserId = user.Id,
                Relationship = studentGuardian.Relationship.Name,
                RelationshipId = studentGuardian.RelationshipId,
                StudentId = studentGuardian.StudentId,
                FullName = $"{studentGuardian.Guardian.FirstName} {studentGuardian.Guardian.MiddleName} {studentGuardian.Guardian.Surname}".Replace("  ", " ")
            };
        }
    }
}
