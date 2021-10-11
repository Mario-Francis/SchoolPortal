using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace SchoolPortal.Core.Models
{
    public class User:BaseEntity, IUpdatable
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public string EmailVerificationToken { get; set; }
        public string PasswordRecoveryToken { get; set; }
       
        public bool IsActive { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

        //Navigation Properties
        [JsonIgnore]
        [NotMapped]
        public virtual User UpdatedByUser { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<StudentGuardian> StudentGuardians{ get; set; }
        public virtual ICollection<UserLoginHistory> UserLoginHistories { get; set; }
        public virtual ICollection<ClassRoom> ClassRooms { get; set; }
    }
}
