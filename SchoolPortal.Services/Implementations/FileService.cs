using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SchoolPortal.Services.Implementations
{
    public class FileService: IFileService
    {
        private readonly IWebHostEnvironment hostEnvironment;

        public FileService(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }

        public bool ValidateFile(IFormFile file, IEnumerable<string> allowedExtensions, out List<string> errorItems, int maxUploadSize = 20)
        {
            bool isValid = true;
            List<string> errList = new List<string>();
            if (file == null)
            {
                isValid = false;
                errList.Add("No file uploaded.");
            }
            else
            {
                if (file.Length > (maxUploadSize * 1024 * 1024))
                {
                    isValid = false;
                    errList.Add($"Max upload size exceeded. Max size is {maxUploadSize}MB");
                }
                var ext = Path.GetExtension(file.FileName);
                if (! allowedExtensions.Contains(ext))
                {
                    isValid = false;
                    errList.Add($"Invalid file format. Supported file formats include {string.Join(", ", allowedExtensions)}");
                }
            }
            errorItems = errList;
            return isValid;
        }

        public string SaveFile(IFormFile file, string uploadFolderPath="uploads")
        {
            string uploadsFolder = Path.Combine(hostEnvironment.WebRootPath, uploadFolderPath);
            var fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            var _filePath = Path.Combine(uploadFolderPath, fileName);
            return _filePath;
        }

        public void DeleteFile(string filePath)
        {
            string fullPath = Path.Combine(hostEnvironment.WebRootPath, filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
