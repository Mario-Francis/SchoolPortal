using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolPortal.Data;
using SchoolPortal.Data.Repositories;
using SchoolPortal.Data.Repositories.Implementations;
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
            services.AddDbContext<AppDbContext>(options => options.UseLazyLoadingProxies()
            .UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            //services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            //services.AddScoped<IMappingUploadHistoryRepo, MappingUploadHistoryRepo>();


            // services
            //services.AddTransient(typeof(ILoggerService<>), typeof(LoggerService<>));
            //services.AddScoped(typeof(ILoggerService<>), typeof(LoggerService<>));
            //services.AddScoped<IMyHRService, MyHRService>();
            //services.AddScoped<IUAMService, UAMServiceMock>();
            //services.AddScoped<IEmployeeService, EmployeeService>();
            //services.AddScoped<IReviewerMapperService, ReviewerMapperService>();
            //services.AddScoped<IListService, ListService>();

            //services.AddSingleton<IBackgroundTaskQueue>(ctx => {
            //    if (!int.TryParse(config["TaskQueueCapacity"], out var queueCapacity))
            //        queueCapacity = 50;
            //    return new ReviewerMappingTaskQueue(queueCapacity);
            //});

            //services.AddScoped<IRoleManagerService, RoleManagerService>();
            //services.AddScoped<IUserManagerService, UserManagerService>();

            //services.AddScoped<IEmailService, EmailService>();
            //services.AddScoped<IPasswordService, PasswordService>();
            //services.AddScoped<ITokenService, TokenService>();

            // Background services
            // services.AddHostedService<SyncMyHREmployeesService>();
            //services.AddHostedService<EmployeeStatusUpdateService>(); // not required 
            //services.AddHostedService<ReviewerMappingQueuedService>();

        }
    }
}
