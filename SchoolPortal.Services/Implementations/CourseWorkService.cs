using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SchoolPortal.Core;
using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Core.Models;
using SchoolPortal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services.Implementations
{
    public class CourseWorkService: ICourseWorkService
    {
        private readonly IRepository<CourseWork> courseWorkRepo;
        private readonly IRepository<ClassRoom> classRoomRepo;
        private readonly ILoggerService<CourseWorkService> logger;
        private readonly IHttpContextAccessor accessor;
        private readonly IFileService fileService;
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;

        public CourseWorkService(
            IRepository<CourseWork> courseWorkRepo,
            IRepository<ClassRoom> classRoomRepo,
            ILoggerService<CourseWorkService> logger,
            IHttpContextAccessor accessor,
            IFileService fileService,
            IOptionsSnapshot<AppSettings> appSettingsDelegate)
        {
            this.courseWorkRepo = courseWorkRepo;
            this.classRoomRepo = classRoomRepo;
            this.logger = logger;
            this.accessor = accessor;
            this.fileService = fileService;
            this.appSettingsDelegate = appSettingsDelegate;
        }

        public async Task AddCourseWork(CourseWork courseWork, IFormFile file)
        {
            if (courseWork == null)
            {
                throw new AppException("Course work object cannot be null");
            }

            if(courseWork.From >= courseWork.To)
            {
                throw new AppException("Starting week date canot be geater than or equal to ending week date");
            }

            if(courseWork.WeekNo == 0)
            {
                throw new AppException("Invalid week number");
            }

            var classRoom = await classRoomRepo.GetById(courseWork.ClassRoomId);
            if (classRoom == null)
            {
                throw new AppException("Invalid classroom id");
            }

            var allowedExtensions = new string[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".ppt", ".pptx" };
            if(!fileService.ValidateFile(file, allowedExtensions, out List<string> errorItems, appSettingsDelegate.Value.MaxUploadSize))
            {
                throw new AppException($"File validation failed", errorItems: errorItems);
            }

            if (await courseWorkRepo.AnyAsync(c => c.ClassRoomId == courseWork.ClassRoomId && c.Title == courseWork.Title))
            {
                throw new AppException($"A course work with title '{courseWork.Title}' already exist for this specified classroom");
            }

            var path = fileService.SaveFile(file, "uploads/course_works");

            var currentUser = accessor.HttpContext.GetUserSession();

            courseWork.FilePath = path;
            courseWork.CreatedBy = currentUser.Username;
            courseWork.CreatedDate = DateTimeOffset.Now;
            courseWork.UpdatedBy = currentUser.Username;
            courseWork.UpdatedDate = DateTimeOffset.Now;

            await courseWorkRepo.Insert(courseWork, false);

            //log action
            await logger.LogActivity(ActivityActionType.ADDED_COURSE_WORK, currentUser.Username, 
                courseWorkRepo.TableName, new CourseWork(), courseWork,
                $"Added new coursework");
        }

        public async Task UpdateCourseWork(CourseWork courseWork)
        {
            var _courseWork = await courseWorkRepo.GetById(courseWork.Id);
            if (_courseWork == null)
            {
                throw new AppException("Invalid course work id");
            }

            if (courseWork.From >= courseWork.To)
            {
                throw new AppException("Starting week date canot be geater than or equal to ending week date");
            }

            if (courseWork.WeekNo == 0)
            {
                throw new AppException("Invalid week number");
            }

            var classRoom = await classRoomRepo.GetById(courseWork.ClassRoomId);
            if (classRoom == null)
            {
                throw new AppException("Invalid classroom id");
            }

            if (await courseWorkRepo.AnyAsync(c => c.ClassRoomId == courseWork.ClassRoomId && c.Title == courseWork.Title && c.Id!=courseWork.Id))
            {
                throw new AppException($"A course work with title '{courseWork.Title}' already exist for this specified classroom");
            }

            var _oldCourseWork = _courseWork.Clone<CourseWork>();
            var currentUser = accessor.HttpContext.GetUserSession();

            _courseWork.Title = courseWork.Title;
            _courseWork.Description = courseWork.Description;
            _courseWork.WeekNo = courseWork.WeekNo;
            _courseWork.From = courseWork.From;
            _courseWork.To = courseWork.To;
            _courseWork.ClassRoomId = courseWork.ClassRoomId;
            _courseWork.UpdatedBy = currentUser.Username;
            _courseWork.UpdatedDate = DateTimeOffset.Now;

            await courseWorkRepo.Update(_courseWork, false);

            //log action
            await logger.LogActivity(ActivityActionType.UPDATED_COURSE_WORK, currentUser.Username,
                courseWorkRepo.TableName, _oldCourseWork, _courseWork,
                $"Updated coursework");
        }

        public async Task DeleteCourseWork(long courseWorkId)
        {
            var courseWork = await courseWorkRepo.GetById(courseWorkId);
            if (courseWork == null)
            {
                throw new AppException("Invalid course work id");
            }

            var oldCourseWork = courseWork.Clone<CourseWork>();
            var currentUser = accessor.HttpContext.GetUserSession();

            await courseWorkRepo.Delete(courseWorkId, false);

            //log action
            await logger.LogActivity(ActivityActionType.DELETED_COURSE_WORK, currentUser.Username,
                courseWorkRepo.TableName, courseWork, new CourseWork(),
                $"Deleted coursework");

            // delete file
            fileService.DeleteFile(oldCourseWork.FilePath);
        }

        public async Task<CourseWork> GetCourseWork(long courseWorkId)
        {
            return await courseWorkRepo.GetById(courseWorkId);
        }

        public IEnumerable<CourseWork> GetCourseWorks()
        {
            return courseWorkRepo.GetAll();
        }

        public IEnumerable<CourseWork> GetCourseWorks(long classRoomId)
        {
            return courseWorkRepo.GetWhere(c => c.ClassRoomId == classRoomId);
        }
    }
}
