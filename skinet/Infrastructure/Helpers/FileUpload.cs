using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Helpers
{
    public class FileUpload
    {
        private readonly string _uploadFolderPath;
        private readonly string _apiUrl;

        public FileUpload(string uploadFolderPath, string apiUrl)
        {
            _uploadFolderPath = uploadFolderPath;
            _apiUrl = apiUrl;
           
        }

        public async Task<string> UploadFileAsync(IFormFile file,string mediaType)
        {
            if (file == null || file.Length == 0)
            {
                // Dosya sağlanmadığı durumu ele alın
                return null;
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(_uploadFolderPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            string relativePath = Path.Combine("images/products", uniqueFileName).Replace("\\","/");

             return $"{_apiUrl}{relativePath}";
        }
    }
}
