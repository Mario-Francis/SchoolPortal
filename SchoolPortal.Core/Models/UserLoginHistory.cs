using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SchoolPortal.Core.Models
{
    public class UserLoginHistory:BaseEntity
    {
        public long? UserId { get; set; }
        public string IPAddress { get; set; }
        public DateTimeOffset LoginDate { get; set; }
        public string Status { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
