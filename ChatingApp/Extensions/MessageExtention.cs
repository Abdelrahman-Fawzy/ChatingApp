using ChatingApp.BackEnd.DTOs;
using ChatingApp.BackEnd.Entities;
using System.Linq.Expressions;

namespace ChatingApp.BackEnd.Extensions
{
    public static class MessageExtention
    {
        public static MessageDTO ToDTO(this Message message)
        {
            return new MessageDTO
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderDisplayName = message.Sender.DisplayName,
                SenderImageUrl = message.Sender.ImageUrl,
                RecipientId = message.RecipientId,
                RecipientDisplayName = message.Recipient.DisplayName,
                RecipientImageUrl = message.Recipient.ImageUrl,
                Content = message.Content,
                DateRead = message.DateRead,
                MessageSent = message.MessageSent
            };
        }

        public static Expression<Func<Message, MessageDTO>> ToDTOProjection()
        {
            return message => new MessageDTO
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderDisplayName = message.Sender.DisplayName,
                SenderImageUrl = message.Sender.ImageUrl,
                RecipientId = message.RecipientId,
                RecipientDisplayName = message.Recipient.DisplayName,
                RecipientImageUrl = message.Recipient.ImageUrl,
                Content = message.Content,
                DateRead = message.DateRead,
                MessageSent = message.MessageSent
            };
        }
    }
}
