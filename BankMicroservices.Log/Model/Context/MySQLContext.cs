using Microsoft.EntityFrameworkCore;

namespace BankMicroservices.Log.Model.Context
{
    public class MySQLContext : DbContext
    {
        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) {}

        public DbSet<LogModel> Logs { get; set; }
    }
}