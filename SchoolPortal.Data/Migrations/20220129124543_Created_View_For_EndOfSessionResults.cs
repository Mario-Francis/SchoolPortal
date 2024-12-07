using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Created_View_For_EndOfSessionResults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                create or alter view EndOfSessionResults_View as
                    select t1.MidTermResultId as FirstMidTermResultId, t1.Id as FirstEndTermResultId, t1.MidTermTotal as FirstMidTermTotal, t1.Total as FirstEndTermTotal, t1.TermTotal as FirstTermTotal, 
					   t2.MidTermResultId as SecondMidTermResultId, t2.Id as SecondEndTermResultId, t2.MidTermTotal as SecondMidTermTotal, t2.Total as SecondEndTermTotal, t2.TermTotal as SecondTermTotal, 
					   (t3.MidTermTotal+t3.Total) as TermTotal, ROUND(((t1.TermTotal+t2.TermTotal+(t3.MidTermTotal+t3.Total))/3), 2) as AverageScore, t3.* from
						(select CONCAT_WS(' ', st.FirstName, st.MiddleName, st.Surname) as StudentName, st.AdmissionNo, e.Session, e.Term, e.TermId, e.ExamTypeId, e.ExamType, c.ClassName, cr.RoomCode, 
								CONCAT(s.Name,' (', s.Code,')') as SubjectName, mr.Total as MidTermTotal, mr.Id as MidTermResultId, r.* 
								from EndTermResults r
								join(
									select e.*, et.Name  as ExamType, t.Name as Term from (select * from exams where TermId = 3) e 
									join Terms t on e.TermId = t.Id
									join ExamTypes et on e.ExamTypeId=et.Id
									) e 
									on r.ExamId = e.Id
								join( 
									select c.*, CONCAT_WS(' ', ct.Name, c.ClassGrade) as ClassName from  Classes c 
									join ClassTypes ct on c.ClassTypeId = ct.Id
									) c 
									on r.ClassId = c.Id
								join ClassRooms cr on r.ClassRoomId = cr.Id
								join Subjects s on r.SubjectId=s.Id
								join Students st on r.StudentId  = st.Id
								join(
									select e.Session, e.TermId, m.ClassId, m.SubjectId,m.StudentId, m.Total, m.Id from MidTermResults m 
									join Exams e on m.ExamId = e.Id
									) mr 
								on e.Session = mr.Session and e.TermId = mr.TermId and r.ClassId=mr.ClassId and r.SubjectId=mr.SubjectId and r.StudentId=mr.StudentId
								) t3
						join(
							select e.Session, e.TermId, et.ClassId, et.SubjectId,et.StudentId, et.Total, et.Id, mt.Id as MidTermResultId, mt.Total as MidTermTotal, (et.Total + mt.Total) as TermTotal from EndTermResults et 
							join (select * from exams where TermId = 2) e on et.ExamId = e.Id
							join (
									select me.Session, me.TermId, m.ClassId, m.SubjectId,m.StudentId, m.Total, m.Id from MidTermResults m 
									join Exams me on m.ExamId = me.Id
								)mt on e.Session = mt.Session and e.TermId = mt.TermId and et.ClassId=mt.ClassId and et.SubjectId=mt.SubjectId and et.StudentId=mt.StudentId
							) t2 on t3.Session = t2.Session and t3.ClassId=t2.ClassId and t3.SubjectId=t2.SubjectId and t3.StudentId=t2.StudentId
						join(
							select e.Session, e.TermId, et.ClassId, et.SubjectId,et.StudentId, et.Total, et.Id, mt.Id as MidTermResultId, mt.Total as MidTermTotal, (et.Total + mt.Total) as TermTotal from EndTermResults et 
							join (select * from exams where TermId = 1) e on et.ExamId = e.Id
							join (
									select me.Session, me.TermId, m.ClassId, m.SubjectId,m.StudentId, m.Total, m.Id from MidTermResults m 
									join Exams me on m.ExamId = me.Id
								)mt on e.Session = mt.Session and e.TermId = mt.TermId and et.ClassId=mt.ClassId and et.SubjectId=mt.SubjectId and et.StudentId=mt.StudentId
							) t1 on t3.Session = t1.Session and t3.ClassId=t1.ClassId and t3.SubjectId=t1.SubjectId and t3.StudentId=t1.StudentId;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                drop view EndOfSessionResults_View;
            ");
        }
    }
}
