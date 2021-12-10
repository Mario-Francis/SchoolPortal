using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Core
{
    public class AppUtilities
    {
        public static bool ValidateSession(string session)
        {
            var isValid = true;
            var maxYear = DateTime.Now.Year + 1;
            session = session.Trim();
            var arr = session.Split('/');
            if (arr.Length != 2 || string.IsNullOrEmpty(arr[0]) || string.IsNullOrEmpty(arr[1]))
            {
                isValid = false;
            }
            else if (!int.TryParse(arr[0], out _) || !int.TryParse(arr[1], out _))
            {
                isValid = false;
            }
            else
            {
                var year1 = Convert.ToInt32(arr[0]);
                var year2 = Convert.ToInt32(arr[1]);

                if (year1.ToString().Length != 4 || year1 > maxYear || year2.ToString().Length != 4 || year2 > maxYear)
                {
                    isValid = false;
                }
                else if (year2 <= year1)
                {
                    isValid = false;
                }
            }

            return isValid;
        }
    }
}
