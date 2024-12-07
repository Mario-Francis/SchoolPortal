using Microsoft.AspNetCore.Http;
using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
    public interface IHealthRecordService
    {
        Task CreateRecord(HealthRecord record);
        Task DeleteRecord(long recordId);
        Task UpdateRecord(HealthRecord record);
        IEnumerable<HealthRecord> GetRecords();
        Task<HealthRecord> GetRecord(long id);
        Task<HealthRecord> GetRecord(string session, long termId, long studentId);
        Task<IEnumerable<HealthRecord>> ExtractData(IFormFile file);
        Task BatchCreateRecords(IEnumerable<HealthRecord> records, string session, long termId);

    }
}
