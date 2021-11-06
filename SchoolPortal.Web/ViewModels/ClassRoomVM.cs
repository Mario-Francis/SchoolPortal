using SchoolPortal.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Web.ViewModels
{
    public class ClassRoomVM
    {
        public long Id { get; set; }
        [Required]
        public long ClassId { get; set; }
        [Required]
        //[MaxLength(1)]
        //[RegularExpression("[A-Za-z]", ErrorMessage ="Only alphabets are allowed for Room Code field")]
        public string RoomCode { get; set; }
        public bool IsActive { get; set; }
        public string Class { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }


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

        public ClassRoom ToClassRoom()
        {
            return new ClassRoom
            {
                Id = Id,
                ClassId = ClassId,
                RoomCode = RoomCode.ToUpper(),
                CreatedDate = DateTimeOffset.Now,
                UpdatedDate = DateTimeOffset.Now,
                IsActive = IsActive
            };
        }

        public static ClassRoomVM FromClassRoom(ClassRoom classRoom, int? clientTimeOffset = null)
        {
            return new ClassRoomVM
            {
                Id = classRoom.Id,
                ClassId = classRoom.ClassId,
                RoomCode=classRoom.RoomCode,
                Class = $"{classRoom.Class.ClassType.Name} {classRoom.Class.ClassGrade}",
                UpdatedBy = classRoom.UpdatedBy,
                IsActive = classRoom.IsActive,
                CreatedDate = clientTimeOffset == null ? classRoom.CreatedDate : classRoom.CreatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                UpdatedDate = clientTimeOffset == null ? classRoom.UpdatedDate : classRoom.UpdatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
            };
        }
    }
}
