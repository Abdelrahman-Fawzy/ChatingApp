using ChatingApp.BackEnd.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatingApp.BackEnd.SignalR
{
    [Authorize]
    public class PresenceHub(PresenceTracker presenceTracker) :  Hub
    {
        public override async Task OnConnectedAsync()
        {
            await presenceTracker.UserConnected(GetUserID(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserOnline", GetUserID());

            var currentUsers = await presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await presenceTracker.userDisconnected(GetUserID(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserOffline", GetUserID());

            var currentUsers = await presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

            await base.OnDisconnectedAsync(exception);
        }

        private string GetUserID()
        {
            return Context.User?.GetMemberId() ?? throw new HubException("Member ID not found in claims.");
        }
    }
}
