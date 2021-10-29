using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Services
{
    public interface IPasswordService
    {
        string Hash(string password);

        bool Verify(string password, string hashedPassword);

        bool ValidatePassword(string password, out string errorMessage);
    }
}
