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
        public string UserType { get; set; } // user or student
        public long ClassRoomId { get; set; }
        public string ClassRoomName { get; set; }
        public bool HasClassRoom { get; set; }
        public bool IsInitialPasswordChanged { get; set; }


        public static SessionObject FromUser(User user)
        {
            var sessionObject =  new SessionObject
            {
                Email = user.Email,
                FirstName = user.FirstName,
                FullName = $"{user.FirstName} {user.Surname}",
                Surname = user.Surname,
                Username =user.Username,
                Id = user.Id,
                Roles = user.UserRoles.Select(er => RoleObject.FromRole(er.Role)).ToList(),
                PhotoPath=user.PhotoPath,
                UserType = Constants.USER_TYPE_USER,
                IsInitialPasswordChanged=user.IsPasswordChanged
            };

            if(user.UserRoles.Any(ur=> ur.RoleId == (long)AppRoles.TEACHER))
            {
                var classroom = user.ClassRoomTeachers?.FirstOrDefault().ClassRoom;
                if (classroom != null)
                {
                    sessionObject.ClassRoomName = $"{classroom.Class.ClassType.Name} {classroom.Class.ClassGrade} {classroom.RoomCode}";
                    sessionObject.ClassRoomId = classroom.Id;
                    sessionObject.HasClassRoom = true;
                }
                else
                {
                    sessionObject.HasClassRoom = false;
                }
            }
            return sessionObject;
        }

        public static SessionObject FromStudent(Student student)
        {
            var classroom = student.ClassRoomStudents.First().ClassRoom;
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
                PhotoPath = student.PhotoPath,
                UserType = Constants.USER_TYPE_STUDENT,
                HasClassRoom=true,
                ClassRoomId = classroom.Id,
                ClassRoomName = $"{classroom.Class.ClassType.Name} {classroom.Class.ClassGrade} {classroom.RoomCode}",
                IsInitialPasswordChanged=student.IsPasswordChanged
            };
        }
    }
}
