using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class StudentDashboardVM
    {
        public int MySubjectCount { get; set; }
        public int MyGuardianCount { get; set; }
        public ClassRoomVM ClassRoom { get; set; }
    }
}
