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

        Task CreateEndTermResult(EndTermResult result);
        Task DeleteEndTermResult(long resultId);
        Task UpdateEndTermResult(EndTermResult result);
        IEnumerable<EndTermResult> GetEndTermResults();
        IEnumerable<EndTermResultViewObject> GetEndTermResultViewObjects();
        Task<EndTermResult> GetEndTermResult(long id);
        Task<EndTermResultViewObject> GetEndTermResultViewObject(long id);
        Task<IEnumerable<EndTermResult>> ExtractEndTermData(IFormFile file);
        Task BatchCreateEndTermResults(IEnumerable<EndTermResult> results, long examId, long subjectId, long classId);
    }
}
