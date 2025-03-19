using Microsoft.EntityFrameworkCore;

namespace BankMicroservices.Client.Model.Context
{
    public class MySQLContext : DbContext
    {
        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfilePicture> ProfilePictures { get; set; }
    }
}