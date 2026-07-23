using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface INotificationService
    {
        Task SendNotificationToUserAsync(string userId, string messageType, object payload);
        Task SendNotificationToAllAsync(string messageType, object payload);
        Task SendNotificationToGroupAsync(string groupName, string messageType, object payload);
        Task SendNotificationToSalonStaffAsync(string salonId, string messageType, object payload);

    }
}
