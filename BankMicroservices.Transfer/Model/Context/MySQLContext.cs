using Microsoft.EntityFrameworkCore;

namespace BankMicroservices.Transfer.Model.Context
{
    public class MySQLContext : DbContext
    {
        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) {}

        public DbSet<TransferModel> Transfers { get; set; }
    }
}