using Microsoft.EntityFrameworkCore;

namespace BankMicroservices.Notification.Model.Context
{
    public class MySQLContext : DbContext
    {
        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) {}

        public DbSet<NotificationModel> Notifications { get; set; }
    }
}