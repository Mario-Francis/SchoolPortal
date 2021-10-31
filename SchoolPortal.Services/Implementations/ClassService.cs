using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SchoolPortal.Core;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Core.Models;
using SchoolPortal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services.Implementations
{
    public class ClassService:IClassService
    {
        private readonly IRepository<Class> classRepo;
        private readonly IRepository<ClassRoom> classRoomRepo;
        private readonly ILogger<ClassService> logger;
        private readonly IHttpContextAccessor accessor;

        public ClassService(IRepository<Class> classRepo,
            IRepository<ClassRoom> classRoomRepo,
            ILogger<ClassService> logger,
            IHttpContextAccessor accessor)
        {
            this.classRepo = classRepo;
            this.classRoomRepo = classRoomRepo;
            this.logger = logger;
            this.accessor = accessor;
        }

        // add class
        public async Task CreateClass(Class @class)
        {
            if (@class == null)
            {
                throw new AppException("Class object cannot be null");
            }

            if(await classRepo.Any(c=>c.ClassTypeId == @class.ClassTypeId && c.ClassGrade == @class.ClassGrade))
            {
                throw new AppException($"A class of same type and grade already exist");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
            @class.CreatedBy = currentUser.Username;
            @class.CreatedDate = DateTimeOffset.Now;
            @class.UpdatedBy = currentUser.Username;
            @class.UpdatedDate = DateTimeOffset.Now;

            await classRepo.Insert(@class, true);

            //log action
            //await loggerService.LogActivity(ActivityActionType.CREATED_ASSESSMENT,
            //    currentUser.PersonNumber,
            //    $"Created new assessment of type '{((Core.AssessmentType)((int)assessment.AssessmentTypeId)).ToString()}' for {assessment.FromDate.ToString("dd-MM-yyyy")} to {assessment.ToDate.ToString("dd-MM-yyyy")}");
        }

        // delete class - without class rooms
        public async Task DeleteClass(long classId)
        {
            var @class = await classRepo.GetById(classId);
            if (@class == null)
            {
                throw new AppException($"Invalid class id {classId}");
            }
            else
            {
                if (@class.ClassRooms.Count > 0)
                {
                    throw new AppException("Class cannot be deleted as it has one or more classrooms attached to it");
                }
                else
                {
                    var _class = @class.Clone<Class>();
                    await classRepo.Delete(classId, true);

                    var currentUser = accessor.HttpContext.GetUserSession();
                    // log activity
                    //await loggerService.LogActivity(ActivityActionType.DELETED_ASSESSMENT, currentUser.PersonNumber,
                    //    $"Deleted assessment of type '{((Core.AssessmentType)((int)_assessment.AssessmentTypeId)).ToString()}' for {_assessment.FromDate.ToString("dd-MM-yyyy")} to {_assessment.ToDate.ToString("dd-MM-yyyy")}");
                }
            }
        }

        // update class
        public async Task UpdateClass(Class @class)
        {
            var _class = await classRepo.GetById(@class.Id);
            if (_class == null)
            {
                throw new AppException($"Invalid class id {@class.Id}");
            }
            else if (await classRepo.Any(c => (c.ClassTypeId == @class.ClassTypeId && c.ClassGrade == @class.ClassGrade) &&
            !(_class.ClassTypeId == @class.ClassTypeId && _class.ClassGrade == @class.ClassGrade)))
            {
                throw new AppException($"A class of same type and grade already exist");
            }
            else
            {
                var currentUser = accessor.HttpContext.GetUserSession();
                var _oldclass = _class.Clone<Class>();

                _class.ClassTypeId = @class.ClassTypeId;
                _class.ClassGrade = @class.ClassGrade;
                _class.UpdatedBy = currentUser.Username;
                _class.UpdatedDate = DateTimeOffset.Now;

                await classRepo.Update(_class, true);


                // log activity
                //await loggerService.LogActivity(ActivityActionType.UPDATED_class, currentUser.PersonNumber,
                //    classRepo.TableName, _oldclass, _class,
                //     $"Updated class of type '{((Core.classType)((int)_class.classTypeId)).ToString()}' for {_class.FromDate.ToString("dd-MM-yyyy")} to {_class.ToDate.ToString("dd-MM-yyyy")}");

            }
        }

        public IEnumerable<Class> GetClasses()
        {
            return classRepo.GetAll().OrderBy(c => c.ClassGrade);
        }

        public async Task<Class> GetClass(long id)
        {
            return await classRepo.GetById(id);
        }
    }
}
