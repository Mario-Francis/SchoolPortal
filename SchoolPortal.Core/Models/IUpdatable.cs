using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Core.Models
{
    public interface IUpdatable
    {
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        // Navigation
        public User UpdatedByUser { get; set; }
    }
}
