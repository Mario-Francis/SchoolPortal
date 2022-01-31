using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
    public interface IStudentResultService
    {
        IEnumerable<string> GetResultSessions(long studentId);
        IEnumerable<Term> GetResultSessionTerms(long studentId, string session);
        IEnumerable<StudentResultItem> GetMidTermResults(long studentId, string session, long termId);
        IEnumerable<StudentResultItem> GetEndTermResults(long studentId, string session, long termId);
        IEnumerable<BehaviouralResult> GetEndTermBehaviouralRatings(long studentId, string session, long termId);
        IEnumerable<StudentResultItem> GetEndOfSessionResults(long studentId, string session);
        Task<StudentResult> GetStudentResult(long studentId, string session, long termId);

    }
}
