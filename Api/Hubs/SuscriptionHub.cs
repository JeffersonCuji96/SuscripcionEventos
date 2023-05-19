using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs
{
    public class SuscriptionHub:Hub
    {
        /// <summary>
        /// Método para ingresar a un grupo de notificaciones
        /// </summary>
        /// <param name="grupo"></param>
        /// <returns></returns>
        [Authorize]
        public async Task JoinGroup(string grupo)
        {
            var id = Context.ConnectionId;
            await Groups.AddToGroupAsync(id,grupo);
        }

        /// <summary>
        /// Método para salir de un grupo de notificaciones
        /// </summary>
        /// <param name="grupo"></param>
        /// <returns></returns>
        public async Task LeaveGroup(string grupo)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, grupo);
        }
    }
}
