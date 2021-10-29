using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
   public interface IEmailService
    {
        Task<bool> IsEmailValidAsync(string email);
    }
}
