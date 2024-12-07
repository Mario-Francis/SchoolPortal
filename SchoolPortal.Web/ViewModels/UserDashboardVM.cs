using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class UserDashboardVM
    {
        public int StudentCount { get; set; }
        public int TeacherCount { get; set; }
        public int ParentCount { get; set; }
        public int ClassRoomCount { get; set; }
        public int MySubjectCount { get; set; }
        public int MyStudentCount { get; set; }
        public ClassRoomVM ClassRoom { get; set; }
        public int MyWardCount { get; set; }
    }
}
