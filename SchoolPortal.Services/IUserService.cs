﻿using Microsoft.AspNetCore.Http;
using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
    public interface IUserService
    {
        Task<long> InitialUserSetup(User user);
        Task<long> CreateUser(User user);
        Task UpdateUser(User user);
        Task UpdatePassword(PasswordRequestObject req);
        Task SetupPassword(PasswordRequestObject req);
        Task<bool> IsUserAuthentic(LoginCredential credential);
        Task<User> GetUser(long userId);
        Task<User> GetUser(string email);
        IEnumerable<User> GetUsers(bool includeInactive = true);
        Task SendPasswordRecoveryMail(string email);
        Task DeleteUser(long userId);
        Task<string> GenerateUsername(string firstName, string lastName);
        Task<bool> AnyUserExists();
        IEnumerable<User> SearchTeachers(string searchParam, int max = 50);
        IEnumerable<User> SearchParents(string searchParam, int max = 50);
        Task UpdateUserStatus(long userId, bool isActive);
        Task AssignClassRoom(long userId, long? roomId);
        Task ResetPassword(long userId);
        Task ResetPassword(PasswordRequestObject req);
        Task AddParentWard(long studentId, long parentId, long relationshipId);
        Task RemoveParentWard(long studentGuardianId);
        Task<string> UploadPhoto(long userId, IFormFile file);
        Task DeletePhoto(long userId);

        bool ValidateFile(IFormFile file, out List<string> errorItems);
        Task<IEnumerable<User>> ExtractData(IFormFile file);
        Task BatchCreateUser(IEnumerable<User> users, long roleId);
        byte[] ExportUsersToExcel();

        Task<bool> VerifyEmail(long userId, string token);
    }
}
