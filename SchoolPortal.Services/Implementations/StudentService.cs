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
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IRepository<Student> studentRepo;
        private readonly IRepository<User> userRepo;
        private readonly IRepository<StudentGuardian> guardianRepo;
        private readonly IRepository<Relationship> relationshipRepo;
        private readonly IRepository<Class> classRepo;
        private readonly IRepository<ClassRoom> classRoomRepo;
        private readonly IRepository<ClassRoomStudent> classRoomStudentRepo;
        private readonly IRepository<Term> termRepo;
        private readonly IEmailService emailService;
        private readonly IPasswordService passwordService;
        private readonly ILogger<StudentService> logger;
        private readonly IHttpContextAccessor accessor;
        private readonly ITokenService tokenService;
        private readonly IMailService mailService;
        private readonly AppSettings appSettings;
        private string[] headers = new string[] { "SN", "First Name", "Middle Name", "Surname", "Gender", "Date of Birth", "Email", "Phone Number", "Admission No", "Entry Class", "Entry Term", "Entry Session", "Enrollment Date", "Current Class", "Current Room Code" };

        public StudentService(IRepository<Student> studentRepo,
            IRepository<User> userRepo,
             IRepository<StudentGuardian> guardianRepo,
            IRepository<Relationship> relationshipRepo,
            IRepository<Class> classRepo,
            IRepository<ClassRoom> classRoomRepo,
             IRepository<ClassRoomStudent> classRoomStudentRepo,
            IRepository<Term> termRepo,
            IEmailService emailService,
            IPasswordService passwordService,
            ILogger<StudentService> logger,
            IHttpContextAccessor accessor,
            ITokenService tokenService,
            IMailService mailService,
            IOptions<AppSettings> appSettings)
        {
            this.studentRepo = studentRepo;
            this.userRepo = userRepo;
            this.guardianRepo = guardianRepo;
            this.relationshipRepo = relationshipRepo;
            this.classRepo = classRepo;
            this.classRoomRepo = classRoomRepo;
            this.classRoomStudentRepo = classRoomStudentRepo;
            this.termRepo = termRepo;
            this.emailService = emailService;
            this.passwordService = passwordService;
            this.logger = logger;
            this.accessor = accessor;
            this.tokenService = tokenService;
            this.mailService = mailService;
            this.appSettings = appSettings.Value;
        }

        // create student
        public async Task<long> CreateStudent(Student student)
        {
            if (student == null)
            {
                throw new AppException("Student object is required");
            }
            if (!await emailService.IsEmailValidAsync(student.Email))
            {
                throw new AppException($"Email '{student.Email}' is not valid");
            }
            if (await studentRepo.Any(u => u.Email == student.Email))
            {
                throw new AppException($"A student with email '{student.Email}' already exist");
            }
            if (await studentRepo.Any(u => u.AdmissionNo == student.AdmissionNo))
            {
                throw new AppException($"A student with admission number '{student.AdmissionNo}' already exist");
            }

            if (!AppUtilities.ValidateSession(student.EntrySession))
            {
                throw new AppException($"Entry session '{student.EntrySession}' is invalid");
            }

            if (student.ClassRoomStudents == null || student.ClassRoomStudents.Count() == 0)
            {
                var _class = await classRepo.GetById(student.EntryClassId);
                student.ClassRoomStudents = new List<ClassRoomStudent> {
                    new ClassRoomStudent {
                        ClassRoomId=_class.ClassRooms.FirstOrDefault().Id
                    }
                };
            }
            var currentUser = accessor.HttpContext.GetUserSession();
            student.ClassRoomStudents = student.ClassRoomStudents.Select(x =>
            {
                x.CreatedBy = currentUser.Username;
                x.UpdatedBy = currentUser.Username;

                return x;
            }).ToList();

           
            student.Username = await GenerateUsername(student.FirstName, student.Surname);
            student.Password = passwordService.Hash(Constants.DEFAULT_NEW_USER_PASSWORD);
            student.IsActive = true;
            student.CreatedBy = currentUser.Username;
            student.UpdatedBy = currentUser.Username;
            student.UpdatedByType = currentUser.UserType;


            await studentRepo.Insert(student, true);

            var token = tokenService.GenerateTokenFromData(student.Id.ToString());
            student.EmailVerificationToken = token;
            await studentRepo.Update(student, true);

            //// log action
            //await loggerService.LogActivity(
            //    $"Created new student with email '{student.Email}'",
            //    ActivityType.CREATE_USER,
            //    currentUser.StudentId);

            // send welcome/email verification mail

            var mail = new MailObject
            {
                Recipients = new List<Recipient> {
                    new Recipient {
                        FirstName=student.FirstName,
                        LastName=student.Surname,
                        Email=student.Email,
                        Username=student.Username,
                        Password= Constants.DEFAULT_NEW_USER_PASSWORD,
                        Token=token
                    }
                }
            };
            await mailService.ScheduleEmailConfirmationMail(mail);

            return student.Id;
        }

        // update student
        public async Task UpdateStudent(Student student)
        {
            if (student == null)
            {
                throw new AppException("Student object is required");
            }
            var _student = await studentRepo.GetById(student.Id);
            if (student == null)
            {
                throw new AppException($"Student with id '{student.Id}' does not exist");
            }
            if (!await emailService.IsEmailValidAsync(student.Email))
            {
                throw new AppException($"Email '{student.Email}' is not valid");
            }
            if (await studentRepo.Any(u => u.Email == student.Email) && student.Email != _student.Email)
            {
                throw new AppException($"A student with email '{student.Email}' already exist");
            }
            if (await studentRepo.Any(u => u.AdmissionNo == student.AdmissionNo) && student.AdmissionNo != _student.AdmissionNo)
            {
                throw new AppException($"A student with admission number '{student.AdmissionNo}' already exist");
            }

            if (!AppUtilities.ValidateSession(student.EntrySession))
            {
                throw new AppException($"Entry session '{student.EntrySession}' is invalid");
            }

            var currentUser = accessor.HttpContext.GetUserSession();

            var oldStudent = _student.Clone<Student>();

            _student.FirstName = student.FirstName;
            _student.MiddleName = student.MiddleName;
            _student.Surname = student.Surname;
            _student.PhoneNumber = student.PhoneNumber;
            _student.Email = student.Email;
            _student.Gender = student.Gender;
            _student.DateOfBirth = student.DateOfBirth;
            _student.AdmissionNo = student.AdmissionNo;
            _student.EnrollmentDate = student.EnrollmentDate;
            _student.EntryClassId = student.EntryClassId;
            _student.EntrySession = student.EntrySession;
            _student.EntryTermId = student.EntryTermId;

            _student.UpdatedBy = currentUser.Username;
            _student.UpdatedDate = DateTimeOffset.Now;
            _student.UpdatedByType = currentUser.UserType;

            await classRoomStudentRepo.DeleteRange(_student.ClassRoomStudents.Select(cs => cs.Id), true);
            _student.ClassRoomStudents.Clear();
            _student.ClassRoomStudents.AddRange(student.ClassRoomStudents.Select(cs =>
            {
                cs.StudentId = student.Id;
                cs.CreatedBy = currentUser.Username;
                cs.UpdatedBy = currentUser.Username;
                return cs;
            }).ToList());

            await studentRepo.Update(_student, true);
            // log action
            //await loggerService.LogActivity(
            //    $"Updated student",
            //    ActivityType.UPDATE_USER,
            //    currentUser.StudentId, studentRepository.TableName, oldStudent, student);
        }

        // update password
        public async Task UpdatePassword(PasswordRequestObject req)
        {
            if (req == null)
            {
                throw new AppException("Password object is required");
            }
            var student = await studentRepo.GetById(req.UserId);
            if (student == null)
            {
                throw new AppException($"Student with id '{req.UserId}' does not exist");
            }
            if (!passwordService.Verify(req.Password, student.Password))
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
            student.Password = passwordService.Hash(req.NewPassword);
            student.UpdatedBy = currentUser.Username;
            student.UpdatedDate = DateTimeOffset.Now;
            student.UpdatedByType = currentUser.UserType;

            await studentRepo.Update(student, true);

            // log action
            //await loggerService.LogActivity(
            //    $"Updated password",
            //    ActivityType.UPDATE_PASSWORD,
            //    currentUser.StudentId);
        }

        // reset password
        public async Task ResetPassword(long studentId)
        {
            var student = await studentRepo.GetById(studentId);
            if (student == null)
            {
                throw new AppException($"Student with id '{studentId}' does not exist");
            }
            var newPassword = Constants.DEFAULT_NEW_USER_PASSWORD;
            var currentUser = accessor.HttpContext.GetUserSession();
            student.Password = passwordService.Hash(newPassword);
            student.UpdatedBy = currentUser.Username;
            student.UpdatedDate = DateTimeOffset.Now;
            student.UpdatedByType = currentUser.UserType;

            await studentRepo.Update(student, true);

            // log action
            //await loggerService.LogActivity(
            //    $"Updated password",
            //    ActivityType.UPDATE_PASSWORD,
            //    currentUser.StudentId);

            var mail = new MailObject
            {
                Recipients = new List<Recipient> {
                    new Recipient {
                        FirstName=student.FirstName,
                        LastName=student.Surname,
                        Email=student.Email,
                        Password= newPassword
                    }
                }
            };
            await mailService.SchedulePasswordResetMail(mail);

        }


        // authenticate student
        public async Task<bool> IsStudentAuthentic(LoginCredential credential)
        {
            if (string.IsNullOrEmpty(credential?.Email))
            {
                throw new AppException($"Email/Username is required");
            }
            if (string.IsNullOrEmpty(credential?.Password))
            {
                throw new AppException($"Password is required");
            }
            var student = await studentRepo.GetSingleWhere(u => u.Email == credential.Email || u.Username == credential.Email);
            if (student == null)
            {
                throw new AppException($"Email/username is invalid");
            }
            if (!passwordService.Verify(credential.Password, student.Password))
            {
                throw new AppException($"Password is invalid");
            }
            return true;
        }

        // get student
        public async Task<Student> GetStudent(long studentId)
        {
            var student = await studentRepo.GetById(studentId);
            if (student == null)
                throw new AppException($"Student with id: '{studentId}' does not exist");
            else
                return student;
        }

        public async Task<Student> GetStudent(string email)
        {
            var student = await studentRepo.GetSingleWhere(u => u.Email == email || u.Username == email);
            if (student == null)
                throw new AppException($"Student with email or username: '{email}' does not exist");
            else
                return student;
        }

        // get students
        public IEnumerable<Student> GetStudents(bool includeInactive = true)
        {
            var students = studentRepo.GetAll();
            if (!includeInactive)
            {
                students = students.Where(u => u.IsActive);
            }
            return students;
        }

