using Microsoft.AspNetCore.Http;
using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
    public interface IStudentService
    {
        Task<long> CreateStudent(Student student);
        Task UpdateStudent(Student student);
        Task UpdateStudentProfile(Student student);
        Task UpdatePassword(PasswordRequestObject req);
        Task SetupPassword(PasswordRequestObject req);
        Task ResetPassword(long studentId);
        Task ResetPassword(PasswordRequestObject req);
        Task<bool> IsStudentAuthentic(LoginCredential credential);
        Task<Student> GetStudent(long studentId);
        Task<Student> GetStudent(string email);
        IEnumerable<Student> GetStudents(bool includeInactive = true);
        Task SendPasswordRecoveryMail(string email);
        Task DeleteStudent(long studentId);
        Task<string> GenerateUsername(string firstName, string lastName);
        Task<bool> AnyStudentExists();
        Task UpdateStudentStatus(long studentId, bool isActive);
        Task UpdateStudentGraduationStatus(long studentId, bool isGraduated);
        Task AddStudentGuardian(long studentId, long parentId, long relationshipId);
        Task RemoveStudentGuardian(long studentGuardianId);
        Task<string> UploadPhoto(long studentId, IFormFile file);
        Task DeletePhoto(long studentId);


        bool ValidateFile(IFormFile file, out List<string> errorItems);
        Task<IEnumerable<Student>> ExtractData(IFormFile file);
        Task BatchCreateStudent(IEnumerable<Student> students);
        byte[] ExportStudentsToExcel(int id);
        IEnumerable<Student> SearchStudents(string searchParam, long? classRoomId = null, int max = 50);
        Task<bool> VerifyEmail(long studentId, string token);

    }
}
