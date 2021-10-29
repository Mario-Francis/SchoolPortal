using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class SetupVM
    {
        public long Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }

        public User ToUser()
        {
            return new User
            {
                FirstName = FirstName,
                MiddleName = MiddleName,
                Surname = Surname,
                Gender = Gender,
                Email = Email,
                PhoneNumber = PhoneNumber,
                Password = Password
            };
        }
    }
}
