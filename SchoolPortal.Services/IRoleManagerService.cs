using SchoolPortal.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
    public interface IRoleManagerService
    {
        Task<Role> GetRole(long roleId);

        Task<Role> GetRole(string name);
        // get all roles
        IEnumerable<Role> GetRoles();
    }
}
