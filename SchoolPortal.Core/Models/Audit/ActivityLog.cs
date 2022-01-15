using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Core.Models.Audit
{
    public class ActivityLog:BaseEntity
    {
        public string ActionType { get; set; }
        public string ActionBy { get; set; }
        public string Description { get; set; }
        public string IPAddress { get; set; }

        // Navigation property
        public virtual AuditLog AuditLog { get; set; }
    }
}
