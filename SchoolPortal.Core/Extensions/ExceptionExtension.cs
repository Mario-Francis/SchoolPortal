using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Core.Extensions
{
    public static class ExceptionExtension
    {
        public static string GetErrorDetails(this Exception ex)
        {
            int count = 1;
            var message = $"({count}) {ex.Message}. ";
            var _ex = ex.InnerException;
            while (_ex != null)
            {
                ++count;
                message += $"({count}) {_ex.Message}. ";
                _ex = _ex.InnerException;
            }
            return message;
        }
    }
}
