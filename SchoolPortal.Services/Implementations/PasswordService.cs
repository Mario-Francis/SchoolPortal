using Easy_Password_Validator;
using Easy_Password_Validator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Services.Implementations
{
    public class PasswordService:IPasswordService
    {
        public string Hash(string password)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return hashedPassword;
        }

        public bool Verify(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public bool ValidatePassword(string password, out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(password?.Trim()))
            {
                errorMessage = "Password is required";
                return false;
            }
            var passwordOptions = new PasswordRequirements
            {
                ExitOnFailure = true,
                MaxRepeatSameCharacter = 4,
                UseEntropy = false,
                MinLength = 8,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
                RequirePunctuation = false,
                MinUniqueCharacters = 1,
                MinScore = 50
            };
            var passwordValidator = new PasswordValidatorService(passwordOptions);
            var pass = passwordValidator.TestAndScore(password);
            if (!pass)
            {
                errorMessage = string.Join(',', passwordValidator.FailureMessages);
            }
            return pass;
        }
    }
}
