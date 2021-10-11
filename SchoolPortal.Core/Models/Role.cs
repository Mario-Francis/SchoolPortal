using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Core.Models
{
    public class Role:BaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
