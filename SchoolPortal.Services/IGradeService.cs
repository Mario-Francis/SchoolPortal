using SchoolPortal.Core;
using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
    public interface IGradeService
    {
        Task AddGrade(Grade grade);
        Task UpdateGrade(Grade grade);
        Task DeleteGrade(long gradeId);
        Task<Grade> GetGrade(long gradeId);
        Grade GetGrade(decimal score, TermSections section);
        Task<Grade> GetGradeAsync(decimal score, TermSections section);
        IEnumerable<Grade> GetGrades();
        IEnumerable<Grade> GetGrades(TermSections section);
    }
}
