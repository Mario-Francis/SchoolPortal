using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.Web.ViewModels
{
    public class UserItemVM
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }

        public static UserItemVM FromUser(User user)
        {
            if (user == null)
            {
                return null;
            }
            else
            {
                return new UserItemVM
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    Surname = user.Surname
                };
            }
        }
    }
}
