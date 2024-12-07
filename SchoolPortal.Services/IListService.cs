using SchoolPortal.Core.Models;
using System.Collections.Generic;

namespace SchoolPortal.Services
{
    public interface IListService
    {
        IEnumerable<Term> GetTerms();
        IEnumerable<TermSection> GetTermSection();
        IEnumerable<ClassType> GetClassTypes();
        IEnumerable<Role> GetRoles();
        IEnumerable<Class> GetClasses();
        IEnumerable<ClassRoom> GetClassRooms();
        IEnumerable<RoomCode> GetRoomCodes();
        IEnumerable<Relationship> GetRelationships();
        IEnumerable<ExamType> GetExamTypes();
        IEnumerable<Exam> GetExams();
    }
}
