using AutoMapper;
using BankMicroservices.Email.Data.ValueObjects;
using BankMicroservices.Email.Messages;
using BankMicroservices.Email.Model;
using BankMicroservices.Email.Model.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankMicroservices.Email.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly DbContextOptions<MySQLContext> _context;
        private IMapper _mapper;

        public EmailRepository(DbContextOptions<MySQLContext> context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmailVO>> FindAll()
        {

            await using var _db = new MySQLContext(_context);
            List<EmailLog> emails = await _db.Emails.ToListAsync();
            return _mapper.Map<List<EmailVO>>(emails);
        }

        public async Task LogEmail(NotificationMessage message)
        {
            EmailLog email = new EmailLog()
            {
                Email = message.Email,
                SentDate = DateTime.Now,
                Log = $"Email '{message.Title}' to user {message.UserId} has been sent successfully!"
            };
            await using var _db = new MySQLContext(_context);
            _db.Emails.Add(email);
            await _db.SaveChangesAsync();
        }
    }
}
