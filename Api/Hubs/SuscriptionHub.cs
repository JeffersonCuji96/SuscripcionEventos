using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs
{
    public class SuscriptionHub:Hub
    {
        [Authorize]
        public async Task JoinGroup(string grupo)
        {
            var id = Context.ConnectionId;
            await Groups.AddToGroupAsync(id,grupo);
        }
        public async Task LeaveGroup(string grupo)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, grupo);
        }
    }
}
