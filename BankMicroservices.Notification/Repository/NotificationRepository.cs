using AutoMapper;
using BankMicroservices.Notification.Data.ValueObjects;
using BankMicroservices.Notification.Messages;
using BankMicroservices.Notification.Model;
using BankMicroservices.Notification.Model.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankMicroservices.Notification.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly DbContextOptions<MySQLContext> _context;
        private IMapper _mapper;

        public NotificationRepository(DbContextOptions<MySQLContext> context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NotificationVO>> FindAll()
        {

            await using var _db = new MySQLContext(_context);
            List<NotificationModel> notifications = await _db.Notifications.ToListAsync();
            return _mapper.Map<List<NotificationVO>>(notifications);
        }

        public async Task SendNotification(NotificationMessage message)
        {
            NotificationModel notification = new NotificationModel()
            {
                Title = message.Title,
                Message = message.Message,
                UserId = message.UserId,
                SentDate = DateTime.Now,
                Read = false
            };

            await using var _db = new MySQLContext(_context);
            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> SetAsRead(long id, string userId)
        {
            await using var _db = new MySQLContext(_context);
            NotificationModel notification = _db.Notifications.Where(n => n.Id == id && userId == n.UserId).FirstOrDefault();
            if (notification == null)
            {
                return false;
            }

            notification.Read = true;
            _db.Notifications.Update(notification);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
