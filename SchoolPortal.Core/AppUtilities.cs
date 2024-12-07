using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

        public static bool ValidateFile(IFormFile file, out List<string> errorItems, int maxUploadSize)
        {
            bool isValid = true;
            List<string> errList = new List<string>();
            if (file == null)
            {
                isValid = false;
                errList.Add("No file uploaded.");
            }
            else
            {
                if (file.Length > (maxUploadSize * 1024 * 1024))
                {
                    isValid = false;
                    errList.Add($"Max upload size exceeded. Max size is {maxUploadSize}MB");
                }
                var ext = Path.GetExtension(file.FileName);
                if (ext != ".xls" && ext != ".xlsx")
                {
                    isValid = false;
                    errList.Add($"Invalid file format. Supported file formats include .xls and .xlsx");
                }
            }
            errorItems = errList;
            return isValid;
        }

        public static bool ValidateHeader(DataRow row, string[] headers, out string errorMessage)
        {
            var err = "";
            var isValid = true;
            for (int i = 0; i < headers.Length; i++)
            {
                if (row[i] == null || Convert.ToString(row[i]).Trim().ToLower() != headers[i].ToLower())
                {
                    isValid = false;
                    err = $"Invalid header value at column {i + 1}. Expected value is {headers[i]}";
                    break;
                }
            }
            errorMessage = err;
            return isValid;
        }

        public static bool IsValidDecimal(string value)
        {
            return Decimal.TryParse(value, out _);
        }


        public static bool ValidateImage(IFormFile file, out List<string> errorItems, int maxUploadSize)
        {
            bool isValid = true;
            List<string> errList = new List<string>();
            if (file == null)
            {
                isValid = false;
                errList.Add("No image uploaded.");
            }
            else
            {
                if (file.Length > (maxUploadSize * 1024 * 1024))
                {
                    isValid = false;
                    errList.Add($"Max upload size exceeded. Max size is {maxUploadSize}MB");
                }
                var ext = Path.GetExtension(file.FileName);
                if (ext != ".jpeg" && ext != ".jpg" && ext != ".png")
                {
                    isValid = false;
                    errList.Add($"Invalid file format. Supported file formats include .jpeg, .jpg and .png");
                }
            }
            errorItems = errList;
            return isValid;
        }


    }
}
