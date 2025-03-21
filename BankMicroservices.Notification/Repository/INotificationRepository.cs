using BankMicroservices.Notification.Data.ValueObjects;
using BankMicroservices.Notification.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankMicroservices.Notification.Repository
{
    public interface INotificationRepository
    {
        Task<IEnumerable<NotificationVO>> FindAll();
        Task<bool> SetAsRead(long id, string userId);
        Task SendNotification(NotificationMessage message);
    }
}
