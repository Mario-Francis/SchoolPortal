using Microsoft.AspNetCore.Http;
using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
    public interface ICourseWorkService
    {
        Task AddCourseWork(CourseWork courseWork, IFormFile file);
        Task UpdateCourseWork(CourseWork courseWork);
        Task DeleteCourseWork(long courseWorkId);
        Task<CourseWork> GetCourseWork(long courseWorkId);
        IEnumerable<CourseWork> GetCourseWorks();
        IEnumerable<CourseWork> GetCourseWorks(long classRoomId);
    }
}
