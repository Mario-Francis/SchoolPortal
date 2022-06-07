using SchoolPortal.Core;
using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Models;
using SchoolPortal.Core.Models.Views;
using SchoolPortal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services.Implementations
{
    public class StudentResultService: IStudentResultService
    {
        private readonly IRepository<Student> studentRepo;
        private readonly IRepository<MidTermResult> midTermResultRepo;
        private readonly IRepository<EndTermResult> endTermResultRepo;
        private readonly IRepository<EndTermResultViewObject> endTermResultViewObjectRepo;
        private readonly IRepository<EndOfSessionResultViewObject> endOfSessionResultViewObjectRepo;
        private readonly IRepository<EndOfSecondTermResultViewObject> endOfSecondTermResultViewObjectRepo;
        private readonly IRepository<Exam> examRepo;
        private readonly ILoggerService<StudentResultService> logger;
        private readonly IRepository<PerformanceRemark> performanceRemarkRepo;
        private readonly IRepository<BehaviouralResult> behaviouralResultRepo;
        private readonly IRepository<HealthRecord> healthRecordRepo;
        private readonly IRepository<AttendanceRecord> attendanceRecordRepo;

        public StudentResultService(IRepository<Student> studentRepo,
            IRepository<MidTermResult>  midTermResultRepo,
            IRepository<EndTermResult> endTermResultRepo,
            IRepository<EndTermResultViewObject> endTermResultViewObjectRepo,
            IRepository<EndOfSessionResultViewObject> endOfSessionResultViewObjectRepo,
            IRepository<EndOfSecondTermResultViewObject> endOfSecondTermResultViewObjectRepo,
            IRepository<Exam>  examRepo,
            ILoggerService<StudentResultService> logger,
            IRepository<PerformanceRemark> performanceRemarkRepo,
            IRepository<BehaviouralResult> behaviouralResultRepo,
            IRepository<HealthRecord> healthRecordRepo,
            IRepository<AttendanceRecord> attendanceRecordRepo)
        {
            this.studentRepo = studentRepo;
            this.midTermResultRepo = midTermResultRepo;
            this.endTermResultRepo = endTermResultRepo;
            this.endTermResultViewObjectRepo = endTermResultViewObjectRepo;
            this.endOfSessionResultViewObjectRepo = endOfSessionResultViewObjectRepo;
            this.endOfSecondTermResultViewObjectRepo = endOfSecondTermResultViewObjectRepo;
            this.examRepo = examRepo;
            this.logger = logger;
            this.performanceRemarkRepo = performanceRemarkRepo;
            this.behaviouralResultRepo = behaviouralResultRepo;
            this.healthRecordRepo = healthRecordRepo;
            this.attendanceRecordRepo = attendanceRecordRepo;
        }

        // get student result sessions
        public IEnumerable<string> GetResultSessions(long studentId)
        {
            var msessions = midTermResultRepo.GetWhere(r => r.StudentId == studentId)
                .Select(r => r.Exam.Session).Distinct().ToList();

            var esessions = endTermResultRepo.GetWhere(r => r.StudentId == studentId)
                .Select(r => r.Exam.Session).Distinct().ToList();

            msessions.AddRange(esessions);
            msessions = msessions.Distinct().OrderByDescending(s => s).ToList();

            return msessions;
        }

        // get available terms
        public IEnumerable<Term> GetResultSessionTerms(long studentId, string session)
        {
            var terms = midTermResultRepo.GetWhere(r => r.Exam.Session == session && r.StudentId == studentId)
                .Select(r => r.Exam.Term).Distinct().ToList();

            var eterms = endTermResultRepo.GetWhere(r => r.Exam.Session == session && r.StudentId == studentId)
                .Select(r => r.Exam.Term).Distinct();

            terms.AddRange(eterms);
            terms = terms.Distinct().OrderByDescending(t => t.Id).ToList();

            return terms;
        }

        // get midterm results
        public IEnumerable<StudentResultItem> GetMidTermResults(long studentId, string session, long termId)
        {
            var results = midTermResultRepo.GetWhere(r => r.StudentId == studentId && r.Exam.Session == session && r.Exam.TermId == termId)
                .Select(r => StudentResultItem.FromMidTermResult(r));

            return results;
        }

        // private - get mid term comments
        private async Task<ResultCommentObject> GetMidTermComments(long studentId, string session, long termId)
        {
            var remark = await performanceRemarkRepo.GetSingleWhereAsync(r => r.StudentId == studentId && r.Exam.Session == session && r.Exam.TermId == termId && r.Exam.ExamTypeId == (int)ExamTypes.MID_TERM);

            return ResultCommentObject.FromPerformanceRemark(remark);
        }

        // get end term results
        public IEnumerable<StudentResultItem> GetEndTermResults(long studentId, string session, long termId)
        {
            var results = endTermResultViewObjectRepo.GetWhere(r => r.StudentId == studentId && r.Session == session && r.TermId == termId)
                .Select(r => StudentResultItem.FromEndTermResultViewObject(r));

            return results;
        }

        // private - get end of term comments
        private async Task<ResultCommentObject> GetEndTermComments(long studentId, string session, long termId)
        {
            var remark = await performanceRemarkRepo.GetSingleWhereAsync(r => r.StudentId == studentId && r.Exam.Session == session && r.Exam.TermId == termId && r.Exam.ExamTypeId == (int)ExamTypes.END_TERM);

            return ResultCommentObject.FromPerformanceRemark(remark);
        }

        // get end of term behavioural ratings
        public IEnumerable<BehaviouralResult> GetEndTermBehaviouralRatings(long studentId, string session, long termId)
        {
            var ratings = behaviouralResultRepo.GetWhere(r => r.StudentId == studentId && r.Session == session && r.TermId == termId);

            return ratings;
        }

        // private - get end of term health records
        private async Task<HealthRecordObject> GetEndTermHealthRecord(long studentId, string session, long termId)
        {
            var record = await healthRecordRepo.GetSingleWhereAsync(r => r.StudentId == studentId && r.Session == session && r.TermId == termId);

            return HealthRecordObject.FromHealthRecord(record);
        }

        private async Task<AttendanceRecordObject> GetEndTermAttendanceRecord(long studentId, string session, long termId)
        {
            var record = await attendanceRecordRepo.GetSingleWhereAsync(r => r.StudentId == studentId && r.Session == session && r.TermId == termId);

            return AttendanceRecordObject.FromAttendanceRecord(record);
        }

        // get end of session results
        public IEnumerable<StudentResultItem> GetEndOfSessionResults(long studentId, string session)
        {
            var results = endOfSessionResultViewObjectRepo.GetWhere(r => r.StudentId == studentId && r.Session == session)
                .Select(r => StudentResultItem.FromEndOfSessionResultViewObject(r));

            return results;
        }

        // get end of second term results
        public IEnumerable<StudentResultItem> GetEndOfSecondTermResults(long studentId, string session)
        {
            var results = endOfSecondTermResultViewObjectRepo.GetWhere(r => r.StudentId == studentId && r.Session == session)
                .Select(r => StudentResultItem.FromEndOfSecondTermResultViewObject(r));

            return results;
        }

        // get results
        public async Task<StudentResult> GetStudentResult(long studentId, string session, long termId)
        {
            var mResult = await midTermResultRepo.GetSingleWhereAsync(r => r.StudentId == studentId && r.Exam.Session == session && r.Exam.TermId == termId);
            var eResult = await endTermResultRepo.GetSingleWhereAsync(r => r.StudentId == studentId && r.Exam.Session == session && r.Exam.TermId == termId);
            
            var midTermResult = mResult==null?null: new StudentMidTermResult
            {
                ClassRoom = mResult.ClassRoom,
                Exam = mResult.Exam,
                Session = mResult.Exam.Session,
                Term = mResult.Exam.Term,
                ResultComment = await GetMidTermComments(studentId, session, termId)
            };

            var endTermResult = eResult == null ? null : new StudentEndTermResult
            {
                ClassRoom = eResult.ClassRoom,
                Exam = eResult.Exam,
                Session = eResult.Exam.Session,
                Term = eResult.Exam.Term,
                ResultComment = await GetEndTermComments(studentId, session, termId),
                HealthRecord = await GetEndTermHealthRecord(studentId, session, termId),
                BehaviouralResults = GetEndTermBehaviouralRatings(studentId, session, termId),
                AttendanceRecord = await GetEndTermAttendanceRecord(studentId, session, termId)
            };

            return new StudentResult
            {
                MidTermResult = midTermResult,
                EndTermResult = endTermResult
            };
        }
    }
}
