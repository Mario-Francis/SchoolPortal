using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Services
{
    public interface IPdfGeneratorService
    {
        byte[] GeneratePdfFromUrl(string url);
    }
}
