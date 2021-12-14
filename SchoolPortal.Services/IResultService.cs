using Microsoft.AspNetCore.Http;
using SchoolPortal.Core.Models;
using SchoolPortal.Core.Models.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
    public interface IResultService
    {
        Task CreateMidTermResult(MidTermResult result);
        Task DeleteMidTermResult(long resultId);
        Task UpdateMidTermResult(MidTermResult result);
        IEnumerable<MidTermResult> GetMidTermResults();
        IEnumerable<MidTermResultViewObject> GetMidTermResultViewObjects();
        Task<MidTermResult> GetMidTermResult(long id);
        Task<MidTermResultViewObject> GetMidTermResultViewObject(long id);

        bool ValidateFile(IFormFile file, out List<string> errorItems);
        Task<IEnumerable<MidTermResult>> ExtractMidTermData(IFormFile file);
        Task BatchCreateMidTermResults(IEnumerable<MidTermResult> results, long examId, long subjectId, long classId);
    }
}
