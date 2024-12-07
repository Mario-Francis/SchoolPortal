using SchoolPortal.Core.Models;

namespace SchoolPortal.Core.DTOs
{
    public class AttendanceRecordObject
    {
        public long Id { get; set; }
        public int SchoolOpenCount { get; set; }
        public int PresentCount { get; set; }

        public static AttendanceRecordObject FromAttendanceRecord(AttendanceRecord record)
        {
            return record == null ? null : new AttendanceRecordObject
            {
                Id = record.Id,
                SchoolOpenCount = record.SchoolOpenCount,
                PresentCount = record.PresentCount
            };
        }
    }
}
