using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchoolPortal.Core.Extensions
{
    public static class StringExtension
    {
        public static string Capitalize(this string input)
        {
            var seperators = new string[] { "-" };
            var res = input;
            foreach(var s in seperators)
            {
                input = string.Join(s, input.Split(s).Select(s => _capitalize(s)));
            }
            return input;
        }

        private static string _capitalize(string input)
        {
            return string.Join(" ", input.ToLower().Split(' ')
               .Where(s => !(s == ""))
               .Select(s => s[0].ToString().ToUpper()
               + new string(s.Skip(1).ToArray()))
               );
        }
    }
}
