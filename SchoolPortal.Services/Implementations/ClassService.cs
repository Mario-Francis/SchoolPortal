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

            if(await classRepo.AnyAsync(c=>c.ClassTypeId == @class.ClassTypeId && c.ClassGrade == @class.ClassGrade))
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
            else if (await classRepo.AnyAsync(c => (c.ClassTypeId == @class.ClassTypeId && c.ClassGrade == @class.ClassGrade) &&
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
            return classRepo.GetAll().OrderBy(c => c.ClassTypeId).ThenBy(c=>c.ClassGrade);
        }

        public async Task<Class> GetClass(long id)
        {
            return await classRepo.GetById(id);
        }

        //========= Class room methods ===============
        // add class
        public async Task CreateClassRoom(ClassRoom classRoom)
        {
            if (classRoom == null)
            {
                throw new AppException("Classroom object cannot be null");
            }

            if (await classRoomRepo.AnyAsync(c => c.ClassId == classRoom.ClassId && c.RoomCode == classRoom.RoomCode))
            {
                throw new AppException($"A classroom of same class already exist with same room code");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
            classRoom.IsActive = true;
            classRoom.CreatedBy = currentUser.Username;
            classRoom.CreatedDate = DateTimeOffset.Now;
            classRoom.UpdatedBy = currentUser.Username;
            classRoom.UpdatedDate = DateTimeOffset.Now;

            await classRoomRepo.Insert(classRoom, true);

            //log action
            //await loggerService.LogActivity(ActivityActionType.CREATED_ASSESSMENT,
            //    currentUser.PersonNumber,
            //    $"Created new assessment of type '{((Core.AssessmentType)((int)assessment.AssessmentTypeId)).ToString()}' for {assessment.FromDate.ToString("dd-MM-yyyy")} to {assessment.ToDate.ToString("dd-MM-yyyy")}");
        }

        // delete classroom - without students
        public async Task DeleteClassRoom(long classRoomId)
        {
            var classRoom = await classRoomRepo.GetById(classRoomId);
            if (classRoom == null)
            {
                throw new AppException($"Invalid classroom id {classRoomId}");
            }
            else
            {
                if (classRoom.ClassRoomStudents.Count > 0)
                {
                    throw new AppException("Classroom cannot be deleted as it still has one or more students in it");
                }
                else
                {
                    var _classRoom = classRoom.Clone<ClassRoom>();
                    await classRoomRepo.Delete(classRoomId, true);

                    var currentUser = accessor.HttpContext.GetUserSession();
                    // log activity
                    //await loggerService.LogActivity(ActivityActionType.DELETED_ASSESSMENT, currentUser.PersonNumber,
                    //    $"Deleted assessment of type '{((Core.AssessmentType)((int)_assessment.AssessmentTypeId)).ToString()}' for {_assessment.FromDate.ToString("dd-MM-yyyy")} to {_assessment.ToDate.ToString("dd-MM-yyyy")}");
                }
            }
        }

        // update classroom
        public async Task UpdateClassRoom(ClassRoom classRoom)
        {
            var _classRoom = await classRoomRepo.GetById(classRoom.Id);
            if (_classRoom == null)
            {
                throw new AppException($"Invalid classroom id {classRoom.Id}");
            }
            else if (await classRoomRepo.AnyAsync(c => (c.ClassId == classRoom.ClassId && c.RoomCode == classRoom.RoomCode) &&
            !(_classRoom.ClassId == classRoom.ClassId && _classRoom.RoomCode == classRoom.RoomCode)))
            {
                throw new AppException($"A classroom of same class already exist with same room code");
            }
            else
            {

                var currentUser = accessor.HttpContext.GetUserSession();
                var _oldclassRoom = _classRoom.Clone<ClassRoom>();

                _classRoom.ClassId = classRoom.ClassId;
                _classRoom.RoomCode = classRoom.RoomCode;
               // _classRoom.IsActive = classRoom.IsActive;
                _classRoom.UpdatedBy = currentUser.Username;
                _classRoom.UpdatedDate = DateTimeOffset.Now;

                await classRoomRepo.Update(_classRoom, true);


                // log activity
                //await loggerService.LogActivity(ActivityActionType.UPDATED_class, currentUser.PersonNumber,
                //    classRepo.TableName, _oldclass, _class,
                //     $"Updated class of type '{((Core.classType)((int)_class.classTypeId)).ToString()}' for {_class.FromDate.ToString("dd-MM-yyyy")} to {_class.ToDate.ToString("dd-MM-yyyy")}");

            }
        }

        public async Task UpdateClassRoomStatus(long classRoomId, bool isActive)
        {
            var _classRoom = await classRoomRepo.GetById(classRoomId);
            if (_classRoom == null)
            {
                throw new AppException($"Invalid classroom id {classRoomId}");
            }
            else if (!isActive && _classRoom.ClassRoomStudents.Count > 0)
            {
                throw new AppException($"Classroom cannot be deactivated as there are still students in it");
            }
            else
            {
                var currentUser = accessor.HttpContext.GetUserSession();
                var _oldclassRoom = _classRoom.Clone<ClassRoom>();

               
                 _classRoom.IsActive = isActive;
                _classRoom.UpdatedBy = currentUser.Username;
                _classRoom.UpdatedDate = DateTimeOffset.Now;

                await classRoomRepo.Update(_classRoom, true);


                // log activity
                //await loggerService.LogActivity(ActivityActionType.UPDATED_class, currentUser.PersonNumber,
                //    classRepo.TableName, _oldclass, _class,
                //     $"Updated class of type '{((Core.classType)((int)_class.classTypeId)).ToString()}' for {_class.FromDate.ToString("dd-MM-yyyy")} to {_class.ToDate.ToString("dd-MM-yyyy")}");

            }
        }

        public IEnumerable<ClassRoom> GetClassRooms(bool includeInactive=false)
        {   
            var classRooms =  classRoomRepo.GetAll();
            if (!includeInactive)
            {
                classRooms = classRooms.Where(c => c.IsActive);
            }
            return classRooms.OrderBy(c => c.ClassId).ThenBy(c => c.RoomCode);
        }

        public async Task<ClassRoom> GetClassRoom(long id)
        {
            return await classRoomRepo.GetById(id);
        }
    }
}
