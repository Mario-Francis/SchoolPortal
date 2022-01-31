using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Created_View_For_EndTermResults_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                create or alter view EndTermResults_View as
                    select CONCAT_WS(' ', st.FirstName, st.MiddleName, st.Surname) as StudentName, st.AdmissionNo, e.Session, e.Term, e.TermId, e.ExamTypeId, e.ExamType, c.ClassName, cr.RoomCode, CONCAT(s.Name,' (', s.Code,')') as SubjectName, mr.Total as MidTermTotal, mr.Id as MidTermResultId, r.* 
                    from EndTermResults r
                    join(
	                    select e.*, et.Name  as ExamType, t.Name as Term from Exams e 
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
			            on e.Session = mr.Session and e.TermId = mr.TermId and r.ClassId=mr.ClassId and r.SubjectId=mr.SubjectId and r.StudentId=mr.StudentId;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                drop view EndTermResults_View;
            ");
        }
    }
}
