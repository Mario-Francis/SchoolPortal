using Microsoft.AspNetCore.Http;
using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class CourseWorkVM
    {
        public long Id { get; set; }
        [Required]
        public long ClassRoomId { get; set; }
        [Required]
        public int WeekNo { get; set; }
        [Required]
        public DateTimeOffset From { get; set; }
        [Required]
        public DateTimeOffset To { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public IFormFile File { get; set; }
        public string FilePath { get; set; }

        public string UpdatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        public ClassRoomVM ClassRoom { get; set; }
        public string ClassRoomName { get; set; }

        public string FormattedFrom
        {
            get
            {
                return From.ToString("MMM d, yyyy");
            }
        }
        public string FormattedTo
        {
            get
            {
                return To.ToString("MMM d, yyyy");
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

        public CourseWork ToCourseWork()
        {
            return new CourseWork
            {
                Id = Id,
                ClassRoomId=ClassRoomId,
                Title=Title,
                Description=Description,
                WeekNo=WeekNo,
                From=From,
                To=To,
                CreatedDate = DateTimeOffset.Now,
                UpdatedDate = DateTimeOffset.Now
            };
        }

        public static CourseWorkVM FromCourseWork(CourseWork courseWork, int? clientTimeOffset = null)
        {
            var classRoom = ClassRoomVM.FromClassRoom(courseWork.ClassRoom);
            return new CourseWorkVM
            {
                Id =courseWork.Id,
                ClassRoomId=courseWork.ClassRoomId,
                ClassRoom=classRoom,
                ClassRoomName= $"{classRoom.Class} {classRoom.RoomCode}",
                Title=courseWork.Title,
                Description=courseWork.Description,
                WeekNo=courseWork.WeekNo,
                FilePath=courseWork.FilePath,
                UpdatedBy = courseWork.UpdatedBy,
                From = clientTimeOffset == null ? courseWork.From : courseWork.From.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                To = clientTimeOffset == null ? courseWork.To : courseWork.To.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                CreatedDate = clientTimeOffset == null ? courseWork.CreatedDate : courseWork.CreatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value)),
                UpdatedDate = clientTimeOffset == null ? courseWork.UpdatedDate : courseWork.UpdatedDate.ToOffset(TimeSpan.FromMinutes(clientTimeOffset.Value))
            };
        }

    }
}
