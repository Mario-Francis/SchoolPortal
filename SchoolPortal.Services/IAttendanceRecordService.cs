using Microsoft.AspNetCore.Http;
using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
    public interface IAttendanceRecordService
    {
        Task CreateRecord(AttendanceRecord record);
        Task DeleteRecord(long recordId);
        Task UpdateRecord(AttendanceRecord record);
        IEnumerable<AttendanceRecord> GetRecords();
        Task<AttendanceRecord> GetRecord(long id);
        Task<AttendanceRecord> GetRecord(string session, long termId, long studentId);
        Task<IEnumerable<AttendanceRecord>> ExtractData(IFormFile file);
        Task BatchCreateRecords(IEnumerable<AttendanceRecord> records, string session, long termId, int schoolOpenCount);
    }
}
