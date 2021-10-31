using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
    public interface IClassService
    {
        Task CreateClass(Class @class);
        Task DeleteClass(long classId);
        Task UpdateClass(Class @class);
        IEnumerable<Class> GetClasses();
        Task<Class> GetClass(long id);
    }
}
