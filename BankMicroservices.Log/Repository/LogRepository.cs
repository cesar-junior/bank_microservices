using AutoMapper;
using BankMicroservices.Log.Data.ValueObjects;
using BankMicroservices.Log.Messages;
using BankMicroservices.Log.Model;
using BankMicroservices.Log.Model.Context;
using BankMicroservices.Log.Utils.Wrapper;
using Microsoft.EntityFrameworkCore;

namespace BankMicroservices.Log.Repository
{
    public class LogRepository : ILogRepository
    {
        private readonly DbContextOptions<MySQLContext> _context;
        private IMapper _mapper;

        public LogRepository(DbContextOptions<MySQLContext> context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<LogVO>>> GetWithOffsetPagination(int pageNumber, int pageSize)
        {
            await using var _db = new MySQLContext(_context);
            var totalRecords = await _db.Logs.AsNoTracking().CountAsync();

            List<LogModel> logs = await _db.Logs.AsNoTracking()
                                                .OrderBy(x => x.Id)
                                                .Skip((pageNumber - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToListAsync();

            var pagedResponse = new PagedResponse<List<LogVO>>(_mapper.Map<List<LogVO>>(logs), pageNumber, pageSize, totalRecords);

            return pagedResponse;
        }

        public async Task SendNotification(LogMessage message)
        {
            LogModel notification = new()
            {
                Type = message.Type,
                Message = message.Message,
                Date = DateTime.Now,
            };

            await using var _db = new MySQLContext(_context);
            _db.Logs.Add(notification);
            await _db.SaveChangesAsync();
        }
    }
}
