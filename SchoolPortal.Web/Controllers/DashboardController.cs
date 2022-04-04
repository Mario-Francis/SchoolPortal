using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            if (!User.IsInRole(Constants.ROLE_STUDENT))
            {
                return UserDashboard();
            }
            else
            {
                return StudentDashboard();
            }
        }

        public IActionResult Default()
        {
            return View("Index");
        }

        [NonAction]
        public IActionResult UserDashboard()
        {
            return View("UserDashboard");
        }
        [NonAction]
        public IActionResult StudentDashboard()
        {
            return View("StudentDashboard");
        }
    }
}
