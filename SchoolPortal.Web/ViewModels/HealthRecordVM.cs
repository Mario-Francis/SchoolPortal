using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class HealthRecordVM
    {
        public long Id { get; set; }
        public decimal StartHeight { get; set; }
        public decimal? EndHeight { get; set; }
        public decimal StartWeight { get; set; }
        public decimal? EndWeight { get; set; }

        public long StudentId { get; set; }
        public long TermId { get; set; }
        public string Session { get; set; }

        public HealthRecord ToHealthRecord()
        {
            return new HealthRecord
            {
                Id = Id,
                StartHeight = StartHeight,
                EndHeight = EndHeight,
                StartWeight = StartWeight,
                EndWeight = EndWeight,

                Session = Session,
                StudentId = StudentId,
                TermId = TermId
            };
        }

        public static HealthRecordVM FromHealthRecordObject(HealthRecordObject record)
        {
            return record == null ? null : new HealthRecordVM
            {
                Id = record.Id,
                StartHeight = record.StartHeight,
                EndHeight = record.EndHeight,
                StartWeight = record.StartWeight,
                EndWeight = record.EndWeight
            };
        }

        public static HealthRecord FromHealthRecord(HealthRecord record)
        {
            return record == null ? null : new HealthRecord
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
