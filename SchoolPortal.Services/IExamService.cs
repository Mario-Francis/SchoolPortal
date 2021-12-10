using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
    public interface IExamService
    {
        Task CreateExam(Exam exam);
        Task DeleteExam(long examId);
        Task UpdateExam(Exam exam);
        IEnumerable<Exam> GetExams();
        Task<Exam> GetExam(long id);
    }
}
