using SchoolPortal.Core.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class ChangePasswordVM
    {
        public long? UserId { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }

        [Required]
        [MinLength(8)]
        public string ConfirmNewPassword { get; set; }

        public PasswordRequestObject ToPasswordRequestObject()
        {
            return new PasswordRequestObject
            {
                UserId = UserId.Value,
                Password = Password,
                NewPassword = NewPassword,
                ConfirmNewPassword = ConfirmNewPassword
            };
        }
    }
}
