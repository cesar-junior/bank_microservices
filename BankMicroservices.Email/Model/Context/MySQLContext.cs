﻿using Microsoft.EntityFrameworkCore;

namespace BankMicroservices.Email.Model.Context
{
    public class MySQLContext : DbContext
    {
        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) {}

        public DbSet<EmailLog> Emails { get; set; }
    }
}