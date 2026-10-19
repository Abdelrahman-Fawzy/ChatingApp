using ChatingApp.BackEnd.DTOs;
using ChatingApp.BackEnd.Entities;
using ChatingApp.BackEnd.Helpers;

namespace ChatingApp.BackEnd.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message?> GetMessage(string id);
        Task<PaginatedResult<MessageDTO>> GetMessagesForMember(MessageParams messageParams);
        Task<IReadOnlyList<MessageDTO>> GetMessageThread(string currentmemberId, string otherMemberId);
        Task<bool> SaveAllAsync();
    }
}
