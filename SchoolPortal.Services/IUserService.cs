using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
    public interface IUserService
    {
        Task<long> InitialUserSetup(User user);
        Task<long> CreateUser(User user);
        Task UpdateUser(User user);
        Task UpdatePassword(PasswordRequestObject req);
        Task<bool> IsUserAuthentic(LoginCredential credential);
        Task<User> GetUser(long userId);
        Task<User> GetUser(string email);
        IEnumerable<User> GetUsers(bool includeInactive = true);
        Task DeleteUser(long userId);
        Task<string> GenerateUsername(string firstName, string lastName);
        Task<bool> AnyUserExists();
    }
}
