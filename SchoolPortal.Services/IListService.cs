using SchoolPortal.Core.Models;
using System.Collections.Generic;

namespace SchoolPortal.Services
{
    public interface IListService
    {
        IEnumerable<Term> GetTerms();
        IEnumerable<TermSection> GetTermSection();
        IEnumerable<ClassType> GetClassTypes();
        IEnumerable<Role> GetRoles();
    }
}
