using ChatingApp.BackEnd.DTOs;
using ChatingApp.BackEnd.Entities;
using ChatingApp.BackEnd.Extensions;
using ChatingApp.BackEnd.Helpers;
using ChatingApp.BackEnd.Interfaces;
using ChatingApp.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatingApp.BackEnd.Data
{
    public class MessageRepository(AppDbContext context) : IMessageRepository
    {
        public void AddGroup(Group group)
        {
            context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<Connection?> GetConnection(string connectionID)
        {
            return await context.Connections.FindAsync(connectionID);
        }

        public async Task<Group?> GetGroupForConnection(string connectionID)
        {
            return await context.Groups.Include(g => g.Connections)
                .Where(g => g.Connections.Any(c => c.ConnectionID == connectionID))
                .FirstOrDefaultAsync();
        }

        public async Task<Message?> GetMessage(string messageId)
        {
            return await context.Messages.FindAsync(messageId);
        }

        public async Task<Group?> GetMessageGroup(string groupName)
        {
            return await context.Groups.Include(g => g.Connections)
                .FirstOrDefaultAsync(g => g.Name == groupName);
        }

        public async Task<PaginatedResult<MessageDTO>> GetMessagesForMember(MessageParams messageParams)
        {
            var query = context.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Outbox" => query.Where(m => m.SenderId == messageParams.MemberId),
                _ => query.Where(m => m.RecipientId == messageParams.MemberId)
            };

            var messageQuery = query.Select(MessageExtention.ToDTOProjection());

            return await PaginationHelper.createPaginationAsync(messageQuery, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IReadOnlyList<MessageDTO>> GetMessageThread(string currentmemberId, string otherMemberId)
        {
            // get unread messages for the current member and mark them as read
            await context.Messages.Where(message => 
                message.RecipientId == currentmemberId && 
                message.SenderId == otherMemberId && 
                message.DateRead == null)
                .ExecuteUpdateAsync(setters => setters.SetProperty(m => m.DateRead, DateTime.UtcNow));

            // get the message thread between the two members
            return await context.Messages.Where(message =>
                (message.RecipientId == currentmemberId && message.SenderId == otherMemberId)
                ||
                (message.RecipientId == otherMemberId && message.SenderId == currentmemberId)
            ).OrderBy(m => m.MessageSent).Select(MessageExtention.ToDTOProjection()).ToListAsync();

        }

        public async Task RemoveConnection(string connectionID)
        {
            await context.Connections.Where(c => c.ConnectionID == connectionID).ExecuteDeleteAsync();
        }
    }
}
