using Microsoft.EntityFrameworkCore;
using SchoolPortal.Core.Models;
using System;

namespace SchoolPortal.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<BehaviouralRating> BehaviouralRatings { get; set; }
        public DbSet<BehaviouralResult> BehaviouralResults { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassRoom> ClassRooms { get; set; }
        public DbSet<ClassRoomStudent> ClassRoomStudents { get; set; }
        public DbSet<ClassType> ClassTypes { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<EndTermResult> EndTermResults { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }
        public DbSet<Mail> Mails { get; set; }
        public DbSet<MidTermResult> MidTermResults { get; set; }
        public DbSet<PerformanceRemark> PerformanceRemarks { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SessionTermLog> SessionTermLogs { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentAttendanceRecord> StudentAttendanceRecords { get; set; }
        public DbSet<StudentGuardian> StudentGuardians { get; set; }
        public DbSet<StudentLoginHistory> StudentLoginHistories { get; set; }
        public DbSet<Term> Terms { get; set; }
        public DbSet<TermSection> TermSections { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserLoginHistory> UserLoginHistories { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Username).IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(x => x.UserRoles)
                .WithOne(x => x.User);

            modelBuilder.Entity<Role>()
                .HasMany(x => x.UserRoles)
                .WithOne(x => x.Role);

            modelBuilder.Entity<User>()
                .HasMany(x => x.ClassRooms)
                .WithOne(x => x.Teacher);

            modelBuilder.Entity<User>()
                .HasMany(x => x.UserLoginHistories)
                .WithOne(x => x.User);

            modelBuilder.Entity<User>()
                .HasMany(x => x.StudentGuardians)
                .WithOne(x => x.Guardian);

            modelBuilder.Entity<Student>()
                .HasMany(x => x.MidTermResults)
                .WithOne(x => x.Student)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Student>()
                .HasMany(x => x.EndTermResults)
                .WithOne(x => x.Student)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Student>()
                .HasMany(x => x.StudentAttendanceRecords)
                .WithOne(x => x.Student)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Student>()
                .HasMany(x => x.StudentLoginHistories)
                .WithOne(x => x.Student)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Student>()
               .HasMany(x => x.StudentGuardians)
               .WithOne(x => x.Student);

            modelBuilder.Entity<Student>()
              .HasMany(x => x.ClassRoomStudents)
              .WithOne(x => x.Student)
              .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Class>()
               .HasMany(x => x.ClassRooms)
               .WithOne(x => x.Class);

            modelBuilder.Entity<Class>()
              .HasMany(x => x.Courses)
              .WithOne(x => x.Class);

            modelBuilder.Entity<ClassRoom>()
              .HasMany(x => x.ClassRoomStudents)
              .WithOne(x => x.ClassRoom)
              .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ClassRoomStudent>()
              .HasOne(x => x.Student)
              .WithMany(x => x.ClassRoomStudents)
              .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HealthRecord>()
              .HasOne(x => x.Term)
              .WithMany()
              .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PerformanceRemark>()
              .HasOne(x => x.Student)
              .WithMany()
              .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BehaviouralResult>()
              .HasOne(x => x.Student)
              .WithMany()
              .OnDelete(DeleteBehavior.NoAction);

            SeeData(modelBuilder);
        }

        private void SeeData(ModelBuilder modelBuilder)
        {
           // modelBuilder.Entity<CadreLevel>().HasData(
           //     new CadreLevel
           //     {
           //         Id = 1,
           //         Name = "EXECUTIVES",
           //         CreatedBy = Constants.SYSTEM_NAME,
           //         CreatedDate = DateTimeOffset.Now,
           //         UpdatedBy = Constants.SYSTEM_NAME,
           //         UpdatedDate = DateTimeOffset.Now
           //     },
           //     new CadreLevel
           //     {
           //         Id = 2,
           //         Name = "SENIOR MANAGEMENT",
           //         CreatedBy = Constants.SYSTEM_NAME,
           //         CreatedDate = DateTimeOffset.Now,
           //         UpdatedBy = Constants.SYSTEM_NAME,
           //         UpdatedDate = DateTimeOffset.Now
           //     },
           //     new CadreLevel
           //     {
           //         Id = 3,
           //         Name = "MANAGEMENT",
           //         CreatedBy = Constants.SYSTEM_NAME,
           //         CreatedDate = DateTimeOffset.Now,
           //         UpdatedBy = Constants.SYSTEM_NAME,
           //         UpdatedDate = DateTimeOffset.Now
           //     },
           //     new CadreLevel
           //     {
           //         Id = 4,
           //         Name = "OFFICERS",
           //         CreatedBy = Constants.SYSTEM_NAME,
           //         CreatedDate = DateTimeOffset.Now,
           //         UpdatedBy = Constants.SYSTEM_NAME,
           //         UpdatedDate = DateTimeOffset.Now
           //     }
           //);

            
        }
    }
}
