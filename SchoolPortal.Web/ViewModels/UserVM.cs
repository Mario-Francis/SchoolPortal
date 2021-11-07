using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class UserVM
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
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Gender { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }

        public string FullName { get {
                return $"{FirstName} {MiddleName} {Surname}".Replace("  ", " ");
            } }

        public string PhotoPath { get; set; }
        public bool IsActive { get; set; }
        public bool IsPasswordChanged { get; set; }
        [Required]
        public IEnumerable<RoleObject> Roles { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public ClassRoomVM ClassRoom { get; set; }
        public int WardCount { get; set; }
        public string FormattedDateOfBirth
        {
            get
            {
                return DateOfBirth?.ToString("MMM d, yyyy");
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

        public User ToUser()
        {
            return new User
            {
                Id = Id,
                FirstName=FirstName,
                MiddleName=MiddleName,
                Surname=Surname,
                Email=Email,
                PhoneNumber=PhoneNumber,
                Gender=Gender,
                DateOfBirth=DateOfBirth,
                IsActive=IsActive,
                IsPasswordChanged=IsPasswordChanged,
                CreatedDate = DateTimeOffset.Now,
                UpdatedDate = DateTimeOffset.Now,
                UserRoles=Roles.Select(r=> new UserRole { RoleId = r.Id}).ToList()
            };
        }

        public static UserVM FromUser(User user, int? clientTimeOffset = null)
        {
            return new UserVM
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                Surname = user.Surname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                PhotoPath = user.PhotoPath,
                IsActive = user.IsActive,
                IsPasswordChanged = user.IsPasswordChanged,
                Username = user.Username,
                WardCount = user.StudentGuardians.Count,
                Roles=user.UserRoles.Select(ur=> RoleObject.FromRole(ur.Role)),
                ClassRoom = ClassRoomVM.FromClassRoom(user.ClassRoomTeachers.FirstOrDefault()?.ClassRoom),
                DateOfBirth = clientTimeOffset == null ? user.DateOfBirth : user.DateOfBirth?.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                UpdatedBy = user.UpdatedBy,
                CreatedDate = clientTimeOffset == null ? user.CreatedDate : user.CreatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                UpdatedDate = clientTimeOffset == null ? user.UpdatedDate : user.UpdatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
            };
        }
    }
}
