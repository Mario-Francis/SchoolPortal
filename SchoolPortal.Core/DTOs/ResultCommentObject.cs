using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Core.DTOs
{
    public class ResultCommentObject
    {
        public long Id { get; set; }
        public string HeadTeacherComment { get; set; }
        public string TeacherComment { get; set; }

        public static ResultCommentObject FromPerformanceRemark(PerformanceRemark remark)
        {
            return remark == null ? null : new ResultCommentObject
            {
                Id = remark.Id,
                TeacherComment = remark.TeacherRemark,
                HeadTeacherComment = remark.HeadTeacherRemark
            };
        }
    }
}
