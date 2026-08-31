using ChatingApp.BackEnd.Helpers;
using ChatingApp.BackEnd.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace ChatingApp.BackEnd.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _Cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _Cloudinary = new Cloudinary(account);
        }
        public async Task<DeletionResult> DeletePhotoAync(string publicId)
        {
            var delettinParams = new DeletionParams(publicId);

            return await _Cloudinary.DestroyAsync(delettinParams);
        }

        public async Task<ImageUploadResult> UploadPhotoAync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = "ChatingApp"
                };

                uploadResult = await _Cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }
    }
}
