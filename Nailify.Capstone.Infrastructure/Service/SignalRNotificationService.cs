using Microsoft.AspNetCore.SignalR;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task SendNotificationToUserAsync(string userId, string messageType, object payload)
        {
            // Gửi tới group của User đó (tương ứng với UserId)
            await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", messageType, payload);
        }
        public async Task SendNotificationToAllAsync(string messageType, object payload)
        {
            // Gửi tới toàn bộ client đang kết nối
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", messageType, payload);
        }
    }
}
