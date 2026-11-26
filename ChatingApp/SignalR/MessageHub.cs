using ChatingApp.BackEnd.Data;
using ChatingApp.BackEnd.DTOs;
using ChatingApp.BackEnd.Entities;
using ChatingApp.BackEnd.Extensions;
using ChatingApp.BackEnd.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ChatingApp.BackEnd.SignalR
{
    public class MessageHub(IUnitOfWork uow, IHubContext<PresenceHub> presenceHubContext) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUserId = httpContext?.Request.Query["userId"].ToString();
            if(string.IsNullOrEmpty(otherUserId))
            {
                throw new HubException("User ID is required to establish a connection.");
            }

            var groupName = GetGroupName(GetUserID(), otherUserId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroup(groupName);

            var messages = await uow.MessageRepository.GetMessageThread(GetUserID(), otherUserId);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await uow.MessageRepository.RemoveConnection(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDTO createMessageDTO)
        {
            var sender = await uow.MemberRepository.GetMember(GetUserID());
            var recepient = await uow.MemberRepository.GetMember(createMessageDTO.RecipientId);

            if (recepient == null)
                throw new HubException("هذا المستخدم غير موجود");
            if (sender == null)
                throw new HubException("هذا المرسل غير موجود");
            if (sender.Id == recepient.Id)
                throw new HubException("لا يمكنك إرسال رسالة إلى نفسك");

            var message = new Message
            {
                SenderId = sender.Id,
                RecipientId = recepient.Id,
                Content = createMessageDTO.Content
            };

            var groupName = GetGroupName(sender.Id, recepient.Id);
            var group = await uow.MessageRepository.GetMessageGroup(groupName);
            var userInGroup = group != null && group.Connections.Any(c => c.UserID == message.RecipientId);

            if(userInGroup)
            {
                message.DateRead = DateTime.UtcNow;
            }

            uow.MessageRepository.AddMessage(message);

            if (await uow.CompleteAsync()) {
                await Clients.Group(groupName).SendAsync("NewMessage", message.ToDTO());
                var connections = await PresenceTracker.GetConnectionsForUser(recepient.Id);
                if(connections != null && connections.Count > 0 && !userInGroup)
                {
                    await presenceHubContext.Clients.Clients(connections).SendAsync("NewMessageReceived", message.ToDTO());
                }
            }

            //throw new HubException("فشل إرسال الرسالة");
        }

        private async Task<bool> AddToGroup(string groupName)
        {
            var group = await uow.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, GetUserID());

            if(group == null)
            {
                group = new Group(groupName);
                uow.MessageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);
            return await uow.CompleteAsync();
        }

        public static string GetGroupName(string caller, string other)
        {
            var stringComparer = string.CompareOrdinal(caller, other) < 0 ? $"{caller}-{other}" : $"{other}-{caller}";
            return stringComparer;
        }

        private string GetUserID()
        {
            return Context.User?.GetMemberId() ?? throw new HubException("Member ID not found in claims.");
        }
    }
}
