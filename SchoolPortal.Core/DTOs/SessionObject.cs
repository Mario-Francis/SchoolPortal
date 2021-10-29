using SchoolPortal.Core.Extensions;
using SchoolPortal.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace SchoolPortal.Core.DTOs
{
    public class SessionObject
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhotoPath { get; set; }
        public List<RoleObject> Roles { get; set; }

        public static SessionObject FromUser(User user)
        {
            return new SessionObject
            {
                Email = user.Email,
                FirstName = user.FirstName,
                FullName = $"{user.FirstName} {user.Surname}",
                Surname = user.Surname,
                Username =user.Username,
                Id = user.Id,
                Roles = user.UserRoles.Select(er => RoleObject.FromRole(er.Role)).ToList(),
                PhotoPath=user.PhotoPath
            };
        }

        public static SessionObject FromStudent(Student student)
        {
            return new SessionObject
            {
                Email = student.Email,
                FirstName = student.FirstName,
                FullName = $"{student.FirstName} {student.Surname}",
                Surname = student.Surname,
                Username = student.Username,
                Id = student.Id,
                Roles = new List<RoleObject> { 
                    new RoleObject { 
                        Id=(int)AppRoles.STUDENT, 
                        Name = AppRoles.STUDENT.ToString().Capitalize()
                    } 
                },
                PhotoPath = student.PhotoPath
            };
        }
    }
}
