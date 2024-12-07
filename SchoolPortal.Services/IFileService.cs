using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Services
{
    public interface IFileService
    {
        bool ValidateFile(IFormFile file, IEnumerable<string> allowedExtensions, out List<string> errorItems, int maxUploadSize = 20);
        string SaveFile(IFormFile file, string uploadFolderPath = "uploads");
        void DeleteFile(string filePath);
    }
}
