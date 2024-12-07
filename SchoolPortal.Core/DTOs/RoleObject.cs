using SchoolPortal.Core.Models;

namespace SchoolPortal.Core.DTOs
{
    public class RoleObject
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public static RoleObject FromRole(Role role)
        {
            return new RoleObject
            {
                Id = role.Id,
                Name = role.Name
            };
        }
    }
}
