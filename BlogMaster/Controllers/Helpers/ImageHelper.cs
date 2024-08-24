using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;  
using Microsoft.AspNetCore.Mvc;

namespace BlogMaster.Controllers.Helpers
{
    public static class ImageHelper
    {
        public static byte[] EncodeImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Invalid image file");

            using (var memoryStream = new MemoryStream())
            {
                imageFile.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static FileContentResult DecodeImage(byte[] imageData, string contentType, string? fileName = null)
        {
            if (imageData == null || imageData.Length == 0)
                throw new ArgumentException("Invalid image data");

            return new FileContentResult(imageData, contentType)
            {
                FileDownloadName = fileName
            };
        }
    }
}
