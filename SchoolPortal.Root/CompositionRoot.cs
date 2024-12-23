﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchoolPortal.Data;
using SchoolPortal.Data.Repositories;
using SchoolPortal.Data.Repositories.Implementations;
using SchoolPortal.Services;
using SchoolPortal.Services.BackgroundServices;
using SchoolPortal.Services.Implementations;
using System;

namespace SchoolPortal.Root
{
    public class CompositionRoot
    {
        public CompositionRoot() { }

        public static void InjectDependencies(IServiceCollection services, IConfiguration config)
        {
            // inject dependencies here
            // db context 
            services.AddDbContext<AppDbContext>(options => 
                options.UseLazyLoadingProxies()
                .UseSqlServer(config.GetConnectionString("DefaultConnection"))
               // .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
            );

            // repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            //services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            //services.AddScoped<IMappingUploadHistoryRepo, MappingUploadHistoryRepo>();


            // services
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IListService, ListService>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<ISubjectService, SubjectService>();  
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped(typeof(ILoggerService<>), typeof(LoggerService<>));
            services.AddScoped<IStudentResultService, StudentResultService>();
            services.AddScoped<IBehaviouralRatingService, BehaviouralRatingService>();
            services.AddScoped<IPerformanceRemarkService, PerformanceRemarkService>();
            services.AddScoped<IHealthRecordService, HealthRecordService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<ICourseWorkService, CourseWorkService>();
            services.AddScoped<IGradeService, GradeService>();
            services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();
            services.AddScoped<IAttendanceRecordService, AttendanceRecordService>();



            services.AddSingleton<IBackgroundTaskQueue>(ctx =>
            {
                if (!int.TryParse(config["TaskQueueCapacity"], out var queueCapacity))
                    queueCapacity = 50;
                return new TaskQueue(queueCapacity);
            });


            // Background services
            services.AddHostedService<QueuedService>();
            services.AddHostedService<NotificationService>();
            services.AddHostedService<ReminderService>();

        }
    }
}
