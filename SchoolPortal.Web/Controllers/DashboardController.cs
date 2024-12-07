using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.Core;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Services;
using SchoolPortal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IStudentService studentService;
        private readonly IUserService userService;
        private readonly ILoggerService<DashboardController> logger;
        private readonly ISubjectService subjectService;
        private readonly IClassService classService;

        public DashboardController(
            IStudentService studentService,
            IUserService userService,
            ILoggerService<DashboardController> logger,
            ISubjectService subjectService,
            IClassService classService
            )
        {
            this.studentService = studentService;
            this.userService = userService;
            this.logger = logger;
            this.subjectService = subjectService;
            this.classService = classService;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.IsInRole(Constants.ROLE_STUDENT))
            {
                return await UserDashboard();
            }
            else
            {
                return await StudentDashboard();
            }
        }

        public IActionResult Default()
        {
            return View("Index");
        }

        [NonAction]
        public async Task<IActionResult> UserDashboard()
        {
            var studentCount = studentService.GetStudents().Count();
            var teacherCount = userService.GetUsers()
                .Where(u=> u.UserRoles.Any(r=>r.RoleId == (int)AppRoles.TEACHER)).Count();
            var parentCount = userService.GetUsers()
                .Where(u => u.UserRoles.Any(r => r.RoleId == (int)AppRoles.PARENT)).Count();
            var classRoomCount = classService.GetClassRooms().Count();

            var currentUser = HttpContext.GetUserSession();
            ClassRoomVM classRoom = null;
            var mySubjectCount = 0;
            var myStudentCount = 0;
            if (currentUser.HasClassRoom)
            {
                var  _classRoom = await classService.GetClassRoom(currentUser.ClassRoomId);
                classRoom = ClassRoomVM.FromClassRoom(_classRoom);
                myStudentCount = _classRoom.ClassRoomStudents.Count();
                mySubjectCount = subjectService.GetSubjects(classRoom.ClassId).Count();
            }

            var user = await userService.GetUser(currentUser.Id);
            var wardCount = user.StudentGuardians.Count();

            var data = new UserDashboardVM
            {
                StudentCount = studentCount,
                TeacherCount = teacherCount,
                ParentCount = parentCount,
                ClassRoomCount = classRoomCount,
                MySubjectCount = mySubjectCount,
                ClassRoom = classRoom,
                MyWardCount = wardCount,
                MyStudentCount=myStudentCount
            };

            return View("UserDashboard", data);
        }

        [NonAction]
        public async Task<IActionResult> StudentDashboard()
        {
            var currentUser = HttpContext.GetUserSession();
            var student = await studentService.GetStudent(currentUser.Id);
            var myGuardianCount = student.StudentGuardians.Count();
           
            ClassRoomVM classRoom = null;
            var mySubjectCount = 0;
           
            if (currentUser.HasClassRoom)
            {
                var _classRoom = await classService.GetClassRoom(currentUser.ClassRoomId);
                classRoom = ClassRoomVM.FromClassRoom(_classRoom);
                mySubjectCount = subjectService.GetSubjects(classRoom.ClassId).Count();
            }

            var data = new StudentDashboardVM
            {
                MySubjectCount = mySubjectCount,
                ClassRoom = classRoom,
                MyGuardianCount=myGuardianCount
            };

            return View("StudentDashboard", data);
        }
    }
}
