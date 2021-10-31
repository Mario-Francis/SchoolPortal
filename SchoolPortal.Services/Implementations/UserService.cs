using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SchoolPortal.Core;
using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Core.Models;
using SchoolPortal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Services.Implementations
{
    public class UserService:IUserService
    {
        private readonly IRepository<User> userRepo;
        private readonly IRepository<Role> roleRepo;
        private readonly IEmailService emailService;
        private readonly IPasswordService passwordService;
        private readonly ILogger<UserService> logger;
        private readonly IHttpContextAccessor accessor;
        private readonly ITokenService tokenService;

        public UserService(
            IRepository<User> userRepo, 
            IRepository<Role> roleRepo, 
            IEmailService emailService, 
            IPasswordService passwordService, 
            ILogger<UserService> logger, 
            IHttpContextAccessor accessor, 
            ITokenService tokenService)
        {
            this.userRepo = userRepo;
            this.roleRepo = roleRepo;
            this.emailService = emailService;
            this.passwordService = passwordService;
            this.logger = logger;
            this.accessor = accessor;
            this.tokenService = tokenService;
        }

        // Initial sysadmin user setup
        public async Task<long> InitialUserSetup(User user)
        {
            if (user == null)
            {
                throw new AppException("User object is required");
            }
            if( !await emailService.IsEmailValidAsync(user.Email))
            {
                throw new AppException($"Email '{user.Email}' is not valid");
            }
            if(!passwordService.ValidatePassword(user.Password, out string passwordValidationMessage))
            {
                throw new AppException(passwordValidationMessage);
            }

            //var user = req.ToUser();
            user.Password = passwordService.Hash(user.Password);
            user.Username = await GenerateUsername(user.FirstName,  user.Surname);
            user.IsActive = true;
            //user.CreatedBy = Constants.SYSTEM_NAME;
            user.CreatedDate = DateTimeOffset.Now;
            //user.UpdatedBy = Constants.SYSTEM_NAME;
            user.UpdatedDate = DateTimeOffset.Now;
            user.UserRoles = new List<UserRole>() {
                new UserRole
                {
                    RoleId = (int)AppRoles.ADMINISTRATOR,
                    CreatedBy = Constants.SYSTEM_NAME
                }
            };

            await userRepo.Insert(user, true);
            user.CreatedBy = user.Username;
            user.UpdatedBy = user.Username;
            await userRepo.Update(user, true);

            // log action
            //await loggerService.LogActivity(
            //    $"System Administrator User Setup with email '{user.Email}'",
            //    ActivityType.CREATE_USER,
            //    user.Id);

            return user.Id;
        }

        // create user
        public async Task<long> CreateUser(User user)
        {
            if (user == null)
            {
                throw new AppException("User object is required");
            }
            if (user.UserRoles == null || user.UserRoles.Count == 0)
            {
                throw new AppException($"Role is required");
            }
            if (!await emailService.IsEmailValidAsync(user.Email))
            {
                throw new AppException($"Email '{user.Email}' is not valid");
            }
            if (await userRepo.Any(u => u.Email == user.Email))
            {
                throw new AppException($"A user with email '{user.Email}' already exist");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
            user.Username = await GenerateUsername(user.FirstName, user.Surname);
            user.Password = passwordService.Hash(Constants.DEFAULT_NEW_USER_PASSWORD);
            user.CreatedBy = currentUser.Username;
            user.UpdatedBy = currentUser.Username;
            await userRepo.Insert(user, true);

            var token = tokenService.GenerateTokenFromData(user.Id.ToString());

            //// log action
            //await loggerService.LogActivity(
            //    $"Created new user with email '{user.Email}'",
            //    ActivityType.CREATE_USER,
            //    currentUser.UserId);

            // send welcome/email verification mail
#warning Schedule welcome/email verification mail

            return user.Id;
        }

        // update user
        public async Task UpdateUser(User user)
        {
            if (user == null)
            {
                throw new AppException("User object is required");
            }
            var _user = await userRepo.GetById(user.Id);
            if (user == null)
            {
                throw new AppException($"User with id '{user.Id}' does not exist");
            }
            if (!await emailService.IsEmailValidAsync(user.Email))
            {
                throw new AppException($"Email '{user.Email}' is not valid");
            }
            if (await userRepo.Any(u => u.Email == user.Email) && user.Email != _user.Email)
            {
                throw new AppException($"A user with email '{user.Email}' already exist");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
            var oldUser = _user.Clone<User>();
            _user.FirstName = user.FirstName;
            _user.MiddleName = user.MiddleName;
            _user.Surname = user.Surname;
            _user.PhoneNumber = user.PhoneNumber;
            _user.Email = user.Email;
            _user.Gender = user.Gender;
            _user.DateOfBirth = user.DateOfBirth;
            _user.UpdatedBy = currentUser.Username;
            _user.UpdatedDate = DateTimeOffset.Now;

            await userRepo.Update(user, false);
            // log action
            //await loggerService.LogActivity(
            //    $"Updated user",
            //    ActivityType.UPDATE_USER,
            //    currentUser.UserId, userRepository.TableName, oldUser, user);
        }
        
        // update password
        public async Task UpdatePassword(PasswordRequestObject req)
        {
            if (req == null)
            {
                throw new AppException("Password object is required");
            }
            var user = await userRepo.GetById(req.UserId);
            if (user == null)
            {
                throw new AppException($"User with id '{req.UserId}' does not exist");
            }
            if(!passwordService.Verify(req.Password, user.Password))
            {
                throw new AppException($"Invalid current password");
            }
            if (!passwordService.ValidatePassword(req.NewPassword, out string passwordValidationMessage))
            {
                throw new AppException($"New password validation error: {passwordValidationMessage}");
            }
            if (!req.NewPasswordMatch)
            {
                throw new AppException($"New passwords don't match");
            }
            var currentUser = accessor.HttpContext.GetUserSession();
            user.Password = passwordService.Hash(req.NewPassword);
            user.UpdatedBy = currentUser.Username;
            user.UpdatedDate = DateTimeOffset.Now;

            await userRepo.Update(user, true);

            // log action
            //await loggerService.LogActivity(
            //    $"Updated password",
            //    ActivityType.UPDATE_PASSWORD,
            //    currentUser.UserId);
        }

        // authenticate user
        public async Task<bool> IsUserAuthentic(LoginCredential credential)
        {
            if (string.IsNullOrEmpty(credential?.Email))
            {
                throw new AppException($"Email/Username is required");
            }
            if (string.IsNullOrEmpty(credential?.Password))
            {
                throw new AppException($"Password is required");
            }
            var user = await userRepo.GetSingleWhere(u => u.Email == credential.Email || u.Username == credential.Email);
            if (user==null)
            {
                throw new AppException($"Email/username is invalid");
            }
            if (!passwordService.Verify(credential.Password, user.Password))
            {
                throw new AppException($"Password is invalid");
            }
            return true;
        }

        // get user
        public async Task<User> GetUser(long userId)
        {
            var user =  await userRepo.GetById(userId);
            if (user == null)
                throw new AppException($"User with id: '{userId}' does not exist");
            else
                return user;
        }

        public async Task<User> GetUser(string email)
        {
            var user = await userRepo.GetSingleWhere(u=>u.Email == email || u.Username == email);
            if (user == null)
                throw new AppException($"User with email or username: '{email}' does not exist");
            else
                return user;
        }
       
        // get users
        public IEnumerable<User> GetUsers(bool includeInactive=true)
        {
            var users = userRepo.GetAll();
            if (!includeInactive)
            {
                users = users.Where(u => u.IsActive);
            }
            return users;
        }

#warning send password recovery mail

        // delete user
        public async Task DeleteUser(long userId)
        {
            try
            {
                var user = await userRepo.GetById(userId);
                if (user == null)
                {
                    throw new AppException($"No user with id '{userId}' exist");
                }

                await userRepo.Delete(user.Id, true);

                var currentUser = accessor.HttpContext.GetUserSession();
                //// log action
                //await loggerService.LogActivity(
                //    $"Deleted user with email '{user.Email}'",
                //    ActivityType.DELETE_USER,
                //    currentUser.UserId);
            }
            catch (Exception ex)
            {
                throw new AppException($"User cannot be deleted as user is still associated with one or more entities");
            }
        }

       // generate username
       public async Task<string> GenerateUsername(string firstName, string lastName)
        {
            var uname = $"{firstName.ToLower()}.{lastName.ToLower()}";
            var cnt = 1;
            while (await userRepo.Any(u => u.Username == uname))
            {
                uname += cnt.ToString();
                cnt++;
            }

            return uname;
        }
    
        public async Task<bool> AnyUserExists()
        {
            return (await userRepo.Count()) > 0;
        }

        public IEnumerable<User> SearchTeachers(string searchParam, int max=50)
        {
            var teacherRoleId = (int)AppRoles.TEACHER;
            if (string.IsNullOrEmpty(searchParam))
            {
                return userRepo.GetWhere(u => u.UserRoles.Any(u => u.RoleId == teacherRoleId)).Take(max);
            }
            else
            {
                searchParam = searchParam.ToLower();
                var teachers = userRepo.GetWhere(u => u.UserRoles.Any(u => u.RoleId == teacherRoleId) &&
                (u.Username.ToLower().Contains(searchParam) || u.Email.ToLower().Contains(searchParam)
                || u.PhoneNumber.ToLower().Contains(searchParam) || u.FirstName.ToLower().Contains(searchParam)
                || u.MiddleName.ToLower().Contains(searchParam) || u.Surname.ToLower().Contains(searchParam))).Take(max);

                return teachers;
            }
           
        }
    
    }
}
