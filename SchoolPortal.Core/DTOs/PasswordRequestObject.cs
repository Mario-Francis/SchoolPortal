using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Core.DTOs
{
    public class PasswordRequestObject
    {
        public long UserId { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }

        public bool NewPasswordMatch
        {
            get
            {
                return NewPassword == ConfirmNewPassword;
            }
        }
    }
}
