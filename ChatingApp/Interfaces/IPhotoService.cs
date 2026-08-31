using CloudinaryDotNet.Actions;

namespace ChatingApp.BackEnd.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> UploadPhotoAync(IFormFile file);

        Task<DeletionResult> DeletePhotoAync(string publicId);
    }
}
