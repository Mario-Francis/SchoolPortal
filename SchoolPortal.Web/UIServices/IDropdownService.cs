using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.UIServices
{
    public interface IDropdownService
    {
        IEnumerable<SelectListItem> GetTerms(string value = null);
        IEnumerable<SelectListItem> GetTermSections(string value = null);
        IEnumerable<SelectListItem> GetClassTypes(string value = null);
        IEnumerable<SelectListItem> GetRoles(string value = null);
        IEnumerable<SelectListItem> GetClasses(string value = null);
        IEnumerable<SelectListItem> GetClassRooms(string value = null);
        IEnumerable<SelectListItem> GetRoomCodes(string value = null);
    }
}
