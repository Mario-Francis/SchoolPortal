using SchoolPortal.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
    public interface IPerformanceRemarkService
    {
        Task CreateRemark(PerformanceRemark remark);
        Task DeleteRemark(long remarkId);
        Task UpdateRemark(PerformanceRemark remark);
        IEnumerable<PerformanceRemark> GetRemarks();
        Task<PerformanceRemark> GetRemark(long id);
        Task<PerformanceRemark> GetRemark(long examId, long studentId);
    }
}
