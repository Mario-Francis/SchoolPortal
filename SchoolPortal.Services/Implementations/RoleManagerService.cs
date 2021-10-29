using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SchoolPortal.Core.Models;
using SchoolPortal.Data.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolPortal.Services.Implementations
{
    public class RoleManagerService:IRoleManagerService
    {
        private readonly IRepository<Role> roleRepo;
        private readonly IHttpContextAccessor accessor;
        private readonly ILogger<RoleManagerService> logger;

        public RoleManagerService(
            IRepository<Role> roleRepo, 
            IHttpContextAccessor accessor, 
            ILogger<RoleManagerService> logger)
        {
            this.roleRepo = roleRepo;
            this.accessor = accessor;
            this.logger = logger;
        }
        // get single role with privileges by name and id
        public async Task<Role> GetRole(long roleId)
        {
            return await roleRepo.GetById(roleId);
        }

        public async Task<Role> GetRole(string name)
        {
            return await roleRepo.GetSingleWhere(r => r.Name == name);
        }
        // get all roles
        public IEnumerable<Role> GetRoles()
        {
            return roleRepo.GetAll();
        }
       
    }
}
