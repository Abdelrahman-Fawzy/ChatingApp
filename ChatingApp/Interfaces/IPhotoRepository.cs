using ChatingApp.BackEnd.DTOs;
using ChatingApp.BackEnd.Entities;

namespace ChatingApp.BackEnd.Interfaces
{
    public interface IPhotoRepository
    {
        Task<IReadOnlyList<PhotoForApproval>> GetUnApprovedPhotos();
        Task<Photo?> GetPhotoById(int id);
        void RemovePhoto(Photo photo);
    }
}