#warning send password recovery mail

        // delete student
        public async Task DeleteStudent(long studentId)
        {
            try
            {
                var student = await studentRepo.GetById(studentId);
                if (student == null)
                {
                    throw new AppException($"No student with id '{studentId}' exist");
                }

                if (/*student.ClassRoomStudents.Count > 0 || */student.StudentGuardians.Count > 0
                    || student.StudentLoginHistories.Count > 0 || student.MidTermResults.Count > 0
                    || student.EndTermResults.Count > 0 || student.StudentAttendanceRecords.Count > 0)
                {
                    throw new AppException($"Student cannot be deleted as student is still associated with one or more entities");
                }

                await classRoomStudentRepo.DeleteRange(student.ClassRoomStudents.Select(cs => cs.Id), false);
                await studentRepo.Delete(student.Id, true);

                var currentUser = accessor.HttpContext.GetUserSession();
                //// log action
                //await loggerService.LogActivity(
                //    $"Deleted student with email '{student.Email}'",
                //    ActivityType.DELETE_USER,
                //    currentUser.StudentId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw new AppException($"Student cannot be deleted as student is still associated with one or more entities");
            }
        }

        // generate username
        public async Task<string> GenerateUsername(string firstName, string lastName)
        {
            var uname = $"{firstName.ToLower()}.{lastName.ToLower()}";
            var cnt = 1;
            while (await studentRepo.Any(u => u.Username == uname))
            {
                uname += cnt.ToString();
                cnt++;
            }

            return uname;
        }

        public async Task<bool> AnyStudentExists()
        {
            return (await studentRepo.Count()) > 0;
        }

        public async Task UpdateStudentStatus(long studentId, bool isActive)
        {
            var _student = await studentRepo.GetById(studentId);
            if (_student == null)
            {
                throw new AppException($"Invalid student id {studentId}");
            }
            else
            {
                var currentUser = accessor.HttpContext.GetUserSession();

                var _oldstudent = _student.Clone<Student>();


                _student.IsActive = isActive;
                _student.UpdatedBy = currentUser.Username;
                _student.UpdatedDate = DateTimeOffset.Now;
                _student.UpdatedByType = currentUser.UserType;

                await studentRepo.Update(_student, true);


                // log activity
                //await loggerService.LogActivity(ActivityActionType.UPDATED_class, currentUser.PersonNumber,
                //    classRepo.TableName, _oldclass, _class,
                //     $"Updated class of type '{((Core.classType)((int)_class.classTypeId)).ToString()}' for {_class.FromDate.ToString("dd-MM-yyyy")} to {_class.ToDate.ToString("dd-MM-yyyy")}");

            }
        }

        public async Task UpdateStudentGraduationStatus(long studentId, bool isGraduated)
        {
            var _student = await studentRepo.GetById(studentId);
            if (_student == null)
            {
                throw new AppException($"Invalid student id {studentId}");
            }
            else
            {
                var currentUser = accessor.HttpContext.GetUserSession();

                var _oldstudent = _student.Clone<Student>();

                _student.IsGraduated = isGraduated;
                _student.UpdatedBy = currentUser.Username;
                _student.UpdatedDate = DateTimeOffset.Now;
                _student.UpdatedByType = currentUser.UserType;

                await studentRepo.Update(_student, true);


                // log activity
                //await loggerService.LogActivity(ActivityActionType.UPDATED_class, currentUser.PersonNumber,
                //    classRepo.TableName, _oldclass, _class,
                //     $"Updated class of type '{((Core.classType)((int)_class.classTypeId)).ToString()}' for {_class.FromDate.ToString("dd-MM-yyyy")} to {_class.ToDate.ToString("dd-MM-yyyy")}");

            }
        }

        public async Task AddStudentGuardian(long studentId, long parentId, long relationshipId)
        {
            var student = await studentRepo.GetById(studentId);
            if(student == null)
            {
                throw new AppException($"Student is not found");
            }
            var parent = await userRepo.GetById(parentId);
            if (parent == null)
            {
                throw new AppException($"Guardian is not found");
            }
            var relationship = await relationshipRepo.GetById(relationshipId);
            if (relationship == null)
            {
                throw new AppException($"Relationship is not found");
            }
            if(await guardianRepo.Any(g=>g.StudentId==studentId && g.GuardianId == parentId))
            {
                throw new AppException($"Guardian already exist");
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

        public async Task RemoveStudentGuardian(long studentGuardianId)
        {
            var studentGuardian = await guardianRepo.GetById(studentGuardianId);
            if (studentGuardian == null)
            {
                throw new AppException($"Student guardian is not found");
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
            if (row[1] == null || Convert.ToString(row[1]).Trim() == "") // first name
            {
                isValid = false;
                err = $"Invalid value for {headers[1]} at row {index}. Field is required.";
            }
            else if (row[3] == null || Convert.ToString(row[3]).Trim() == "") // surname
            {
                isValid = false;
                err = $"Invalid value for {headers[3]} at row {index}. Field is required.";
            }
            else if (row[4] == null || Convert.ToString(row[4]).Trim() == "") // gender
            {
                isValid = false;
                err = $"Invalid value for {headers[4]} at row {index}. Field is required.";
            }
            else if (!(new string[] { "male", "female" }).Contains(Convert.ToString(row[4]).Trim().ToLower())) // gender
            {
                isValid = false;
                err = $"Invalid value for {headers[4]} at row {index}. Value can either be 'Male' or 'Female'.";
            }
            else if (row[5] == null || Convert.ToString(row[5]).Trim() == "") // date of birth
            {
                isValid = false;
                err = $"Invalid value for {headers[5]} at row {index}. Field is required.";
            }
            else if (!string.IsNullOrEmpty(Convert.ToString(row[5]).Trim()) && !DateTimeOffset.TryParse(Convert.ToString(row[5]).Trim(), out DateTimeOffset _)) // date  of birth
            {
                isValid = false;
                err = $"Invalid value for {headers[5]} at row {index}. A valid date value expected.";
            }
            else if (row[6] == null || Convert.ToString(row[6]).Trim() == "") // email
            {
                isValid = false;
                err = $"Invalid value for {headers[6]} at row {index}. Field is required.";
            }
            else if (!string.IsNullOrEmpty(Convert.ToString(row[6]).Trim()) && !(await emailService.IsEmailValidAsync(Convert.ToString(row[6]).Trim(), false))) // email
            {
                isValid = false;
                err = $"Invalid value for {headers[6]} at row {index}. Field is required.";
            }
            else if (row[7] == null || Convert.ToString(row[7]).Trim() == "") // phone number
            {
                isValid = false;
                err = $"Invalid value for {headers[7]} at row {index}. Field is required.";
            }
            else if (row[8] == null || Convert.ToString(row[8]).Trim() == "") // admission no
            {
                isValid = false;
                err = $"Invalid value for {headers[8]} at row {index}. Field is required.";
            }
            else if (row[9] == null || Convert.ToString(row[9]).Trim() == "") // entry class
            {
                isValid = false;
                err = $"Invalid value for {headers[9]} at row {index}. Field is required.";
            }
            else if (!(await ValidateClass(Convert.ToString(row[9]).Trim()))) // entry class
            {
                isValid = false;
                err = $"Invalid value for {headers[9]} at row {index}. Class does not exist.";
            }
            else if (row[10] == null || Convert.ToString(row[10]).Trim() == "") // entry term
            {
                isValid = false;
                err = $"Invalid value for {headers[10]} at row {index}. Field is required.";
            }
            else if (!(new string[] { "first", "second", "third" }).Contains(Convert.ToString(row[10]).Trim().ToLower())) // entry term
            {
                isValid = false;
                err = $"Invalid value for {headers[10]} at row {index}. Term does not exist.";
            }
            else if (row[11] == null || Convert.ToString(row[11]).Trim() == "") // entry session
            {
                isValid = false;
                err = $"Invalid value for {headers[11]} at row {index}. Field is required.";
            }
            else if (!AppUtilities.ValidateSession(Convert.ToString(row[11]).Trim())) // entry session
            {
                isValid = false;
                err = $"Invalid value for {headers[11]} at row {index}. Session is invalid.";
            }
            else if (row[12] == null || Convert.ToString(row[12]).Trim() == "") // enrollment  date
            {
                isValid = false;
                err = $"Invalid value for {headers[12]} at row {index}. Field is required.";
            }
            else if (!string.IsNullOrEmpty(Convert.ToString(row[12]).Trim()) && !DateTimeOffset.TryParse(Convert.ToString(row[12]).Trim(), out DateTimeOffset _)) // ennrollment date
            {
                isValid = false;
                err = $"Invalid value for {headers[12]} at row {index}. A valid date value expected.";
            }
            else if (!string.IsNullOrEmpty(Convert.ToString(row[13]).Trim()) && !(await ValidateClass(Convert.ToString(row[13]).Trim()))) // current class
            {
                isValid = false;
                err = $"Invalid value for {headers[13]} at row {index}. Class does not exist.";
            }
            else if (!string.IsNullOrEmpty(Convert.ToString(row[14]).Trim()) && !(await ValidateClassRoom(Convert.ToString(row[13]).Trim(), Convert.ToString(row[14]).Trim()))) // current classroom
            {
                isValid = false;
                err = $"Invalid value for {headers[14]} at row {index}. Classroom does not exist.";
            }

            return (isValid, err);
        }

        private async Task<bool> ValidateClass(string @class)
        {
            var isValid = true;
            @class = @class.Trim();
            var arr = @class.Split(' ');
            if (arr.Length != 2 || string.IsNullOrEmpty(arr[0]) || string.IsNullOrEmpty(arr[1]))
            {
                isValid = false;
            }

            if (!(await classRepo.Any(c => c.ClassType.Name.ToLower() == arr[0].ToLower() && c.ClassGrade.ToString() == arr[1])))
            {
                isValid = false;
            }

            return isValid;
        }
        private async Task<long> GetClassId(string @class)
        {
            @class = @class.Trim();
            var arr = @class.Split(' ');

            var _class = await classRepo.GetSingleWhere(c => c.ClassType.Name.ToLower() == arr[0].ToLower() && c.ClassGrade.ToString() == arr[1]);

            return _class.Id;
        }

        private async Task<bool> ValidateClassRoom(string @class, string roomCode)
        {
            var isValid = true;
            @class = @class.Trim();
            var arr = @class.Split(' ');
            var _class = (await classRepo.GetSingleWhere(c => c.ClassType.Name.ToLower() == arr[0].ToLower() && c.ClassGrade.ToString() == arr[1]));
            if (!_class.ClassRooms.Any(r => r.RoomCode.ToLower() == roomCode.ToLower()))
            {
                isValid = false;
            }

            return isValid;
        }
        private async Task<long> GetClassRoomId(string @class, string roomCode)
        {
            @class = @class.Trim();
            var arr = @class.Split(' ');
            var classRoom = await classRoomRepo.GetSingleWhere(r => r.Class.ClassType.Name.ToLower() == arr[0].ToLower() && r.Class.ClassGrade.ToString() == arr[1] && r.RoomCode.ToLower() == roomCode.ToLower());

            return classRoom.Id;
        }

        public async Task<IEnumerable<Student>> ExtractData(IFormFile file)
        {
            List<Student> students = new List<Student>();
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
                        var student = new Student()
                        {
                            FirstName = Convert.ToString(rows[i][1]),
                            MiddleName = Convert.ToString(rows[i][2]),
                            Surname = Convert.ToString(rows[i][3]),
                            Gender = Convert.ToString(rows[i][4]),
                            DateOfBirth = DateTimeOffset.Parse(Convert.ToString(rows[i][5])),
                            Email = Convert.ToString(rows[i][6]),
                            PhoneNumber = Convert.ToString(rows[i][7]),
                            AdmissionNo = Convert.ToString(rows[i][8]),
                            EntryClassId = await GetClassId(Convert.ToString(rows[i][9])),
                            EntryTermId = (await termRepo.GetSingleWhere(t => t.Name.ToLower() == Convert.ToString(rows[i][10]).ToLower())).Id,
                            EntrySession = Convert.ToString(rows[i][11]),
                            EnrollmentDate = DateTimeOffset.Parse(Convert.ToString(rows[i][12]))
                        };

                        if (!string.IsNullOrEmpty(Convert.ToString(rows[i][13])))
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(rows[i][14])))
                            {
                                student.ClassRoomStudents = new List<ClassRoomStudent>
                                {
                                    new ClassRoomStudent{ClassRoomId = await GetClassRoomId(Convert.ToString(rows[i][13]), Convert.ToString(rows[i][14]))}
                                };
                            }
                            else
                            {
                                var _class = await classRepo.GetById(await GetClassId(Convert.ToString(rows[i][13])));
                                student.ClassRoomStudents = new List<ClassRoomStudent>
                                {
                                    new ClassRoomStudent{ClassRoomId = _class.ClassRooms.FirstOrDefault().Id }
                                };
                            }
                        }
                        else
                        {
                            var _class = await classRepo.GetById(student.EntryClassId);
                            student.ClassRoomStudents = new List<ClassRoomStudent>
                                {
                                    new ClassRoomStudent{ClassRoomId = _class.ClassRooms.FirstOrDefault().Id }
                                };
                        }
                        students.Add(student);
                    }
                }
            }
            fileStream.Dispose();
            return students;
        }

        public async Task BatchCreateStudent(IEnumerable<Student> students)
        {
            var currentUser = accessor.HttpContext.GetUserSession();
            var password = Constants.DEFAULT_NEW_USER_PASSWORD;

            var _students = new List<Student>();

            foreach (var u in students)
            {
                u.Username = await GenerateUsername(u.FirstName, u.Surname);
                u.Password = passwordService.Hash(password);
                u.IsActive = true;
                u.CreatedBy = currentUser.Username;
                u.UpdatedBy = currentUser.Username;
                u.CreatedDate = DateTimeOffset.Now;
                u.UpdatedDate = DateTimeOffset.Now;
                u.UpdatedByType = currentUser.UserType;

                //  validate email and username for duplicate
                if (_students.Any(usr => usr.Email == u.Email))
                {
                    throw new AppException($"A student with email '{u.Email}' already exists on excel");
                }
                if (_students.Any(usr => usr.AdmissionNo == u.AdmissionNo))
                {
                    throw new AppException($"A student with admission number '{u.AdmissionNo}' already exists on excel");
                }
                if (await studentRepo.Any(usr => usr.Email == u.Email))
                {
                    throw new AppException($"A student with email '{u.Email}' already exists");
                }
                if (await studentRepo.Any(usr => usr.AdmissionNo == u.AdmissionNo))
                {
                    throw new AppException($"A student with admission number '{u.AdmissionNo}' already exist");
                }

                u.Username = await GenerateUsername(u.FirstName, u.Surname);
                var ucnt = 1;
                while (_students.Any(usr => usr.Username == u.Username))
                {
                    u.Username = u.Username + ucnt.ToString();
                    ucnt++;
                }
                ucnt = 1;


                _students.Add(u);
            }

            await studentRepo.InsertRange(_students);

            _students = _students.Select(u =>
            {
                u.EmailVerificationToken = tokenService.GenerateTokenFromData(u.Id.ToString());
                return u;
            }).ToList();

            await studentRepo.UpdateRange(_students);

            //// log action
            //await loggerService.LogActivity(
            //    $"Created new student with email '{student.Email}'",
            //    ActivityType.CREATE_USER,
            //    currentUser.StudentId);

            // send welcome/email verification mail

            var recipients = _students.Select(u => new Recipient
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


        public byte[] ExportStudentsToExcel(int id)
        {
            var students = studentRepo.GetAll();

            // create excel
            var workbook = new XLWorkbook(ClosedXML.Excel.XLEventTracking.Disabled);

            // using data table
            var table = new DataTable("Students");
            foreach (var h in headers)
            {
                table.Columns.Add(h, typeof(string));
            }
            table.Columns.Add("Is Active?", typeof(string));
            table.Columns.Add("Created By", typeof(string));
            table.Columns.Add("Date Created", typeof(string));

            var count = 1;
            foreach (var u in students)
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
                row[8] = u.AdmissionNo;
                row[9] = $"{u.EntryClass.ClassType.Name} {u.EntryClass.ClassGrade}";
                row[10] = u.EntryTerm.Name;
                row[11] = u.EntrySession;
                row[12] = u.EnrollmentDate.ToString("yyyy-MM-dd");
                var room = u.ClassRoomStudents.FirstOrDefault().ClassRoom;
                row[13] = $"{room.Class.ClassType.Name} {room.Class.ClassGrade}";
                row[14] = room.RoomCode;
                row[15] = u.IsActive ? "Yes" : "No";
                row[16] = u.CreatedBy;
                row[17] = u.CreatedDate.ToString("yyyy-MM-dd hh:mmtt");

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

        public IEnumerable<Student> SearchStudents(string searchParam, int max = 50)
        {
            if (string.IsNullOrEmpty(searchParam))
            {
                return studentRepo.GetWhere(s => s.IsActive).Take(max);
            }
            else
            {
                searchParam = searchParam.ToLower();
                var students = studentRepo.GetWhere(u => u.IsActive && !u.IsGraduated &&
                (u.Username.ToLower().Contains(searchParam) || u.Email.ToLower().Contains(searchParam)
                || u.PhoneNumber.ToLower().Contains(searchParam) || u.FirstName.ToLower().Contains(searchParam)
                || u.MiddleName.ToLower().Contains(searchParam) || u.Surname.ToLower().Contains(searchParam) 
                || u.AdmissionNo.ToLower().Contains(searchParam))).Take(max);


                return students;
            }
        }
    }
}
