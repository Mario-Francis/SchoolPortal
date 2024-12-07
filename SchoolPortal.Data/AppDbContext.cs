using Audit.Core;
using Audit.EntityFramework;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Core;
using SchoolPortal.Core.Models;
using SchoolPortal.Core.Models.Audit;
using SchoolPortal.Core.Models.Views;
using System;

namespace SchoolPortal.Data
{
    public class AppDbContext : DbContext
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
        public DbSet<Subject> Subjects { get; set; }
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
        public DbSet<ClassRoomTeacher> ClassRoomTeachers { get; set; }
        public DbSet<RoomCode> RoomCodes { get; set; }
        public DbSet<ExamType> ExamTypes { get; set; }
        public DbSet<CourseWork> CourseWorks { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }

        // Audit Table
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<AuditLogChange> AuditLogChanges { get; set; }

        // ========== views =========
        public DbSet<MidTermResultViewObject> MidTermResultViewObjects { get; set; }
        public DbSet<EndTermResultViewObject> EndTermResultViewObjects { get; set; }
        public DbSet<EndOfSessionResultViewObject> EndOfSessionResultViewObjects { get; set; }
        public DbSet<EndOfSecondTermResultViewObject> EndOfSecondTermResultViewObjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Username).IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email).IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(x => x.UserRoles)
                .WithOne(x => x.User);

            modelBuilder.Entity<Role>()
                .HasMany(x => x.UserRoles)
                .WithOne(x => x.Role);

            modelBuilder.Entity<User>()
                .HasMany(x => x.ClassRoomTeachers)
                .WithOne(x => x.Teacher)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(x => x.UserLoginHistories)
                .WithOne(x => x.User);

            modelBuilder.Entity<User>()
                .HasMany(x => x.StudentGuardians)
                .WithOne(x => x.Guardian);

            modelBuilder.Entity<Student>()
               .HasIndex(x => x.Username).IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(x => x.AdmissionNo).IsUnique();

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
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Class>()
               .HasMany(x => x.ClassRooms)
               .WithOne(x => x.Class);

            modelBuilder.Entity<Class>()
              .HasMany(x => x.Subjects)
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

            modelBuilder.Entity<Grade>()
              .HasOne(x => x.TermSection)
              .WithMany()
              .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ClassRoomTeacher>()
              .HasOne(x => x.ClassRoom)
              .WithMany(x=> x.ClassRoomTeachers)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClassRoomTeacher>()
             .HasOne(x => x.Teacher)
             .WithMany(x => x.ClassRoomTeachers)
             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subject>()
             .HasMany(x => x.MidTermResults)
             .WithOne(x => x.Subject)
             .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Subject>()
             .HasMany(x => x.EndTermResults)
             .WithOne(x => x.Subject)
             .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Exam>()
            .HasOne(x => x.ExamType)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Exam>()
            .HasOne(x => x.Term)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MidTermResult>()
            .HasOne(x => x.ClassRoom)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MidTermResult>()
            .HasOne(x => x.Class)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EndTermResult>()
            .HasOne(x => x.ClassRoom)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EndTermResult>()
            .HasOne(x => x.Class)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AttendanceRecord>()
             .HasOne(x => x.Term)
             .WithMany()
             .OnDelete(DeleteBehavior.NoAction);

            // view
            modelBuilder.Entity<MidTermResultViewObject>(
            x =>
            {
                x.HasNoKey();
                x.ToView("MidTermResults_View");
            });

            modelBuilder.Entity<EndTermResultViewObject>(
            x =>
            {
                x.HasNoKey();
                x.ToView("EndTermResults_View");
            });

            modelBuilder.Entity<EndOfSessionResultViewObject>(
           x =>
           {
               x.HasNoKey();
               x.ToView("EndOfSessionResults_View");
           });

            modelBuilder.Entity<EndOfSecondTermResultViewObject>(
           x =>
           {
               x.HasNoKey();
               x.ToView("EndOfSecondTermResults_View");
           });

            SeeData(modelBuilder);
        }

        private void SeeData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    Name = "Administrator",
                    CreatedBy = Constants.SYSTEM_NAME,
                    CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
                },
                new Role
                {
                    Id = 2,
                    Name = "Head Teacher",
                    CreatedBy = Constants.SYSTEM_NAME,
                    CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
                },
                new Role
                {
                    Id = 3,
                    Name = "Teacher",
                    CreatedBy = Constants.SYSTEM_NAME,
                    CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
                },
                new Role
                {
                    Id = 4,
                    Name = "Parent",
                    CreatedBy = Constants.SYSTEM_NAME,
                    CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
                },
                 new Role
                 {
                     Id = 5,
                     Name = "Student",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
                 }
           );

            modelBuilder.Entity<Term>().HasData(
               new Term
               {
                   Id = 1,
                   Name = "First",
                   CreatedBy = Constants.SYSTEM_NAME,
                   CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
               },
               new Term
               {
                   Id = 2,
                   Name = "Second",
                   CreatedBy = Constants.SYSTEM_NAME,
                   CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
               },
               new Term
               {
                   Id = 3,
                   Name = "Third",
                   CreatedBy = Constants.SYSTEM_NAME,
                   CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
               }
          );

            modelBuilder.Entity<TermSection>().HasData(
              new TermSection
              {
                  Id = 1,
                  Name = "First-Half",
                  CreatedBy = Constants.SYSTEM_NAME,
                  CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
              },
              new TermSection
              {
                  Id = 2,
                  Name = "Second-Half",
                  CreatedBy = Constants.SYSTEM_NAME,
                  CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
              }
         );

              modelBuilder.Entity<ClassType>().HasData(
                 new ClassType
                 {
                     Id = 1,
                     Name = "Nursery",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
                 },
                 new ClassType
                 {
                     Id = 2,
                     Name = "Primary",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
                 },
                  new ClassType
                  {
                      Id = 3,
                      Name = "Secondary",
                      CreatedBy = Constants.SYSTEM_NAME,
                      CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
                  }
            );

            modelBuilder.Entity<RoomCode>().HasData(
                new RoomCode
                {
                    Id = 1,
                    Code = "CHARITY",
                    CreatedBy = Constants.SYSTEM_NAME,
                    CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
                },
                new RoomCode
                {
                    Id = 2,
                    Code = "PEACE",
                    CreatedBy = Constants.SYSTEM_NAME,
                    CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
                },
                 new RoomCode
                 {
                     Id = 3,
                     Code = "LOVE",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
                 }
           );

            modelBuilder.Entity<ExamType>().HasData(
                new ExamType
                {
                    Id = 1,
                    Name = "Mid-Term",
                    CreatedBy = Constants.SYSTEM_NAME,
                    CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
                },
                new ExamType
                {
                    Id = 2,
                    Name = "End-Term",
                    CreatedBy = Constants.SYSTEM_NAME,
                    CreatedDate = new DateTimeOffset(2021, 10, 29, 18, 38, 0, TimeSpan.FromMinutes(60))
                }
           );

            modelBuilder.Entity<BehaviouralRating>().HasData(
               new BehaviouralRating
               {
                   Id = 1,
                   Name = "Punctuality",
                   Category="Affective",
                   CreatedBy = Constants.SYSTEM_NAME,
                   CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
               },
                new BehaviouralRating
                {
                    Id = 2,
                    Name = "Neatness",
                    Category = "Affective",
                    CreatedBy = Constants.SYSTEM_NAME,
                    CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
                },
                 new BehaviouralRating
                 {
                     Id = 3,
                     Name = "Politeness",
                     Category = "Affective",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
                 },
                 new BehaviouralRating
                 {
                     Id = 4,
                     Name = "Leadership",
                     Category = "Affective",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
                 },
                 new BehaviouralRating
                 {
                     Id = 5,
                     Name = "Obedience",
                     Category = "Affective",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
                 },
                 new BehaviouralRating
                 {
                     Id = 6,
                     Name = "Poise",
                     Category = "Affective",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
                 },
                 new BehaviouralRating
                 {
                     Id = 7,
                     Name = "Helping Others",
                     Category = "Affective",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
                 },
                 new BehaviouralRating
                 {
                     Id = 8,
                     Name = "Emotional Stability",
                     Category = "Affective",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
                 },
                 new BehaviouralRating
                 {
                     Id = 9,
                     Name = "Health",
                     Category = "Affective",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
                 },
                 new BehaviouralRating
                 {
                     Id = 10,
                     Name = "Attentiveness",
                     Category = "Affective",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
                 },
                 new BehaviouralRating
                 {
                     Id = 11,
                     Name = "Honesty",
                     Category = "Affective",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
                 },
                 new BehaviouralRating
                 {
                     Id = 12,
                     Name = "Orderliness",
                     Category = "Affective",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
                 },
                 new BehaviouralRating
                 {
                     Id = 13,
                     Name = "Handwriting",
                     Category = "Psychomotor",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
                 },
                 new BehaviouralRating
                 {
                     Id = 14,
                     Name = "Sports",
                     Category = "Psychomotor",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
                 },
                 new BehaviouralRating
                 {
                     Id = 15,
                     Name = "Musical Skills",
                     Category = "Psychomotor",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
                 },
                 new BehaviouralRating
                 {
                     Id = 16,
                     Name = "Drawing/Painting",
                     Category = "Psychomotor",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
                 },
                 new BehaviouralRating
                 {
                     Id = 17,
                     Name = "Verbal Fluency",
                     Category = "Psychomotor",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
                 },
                 new BehaviouralRating
                 {
                     Id = 18,
                     Name = "Handling Tools",
                     Category = "Psychomotor",
                     CreatedBy = Constants.SYSTEM_NAME,
                     CreatedDate = new DateTimeOffset(2022, 01, 23, 23, 43, 0, TimeSpan.FromMinutes(60))
                 }

          );

        }
    }
}
