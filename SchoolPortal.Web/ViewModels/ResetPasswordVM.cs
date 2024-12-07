using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SchoolPortal.Core.DTOs;

namespace SchoolPortal.Web.ViewModels
{
    public class ResetPasswordVM
    {

        [Required]
        public string UserType { get; set; }
        [Required]
        public long? UserId { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
        [Required]
        [MinLength(8)]
        public string ConfirmPassword { get; set; }

        public PasswordRequestObject ToPasswordRequestObject()
        {
            return new PasswordRequestObject
            {
                UserId = UserId.Value,
                NewPassword = Password,
                ConfirmNewPassword = Password
            };
        }
    }
}
