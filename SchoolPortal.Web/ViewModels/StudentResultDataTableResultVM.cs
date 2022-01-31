using DataTablesParser;
using SchoolPortal.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class StudentResultDataTableResultVM:Results<StudentResultItemVM>
    {
        public decimal TotalScoreObtained { get; set; }
        public decimal TotalScoreObtainable { get; set; }
        public decimal Percentage { get; set; }
        public static StudentResultDataTableResultVM FromDTResults(Results<StudentResultItemVM> result)
        {
            return new StudentResultDataTableResultVM
            {
                data = result.data,
                draw = result.draw,
                recordsFiltered = result.recordsFiltered,
                recordsTotal = result.recordsTotal
            };
        }
    }
}
