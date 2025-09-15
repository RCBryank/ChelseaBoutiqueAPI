using Chelsea_Boutique.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

namespace Chelsea_Boutique.Services
{
    public class CloudinaryService(IConfiguration _configuration)
    {
        private string CloudinaryURL = _configuration.GetValue<string>("Cloudinary_URL");

        public ImageUploadResult UploadMedia(IFormFile file, string folder)
        {
            Cloudinary cloudinary = new Cloudinary(CloudinaryURL);
            cloudinary.Api.Secure = true;

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                UseFilename = false,
                UniqueFilename = true,
                Overwrite = true,
                AssetFolder = folder
            };

            return cloudinary.Upload(uploadParams);
        }
    }
}
