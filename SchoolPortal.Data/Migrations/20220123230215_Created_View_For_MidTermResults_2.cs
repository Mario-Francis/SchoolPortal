using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Created_View_For_MidTermResults_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                create or alter view MidTermResults_View as
                    select CONCAT_WS(' ', st.FirstName, st.MiddleName, st.Surname) as StudentName, st.AdmissionNo, e.Session, e.Term, e.TermId, e.ExamTypeId, e.ExamType, c.ClassName, cr.RoomCode, CONCAT(s.Name,' (', s.Code,')') as SubjectName, r.* 
                    from MidTermResults r
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
                    join Students st on r.StudentId  = st.Id;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                drop view MidTermResults_View;
            ");
        }
    }
}
