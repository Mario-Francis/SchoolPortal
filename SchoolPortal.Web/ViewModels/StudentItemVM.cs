using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class StudentItemVM
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string AdmissionNo { get; set; }
        public string Class { get; set; }

        public static StudentItemVM FromStudent(Student student)
        {
            if (student == null)
            {
                return null;
            }
            else
            {
                var classRoom = student.ClassRoomStudents.FirstOrDefault()?.ClassRoom;
                return new StudentItemVM
                {
                    Id = student.Id,
                    Username = student.Username,
                    Email = student.Email,
                    FirstName = student.FirstName,
                    MiddleName = student.MiddleName,
                    Surname = student.Surname,
                    AdmissionNo = student.AdmissionNo,
                    Class = classRoom == null ? "Graduated" : $"{classRoom.Class.ClassType.Name} {classRoom.Class.ClassGrade} {classRoom.RoomCode}"
                };
            }
        }
    }
}
