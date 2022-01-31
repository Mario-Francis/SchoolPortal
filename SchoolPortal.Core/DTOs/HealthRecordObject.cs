using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Core.DTOs
{
    public class HealthRecordObject
    {
        public long Id { get; set; }
        public decimal StartHeight { get; set; }
        public decimal? EndHeight { get; set; }
        public decimal StartWeight { get; set; }
        public decimal? EndWeight { get; set; }

        public static HealthRecordObject FromHealthRecord(HealthRecord record)
        {
            return record == null ? null : new HealthRecordObject
            {
                Id = record.Id,
                StartHeight = record.StartHeight,
                EndHeight = record.EndHeight,
                StartWeight = record.StartWeight,
                EndWeight = record.EndWeight
            };
        }
    }
}
