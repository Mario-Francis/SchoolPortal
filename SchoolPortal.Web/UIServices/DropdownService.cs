using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolPortal.Services;
using System.Collections.Generic;
using System.Linq;

namespace SchoolPortal.Web.UIServices
{
    public class DropdownService:IDropdownService
    {
        private readonly IListService listService;

        public DropdownService(IListService listService)
        {
            this.listService = listService;
        }

        public IEnumerable<SelectListItem> GetTerms(string value=null)
        {
            List<SelectListItem> terms =  listService.GetTerms()
                .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected=c.Id.ToString() == value }).ToList();
            terms.Insert(0, new SelectListItem { Text = "- Select term -", Value = "" });
            return terms;
        }

        public IEnumerable<SelectListItem> GetTermSections(string value = null)
        {
            List<SelectListItem> termSections = listService.GetTermSection()
                .Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString(), Selected = t.Id.ToString() == value }).ToList();

            termSections.Insert(0, new SelectListItem { Text = "- Select term section -", Value = "" });
            return termSections;
        }

        public IEnumerable<SelectListItem> GetClassTypes(string value = null)
        {
            List<SelectListItem> classTypes = listService.GetClassTypes()
                .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = c.Id.ToString() == value }).ToList();

            classTypes.Insert(0, new SelectListItem { Text = "- Select class types -", Value = "" });
            return classTypes;
        }


        public IEnumerable<SelectListItem> GetRoles(string value = null)
        {
            List<SelectListItem> roles = listService.GetRoles()
                .Select(l => new SelectListItem { Text = l.Name, Value = l.Id.ToString(), Selected = l.Id.ToString() == value }).ToList();

            roles.Insert(0, new SelectListItem { Text = "- Select role -", Value = "" });
            return roles;
        }
    }
}
