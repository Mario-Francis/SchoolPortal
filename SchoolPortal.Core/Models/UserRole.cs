using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SchoolPortal.Core.Models
{
    public class UserRole:BaseEntity
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }

        // Navigationn Properties
        [ForeignKey("UserId")]
        public virtual  User User { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }
}
