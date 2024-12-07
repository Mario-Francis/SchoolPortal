using SchoolPortal.Core;
using SchoolPortal.Core.Models;
using System;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
    public interface ILoggerService<T> where T : class
    {
        void LogException(Exception ex);

        public void LogInfo(string log);

        void LogError(string log);

        // activity logs
        Task LogActivity(ActivityActionType actionType, string actionBy, string descrirption = null);

        Task LogActivity<L>(ActivityActionType actionType, string actionBy, string tableName, L _from, L _to, string descrirption = null) where L : BaseEntity;
    }
}
