using ChatingApp.BackEnd.DTOs;
using ChatingApp.BackEnd.Entities;
using ChatingApp.BackEnd.Interfaces;
using ChatingApp.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatingApp.BackEnd.Data
{
    public class PhotoRepository(AppDbContext context) : IPhotoRepository
    {
        public async Task<Photo?> GetPhotoById(int id)
        {
            return await context.Photos
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IReadOnlyList<PhotoForApproval>> GetUnApprovedPhotos()
        {
            return await context.Photos
                .IgnoreQueryFilters()
                .Where(p => !p.IsApproved)
                .Select(p => new PhotoForApproval
                {
                    Id = p.Id,
                    Url = p.Url,
                    MemberId = p.MemberId,
                    IsApproved = p.IsApproved
                }).ToListAsync();
        }

        public void RemovePhoto(Photo photo)
        {
            context.Photos.Remove(photo);
        }
    }
}
