using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class ResultCommentVM
    {
        public long Id { get; set; }
        public string HeadTeacherComment { get; set; }
        public string TeacherComment { get; set; }
        public long ExamId { get; set; }
        public long StudentId { get; set; }

        public PerformanceRemark ToPerformanceRemark()
        {
            return new PerformanceRemark
            {
                Id = Id,
                TeacherRemark = TeacherComment,
                HeadTeacherRemark = HeadTeacherComment,
                ExamId = ExamId,
                StudentId = StudentId
            };
        }

        public static ResultCommentVM FromPerformanceRemark(PerformanceRemark remark)
        {
            return remark == null ? null : new ResultCommentVM
            {
                Id = remark.Id,
                TeacherComment = remark.TeacherRemark,
                HeadTeacherComment = remark.HeadTeacherRemark
            };
        }
        public static ResultCommentVM FromResultCommentObject(ResultCommentObject remark)
        {
            return remark == null ? null : new ResultCommentVM
            {
                Id = remark.Id,
                TeacherComment = remark.TeacherComment,
                HeadTeacherComment = remark.HeadTeacherComment
            };
        }


    }
}
