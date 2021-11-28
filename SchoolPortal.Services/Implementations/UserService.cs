using ClosedXML.Excel;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchoolPortal.Core;
using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Core.Models;
using SchoolPortal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> userRepo;
        private readonly IRepository<StudentGuardian> guardianRepo;
        private readonly IRepository<Student> studentRepo;
        private readonly IRepository<Relationship> relationshipRepo;
        private readonly IRepository<Role> roleRepo;
        private readonly IRepository<UserRole> userRoleRepo;
        private readonly IEmailService emailService;
        private readonly IPasswordService passwordService;
        private readonly ILogger<UserService> logger;
        private readonly IHttpContextAccessor accessor;
        private readonly ITokenService tokenService;
        private readonly IMailService mailService;
        private readonly AppSettings appSettings;
        private string[] headers = new string[] { "SN", "First Name", "Middle Name", "Surname", "Gender", "Date of Birth", "Email", "Phone Number" };

        public UserService(
            IRepository<User> userRepo,
             IRepository<StudentGuardian> guardianRepo,
            IRepository<Student> studentRepo,
            IRepository<Relationship> relationshipRepo,
            IRepository<Role> roleRepo,
            IRepository<UserRole> userRoleRepo,
            IEmailService emailService,
            IPasswordService passwordService,
            ILogger<UserService> logger,
            IHttpContextAccessor accessor,
            ITokenService tokenService,
            IMailService mailService,
            IOptions<AppSettings> appSettings)
        {
            this.userRepo = userRepo;
            this.guardianRepo = guardianRepo;
            this.studentRepo = studentRepo;
            this.relationshipRepo = relationshipRepo;
            this.roleRepo = roleRepo;
            this.userRoleRepo = userRoleRepo;
            this.emailService = emailService;
            this.passwordService = passwordService;
            this.logger = logger;
            this.accessor = accessor;
            this.tokenService = tokenService;
            this.mailService = mailService;
            this.appSettings = appSettings.Value;
        }

        // Initial sysadmin user setup
        public async Task<long> InitialUserSetup(User user)
        {
            if (user == null)
            {
                throw new AppException("User object is required");
            }
            if (!await emailService.IsEmailValidAsync(user.Email))
            {
                throw new AppException($"Email '{user.Email}' is not valid");
            }
            if (!passwordService.ValidatePassword(user.Password, out string passwordValidationMessage))
            {
                throw new AppException(passwordValidationMessage);
            }

            //var user = req.ToUser();
            user.Password = passwordService.Hash(user.Password);
            user.Username = await GenerateUsername(user.FirstName, user.Surname);
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
            user.IsActive = true;
            user.CreatedBy = currentUser.Username;
            user.UpdatedBy = currentUser.Username;
            user.UserRoles = user.UserRoles.Select(ur =>
            {
                ur.CreatedBy = currentUser.Username;
                return ur;
            }).ToList();
            await userRepo.Insert(user, true);

            var token = tokenService.GenerateTokenFromData(user.Id.ToString());
            user.EmailVerificationToken = token;
            await userRepo.Update(user, true);

            //// log action
            //await loggerService.LogActivity(
            //    $"Created new user with email '{user.Email}'",
            //    ActivityType.CREATE_USER,
            //    currentUser.UserId);

            // send welcome/email verification mail

            var mail = new MailObject
            {
                Recipients = new List<Recipient> {
                    new Recipient {
                        FirstName=user.FirstName,
                        LastName=user.Surname,
                        Email=user.Email,
                        Username=user.Username,
                        Password= Constants.DEFAULT_NEW_USER_PASSWORD,
                        Token=token
                    }
                }
            };
            await mailService.ScheduleEmailConfirmationMail(mail);

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
            if (user.UserRoles == null || user.UserRoles.Count == 0)
            {
                throw new AppException($"Role is required");
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
            if (user.Id == currentUser.Id && _user.UserRoles.Any(ur => ur.RoleId == (long)AppRoles.ADMINISTRATOR) && !user.UserRoles.Any(ur => ur.RoleId == (long)AppRoles.ADMINISTRATOR))
            {
                throw new AppException($"You cannot unassign the Administartor role from yourself. You can make another user an administrator and then the user can unassign the role from you");
            }
            if (_user.UserRoles.Any(ur => ur.RoleId == (long)AppRoles.PARENT) && !user.UserRoles.Any(ur => ur.RoleId == (long)AppRoles.PARENT))
            {
                _user.StudentGuardians.Clear();
            }

            if (_user.UserRoles.Any(ur => ur.RoleId == (long)AppRoles.TEACHER) && !user.UserRoles.Any(ur => ur.RoleId == (long)AppRoles.TEACHER))
            {
                _user.ClassRoomTeachers.Clear();
            }

            _user.FirstName = user.FirstName;
            _user.MiddleName = user.MiddleName;
            _user.Surname = user.Surname;
            _user.PhoneNumber = user.PhoneNumber;
            _user.Email = user.Email;
            _user.Gender = user.Gender;
            _user.DateOfBirth = user.DateOfBirth;
            _user.UserRoles.Clear();
            _user.UserRoles.AddRange(user.UserRoles.Select(ur =>
            {
                ur.UserId = user.Id;
                ur.CreatedBy = currentUser.Username;
                return ur;
            }).ToList());

            _user.UpdatedBy = currentUser.Username;
            _user.UpdatedDate = DateTimeOffset.Now;

            await userRepo.Update(_user, true);
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
            if (!passwordService.Verify(req.Password, user.Password))
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

        // reset password
        public async Task ResetPassword(long userId)
        {
            var user = await userRepo.GetById(userId);
            if (user == null)
            {
                throw new AppException($"User with id '{userId}' does not exist");
            }
            var newPassword = Constants.DEFAULT_NEW_USER_PASSWORD;
            var currentUser = accessor.HttpContext.GetUserSession();
            user.Password = passwordService.Hash(newPassword);
            user.UpdatedBy = currentUser.Username;
            user.UpdatedDate = DateTimeOffset.Now;

            await userRepo.Update(user, true);

            // log action
            //await loggerService.LogActivity(
            //    $"Updated password",
            //    ActivityType.UPDATE_PASSWORD,
            //    currentUser.UserId);

            var mail = new MailObject
            {
                Recipients = new List<Recipient> {
                    new Recipient {
                        FirstName=user.FirstName,
                        LastName=user.Surname,
                        Email=user.Email,
                        Password= newPassword
                    }
                }
            };
            await mailService.SchedulePasswordResetMail(mail);

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
            if (user == null)
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
            var user = await userRepo.GetById(userId);
            if (user == null)
                throw new AppException($"User with id: '{userId}' does not exist");
            else
                return user;
        }

        public async Task<User> GetUser(string email)
        {
            var user = await userRepo.GetSingleWhere(u => u.Email == email || u.Username == email);
            if (user == null)
                throw new AppException($"User with email or username: '{email}' does not exist");
            else
                return user;
        }

        // get users
        public IEnumerable<User> GetUsers(bool includeInactive = true)
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

                if (user.ClassRoomTeachers.Count > 0 || user.StudentGuardians.Count > 0 || user.UserLoginHistories.Count > 0)
                {
                    throw new AppException($"User cannot be deleted as user is still associated with one or more entities");
                }

                await userRoleRepo.DeleteRange(user.UserRoles.Select(ur => ur.Id), false);
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
                logger.LogError(ex, ex.Message);
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

        public IEnumerable<User> SearchTeachers(string searchParam, int max = 50)
        {
            var teacherRoleId = (int)AppRoles.TEACHER;
            if (string.IsNullOrEmpty(searchParam))
            {
                return userRepo.GetWhere(u => u.UserRoles.Any(u => u.RoleId == teacherRoleId) && u.IsActive).Take(max);
            }
            else
            {
                searchParam = searchParam.ToLower();
                var teachers = userRepo.GetWhere(u => u.UserRoles.Any(u => u.RoleId == teacherRoleId) && u.IsActive &&
                (u.Username.ToLower().Contains(searchParam) || u.Email.ToLower().Contains(searchParam)
                || u.PhoneNumber.ToLower().Contains(searchParam) || u.FirstName.ToLower().Contains(searchParam)
                || u.MiddleName.ToLower().Contains(searchParam) || u.Surname.ToLower().Contains(searchParam))).Take(max);

                return teachers;
            }

        }

        public IEnumerable<User> SearchParents(string searchParam, int max = 50)
        {
            var parentRoleId = (int)AppRoles.PARENT;
            if (string.IsNullOrEmpty(searchParam))
            {
                return userRepo.GetWhere(u => u.UserRoles.Any(u => u.RoleId == parentRoleId) && u.IsActive).Take(max);
            }
            else
            {
                searchParam = searchParam.ToLower();
                var teachers = userRepo.GetWhere(u => u.UserRoles.Any(u => u.RoleId == parentRoleId) && u.IsActive &&
                (u.Username.ToLower().Contains(searchParam) || u.Email.ToLower().Contains(searchParam)
                || u.PhoneNumber.ToLower().Contains(searchParam) || u.FirstName.ToLower().Contains(searchParam)
                || u.MiddleName.ToLower().Contains(searchParam) || u.Surname.ToLower().Contains(searchParam))).Take(max);

                return teachers;
            }

        }


        public async Task UpdateUserStatus(long userId, bool isActive)
        {
            var _user = await userRepo.GetById(userId);
            if (_user == null)
            {
                throw new AppException($"Invalid user id {userId}");
            }
            else
            {
                var currentUser = accessor.HttpContext.GetUserSession();
                if (userId == currentUser.Id)
                {
                    throw new AppException($"Sorry! You cannot change your own status");
                }
                var _olduser = _user.Clone<User>();


                _user.IsActive = isActive;
                _user.UpdatedBy = currentUser.Username;
                _user.UpdatedDate = DateTimeOffset.Now;

                await userRepo.Update(_user, true);


                // log activity
                //await loggerService.LogActivity(ActivityActionType.UPDATED_class, currentUser.PersonNumber,
                //    classRepo.TableName, _oldclass, _class,
                //     $"Updated class of type '{((Core.classType)((int)_class.classTypeId)).ToString()}' for {_class.FromDate.ToString("dd-MM-yyyy")} to {_class.ToDate.ToString("dd-MM-yyyy")}");

            }
        }

        public async Task AssignClassRoom(long userId, long? roomId)
        {
            var _user = await userRepo.GetById(userId);
            if (_user == null)
            {
                throw new AppException($"Invalid user id {userId}");
            }
            else
            {
                var currentUser = accessor.HttpContext.GetUserSession();

                var _olduser = _user.Clone<User>();

                _user.ClassRoomTeachers.Clear();
                if (roomId != null)
                {
                    _user.ClassRoomTeachers.Add(new ClassRoomTeacher
                    {
                        ClassRoomId = roomId.Value,
                        TeacherId = userId,
                        CreatedBy = currentUser.Username,
                        UpdatedBy = currentUser.Username,
                        CreatedDate = DateTimeOffset.Now,
                        UpdatedDate = DateTimeOffset.Now
                    });
                }
                _user.UpdatedBy = currentUser.Username;
                _user.UpdatedDate = DateTimeOffset.Now;

                await userRepo.Update(_user, true);


                // log activity
                //await loggerService.LogActivity(ActivityActionType.UPDATED_class, currentUser.PersonNumber,
                //    classRepo.TableName, _oldclass, _class,
                //     $"Updated class of type '{((Core.classType)((int)_class.classTypeId)).ToString()}' for {_class.FromDate.ToString("dd-MM-yyyy")} to {_class.ToDate.ToString("dd-MM-yyyy")}");

            }
        }
        public async Task AddParentWard(long studentId, long parentId, long relationshipId)
        {
            var student = await studentRepo.GetById(studentId);
            if (student == null)
            {
                throw new AppException($"Ward is not found");
            }
            var parent = await userRepo.GetById(parentId);
            if (parent == null)
            {
                throw new AppException($"Parent is not found");
            }
            var relationship = await relationshipRepo.GetById(relationshipId);
            if (relationship == null)
            {
                throw new AppException($"Relationship is not found");
            }
            if (await guardianRepo.Any(g => g.StudentId == studentId && g.GuardianId == parentId))
            {
                throw new AppException($"Ward already exist");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
            var studentGuardian = new StudentGuardian
            {
                GuardianId = parentId,
                StudentId = studentId,
                RelationshipId = relationshipId,
                CreatedBy = currentUser.Username,
                CreatedDate = DateTimeOffset.Now,
                UpdatedBy = currentUser.Username,
                UpdatedDate = DateTimeOffset.Now
            };
            await guardianRepo.Insert(studentGuardian);

            // log 
        }

        public async Task RemoveParentWard(long studentGuardianId)
        {
            var studentGuardian = await guardianRepo.GetById(studentGuardianId);
            if (studentGuardian == null)
            {
                throw new AppException($"Parent ward is not found");
            }
            await guardianRepo.Delete(studentGuardian.Id);

            // log 
        }

        //=========== Batch Upload ===========
        public bool ValidateFile(IFormFile file, out List<string> errorItems)
        {
            bool isValid = true;
            List<string> errList = new List<string>();
            var maxUploadSize = appSettings.MaxUploadSize;
            if (file == null)
            {
                isValid = false;
                errList.Add("No file uploaded.");
            }
            else
            {
                if (file.Length > (maxUploadSize * 1024 * 1024))
                {
                    isValid = false;
                    errList.Add($"Max upload size exceeded. Max size is {maxUploadSize}MB");
                }
                var ext = Path.GetExtension(file.FileName);
                if (ext != ".xls" && ext != ".xlsx")
                {
                    isValid = false;
                    errList.Add($"Invalid file format. Supported file formats include .xls and .xlsx");
                }
            }
            errorItems = errList;
            return isValid;
        }

        private bool ValidateHeader(DataRow row, string[] headers, out string errorMessage)
        {
            var err = "";
            var isValid = true;
            for (int i = 0; i < headers.Length; i++)
            {
                if (row[i] == null || Convert.ToString(row[i]).Trim().ToLower() != headers[i].ToLower())
                {
                    isValid = false;
                    err = $"Invalid header value at column {i + 1}. Expected value is {headers[i]}";
                    break;
                }
            }
            errorMessage = err;
            return isValid;
        }

        private async Task<(bool isValid, string errorMessage)> ValidateDataRow(int index, DataRow row)
        {
            var err = "";
            var isValid = true;
            if (row[1] == null || Convert.ToString(row[1]).Trim() == "")
            {
                isValid = false;
                err = $"Invalid value for {headers[1]} at row {index}. Field is required.";
            }
            else if (row[3] == null || Convert.ToString(row[3]).Trim() == "")
            {
                isValid = false;
                err = $"Invalid value for {headers[3]} at row {index}. Field is required.";
            }
            else if (row[4] == null || Convert.ToString(row[4]).Trim() == "")
            {
                isValid = false;
                err = $"Invalid value for {headers[4]} at row {index}. Field is required.";
            }
            else if (!(new string[] { "male", "female" }).Contains(Convert.ToString(row[4]).Trim().ToLower())) // gender
            {
                isValid = false;
                err = $"Invalid value for {headers[4]} at row {index}. Value can either be 'Male' or 'Female'.";
            }
            else if (row[5] != null && !DateTimeOffset.TryParse(Convert.ToString(row[5]).Trim(), out DateTimeOffset _))
            {
                isValid = false;
                err = $"Invalid value for {headers[5]} at row {index}. A valid date value expected.";
            }
            else if (row[6] == null || Convert.ToString(row[6]).Trim() == "")
            {
                isValid = false;
                err = $"Invalid value for {headers[6]} at row {index}. Field is required.";
            }
            else if (row[6] != null && !(await emailService.IsEmailValidAsync(Convert.ToString(row[6]).Trim(), false)))
            {
                isValid = false;
                err = $"Invalid value for {headers[6]} at row {index}. Field is required.";
            }
            else if (row[7] == null || Convert.ToString(row[7]).Trim() == "")
            {
                isValid = false;
                err = $"Invalid value for {headers[7]} at row {index}. Field is required.";
            }

            return (isValid, err);
        }

        public async Task<IEnumerable<User>> ExtractData(IFormFile file)
        {
            List<User> users = new List<User>();
            IExcelDataReader excelReader = null;
            DataSet dataSet = new DataSet();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var fileStream = file.OpenReadStream();

            if (file.FileName.EndsWith(".xls"))
                excelReader = ExcelReaderFactory.CreateBinaryReader(fileStream);
            else if (file.FileName.EndsWith(".xlsx"))
                excelReader = ExcelReaderFactory.CreateReader(fileStream);
            else
                throw new AppException($"Invalid file '{file.FileName}'");

            dataSet = excelReader.AsDataSet();
            excelReader.Close();

            if (dataSet == null || dataSet.Tables.Count == 0)
                throw new AppException($"Unable to read file. Ensure file complies with the specified template.");

            var table = dataSet.Tables[0];
            var header = table.Rows[0];
            if (!ValidateHeader(header, headers, out string error))
            {
                throw new AppException(error);
            }
            else
            {
                //validate and load data
                var rows = table.Rows;
                for (int i = 1; i < rows.Count; i++)
                {
                    var validationResult = await ValidateDataRow(i, rows[i]);
                    if (!validationResult.isValid)
                    {
                        throw new AppException(validationResult.errorMessage);
                    }
                    else
                    {
                        var user = new User()
                        {
                            FirstName = Convert.ToString(rows[i][1]),
                            MiddleName = Convert.ToString(rows[i][2]),
                            Surname = Convert.ToString(rows[i][3]),
                            Gender = Convert.ToString(rows[i][4]),
                            DateOfBirth = string.IsNullOrEmpty(Convert.ToString(rows[i][5])) ? (DateTimeOffset?)null : DateTimeOffset.Parse(Convert.ToString(rows[i][5])),
                            Email = Convert.ToString(rows[i][6]),
                            PhoneNumber = Convert.ToString(rows[i][7])
                        };
                        users.Add(user);
                    }
                }
            }
            fileStream.Dispose();
            return users;
        }

        public async Task BatchCreateUser(IEnumerable<User> users, long roleId)
        {
            var role = await roleRepo.GetById(roleId);
            if (role == null)
            {
                throw new AppException("Role is required");
            }

            var currentUser = accessor.HttpContext.GetUserSession();
            var password = Constants.DEFAULT_NEW_USER_PASSWORD;

            var _users = new List<User>();

            foreach (var u in users)
            {

                u.Password = passwordService.Hash(password);
                u.IsActive = true;
                u.CreatedBy = currentUser.Username;
                u.UpdatedBy = currentUser.Username;
                u.CreatedDate = DateTimeOffset.Now;
                u.UpdatedDate = DateTimeOffset.Now;
                u.UserRoles = new List<UserRole> { new UserRole { RoleId = roleId, CreatedBy = currentUser.Username } };

                //  validate email and username for duplicate
                if (_users.Any(usr => usr.Email == u.Email))
                {
                    throw new AppException($"A user with email '{u.Email}' already exists on excel");
                }
                if (await userRepo.Any(usr => usr.Email == u.Email))
                {
                    throw new AppException($"A user with email '{u.Email}' already exists");
                }

                u.Username = await GenerateUsername(u.FirstName, u.Surname);
                var ucnt = 1;
                while (_users.Any(usr => usr.Username == u.Username))
                {
                    u.Username = u.Username + ucnt.ToString();
                    ucnt++;
                }
                ucnt = 1;

                _users.Add(u);
            }

            await userRepo.InsertRange(_users);

            _users = _users.Select(u =>
            {
                u.EmailVerificationToken = tokenService.GenerateTokenFromData(u.Id.ToString());
                return u;
            }).ToList();

            await userRepo.UpdateRange(_users);

            //// log action
            //await loggerService.LogActivity(
            //    $"Created new user with email '{user.Email}'",
            //    ActivityType.CREATE_USER,
            //    currentUser.UserId);

            // send welcome/email verification mail

            var recipients = _users.Select(u => new Recipient
            {
                FirstName = u.FirstName,
                LastName = u.Surname,
                Email = u.Email,
                Password = password,
                Token = u.EmailVerificationToken,
                Username = u.Username
            });
            var mail = new MailObject
            {
                Recipients = recipients
            };
            await mailService.ScheduleEmailConfirmationMail(mail);
        }


        public byte[] ExportUsersToExcel(int id)
        {
            var users = userRepo.GetAll();

            // create excel
            var workbook = new XLWorkbook(ClosedXML.Excel.XLEventTracking.Disabled);

            // using data table
            var table = new DataTable("Users");
            foreach (var h in headers)
            {
                table.Columns.Add(h, typeof(string));
            }
            table.Columns.Add("Is Active?", typeof(string));
            table.Columns.Add("Created By", typeof(string));
            table.Columns.Add("Date Created", typeof(string));

            var count = 1;
            foreach (var u in users)
            {
                var row = table.NewRow();

                row[0] = count.ToString();
                row[1] = u.FirstName;
                row[2] = u.MiddleName;
                row[3] = u.Surname;
                row[4] = u.Gender;
                row[5] = u.DateOfBirth;
                row[6] = u.Email;
                row[7] = u.PhoneNumber;
                row[8] = u.IsActive ? "Yes" : "No";
                row[9] = u.CreatedBy;
                row[10] = u.CreatedDate.ToString("yyyy-MM-dd hh:mmtt");

                table.Rows.Add(row);
                count++;
            }
            workbook.AddWorksheet(table);

            byte[] byteFile = null;
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                byteFile = stream.ToArray();
            }

            return byteFile;
        }

        //=========== End Batch Upload =============
    }
}
