using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
    public interface ISubjectService
    {
        Task CreateSubject(Subject subject);
        Task DeleteSubject(long SubjectId);
        Task UpdateSubject(Subject subject);
        IEnumerable<Subject> GetSubjects();
        IEnumerable<Subject> GetSubjects(long classId);
        Task<Subject> GetSubject(long id);
    }
}
