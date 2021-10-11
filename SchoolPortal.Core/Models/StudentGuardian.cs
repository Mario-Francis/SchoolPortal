﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace SchoolPortal.Core.Models
{
    public class StudentGuardian:BaseEntity,IUpdatable
    {
        public long StudentId { get; set; }
        public long GuardianId { get; set; }
        public long RelationshipId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

        //Navigation Properties
        [JsonIgnore]
        [NotMapped]
        public virtual User UpdatedByUser { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
        [ForeignKey("GuardianId")]
        public virtual User Guardian { get; set; }
        [ForeignKey("RelationshipId")]
        public virtual Relationship Relationship { get; set; }
    }
}
