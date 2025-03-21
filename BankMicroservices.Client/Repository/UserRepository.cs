using AutoMapper;
using BankMicroservices.Client.Data.ValueObjects;
using BankMicroservices.Client.Model;
using BankMicroservices.Client.Model.Context;
using BankMicroservices.Client.Repository.Caching;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.IO;
using BankMicroservices.Client.Messages;
using BankMicroservices.Client.RabbitMQSender;

namespace BankMicroservices.Client.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MySQLContext _context;
        private readonly ICachingService _caching;
        private IMapper _mapper;
        private IRabbitMQMessageSender _rabbitMQMessageSender;
        private IRabbitMQMessageSender _rabbitMQLogSender;

        public UserRepository(DbContextOptions<MySQLContext> context, IMapper mapper, ICachingService caching, [FromKeyedServices("Notification")] IRabbitMQMessageSender rabbitMQMessageSender, [FromKeyedServices("Log")]IRabbitMQMessageSender rabbitMQLogSender)
        {
            _context = new MySQLContext(context);
            _caching = caching;
            _mapper = mapper;
            _rabbitMQMessageSender = rabbitMQMessageSender;
            _rabbitMQLogSender = rabbitMQLogSender;
        }

        public async Task<List<UserVO>> GetByNameOrEmail(string name, string email)
        {
            List<User> users = await _context.Users
                .Where(a => a.FullName.ToLower().Contains(name.ToLower())
                && a.Email.ToLower().Contains(email.ToLower())).ToListAsync();
            return _mapper.Map<List<UserVO>>(users);
        }

        public async Task<UserVO> GetByUserId(string userId)
        {
            string? userCache = _caching.GetAsync(userId);

            if (!string.IsNullOrWhiteSpace(userCache))
            {
                var userVO = JsonSerializer.Deserialize<UserVO>(userCache);
                if(userVO != null)
                    return userVO;
            }
            
            var user = await _context.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();

            _caching.SetAsync(user?.UserId ?? "", JsonSerializer.Serialize(user));

            return _mapper.Map<UserVO>(user);
        }

        public async Task<UserVO> Create(CreateUserVO userVO)
        {
            var random = new Random();
            User user = new User
            {
                UserId = userVO.UserId,
                FullName = userVO.FullName,
                Email = userVO.Email,
                Address = userVO.Address,
                Gender = userVO.Gender,
                DateRegistered = DateTime.Now,
                AccountNumber = random.Next(100000, 1000000),
                Agency = random.Next(0, 100),
                Balance = random.Next(100, 1000000)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return _mapper.Map<UserVO>(user);
        }

        public async Task<UserVO> Update(UserVO userVO, bool isAdmin)
        {
            User user = _mapper.Map<User>(userVO);
            var u = await _context.Users.Where(u => u.UserId == user.UserId).FirstOrDefaultAsync();

            if (u == null)
            {
                var logMessage = new LogMessage
                {
                    Type = "Warning",
                    Message = $"User {userVO.UserId} was not found. At Update User"
                };

                _rabbitMQLogSender.SendMessage(logMessage);
                
                throw new Exception("User not found");
            }

            if (!isAdmin)
            {
                user.AccountNumber = u.AccountNumber;
                user.Agency = u.Agency;
            }
            user.DateRegistered = u.DateRegistered;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return _mapper.Map<UserVO>(user);
        }

        public async Task<bool> UserHasBalance(string userId, float quantity)
        {
            var u = await _context.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();
            if (u != null && quantity <= u.Balance ) return true;
            if(u == null)
            {
                var logMessage = new LogMessage
                {
                    Type = "Warning",
                    Message = $"User {userId} was not found. At UserHasBalance"
                };

                _rabbitMQLogSender.SendMessage(logMessage);
            }

            return false;
        }

        public async Task<UserVO> TransferBalance(string senderUserId, string receiverUserId, float quantity)
        {
            var senderUser = await _context.Users.Where(u => u.UserId == senderUserId).FirstOrDefaultAsync();
            if (senderUser == null)
            {

                var logMessage = new LogMessage
                {
                    Type = "Warning",
                    Message = $"User {senderUserId} was not found. SenderUser at TransferBalance"
                };

                _rabbitMQLogSender.SendMessage(logMessage);
                throw new Exception("Sender user not found");
            }

            if (quantity <= senderUser.Balance) {

                var receiverUser = await _context.Users.Where(u => u.UserId == receiverUserId).FirstOrDefaultAsync();
                if (receiverUser == null)
                {

                    var logMessage = new LogMessage
                    {
                        Type = "Warning",
                        Message = $"User {receiverUserId} was not found. ReceiverUser at TransferBalance"
                    };

                    _rabbitMQLogSender.SendMessage(logMessage);
                    throw new Exception("Receiver user not found");
                }

                var notificationMessage = new NotificationMessage
                {
                    UserId = receiverUser.UserId,
                    Email = receiverUser.Email,
                    Title = "You received a transfer.",
                    Message = $"Transfer amount {quantity}"
                };
                _rabbitMQMessageSender.SendMessage(notificationMessage);

                senderUser.Balance -= quantity;
                receiverUser.Balance += quantity;
                _context.Users.Update(senderUser);
                _context.Users.Update(receiverUser);
                await _context.SaveChangesAsync();
                return _mapper.Map<UserVO>(senderUser);
            } else
            {

                var logMessage = new LogMessage
                {
                    Type = "Warning",
                    Message = $"User {senderUser.Id} does not have sufficient balance to return this transfer. Transfer Amount {quantity}"
                };

                _rabbitMQLogSender.SendMessage(logMessage);
                throw new Exception($"User {senderUser.Id} does not have sufficient balance to return this transfer. Transfer Amount {quantity}");
            }

        }
    }
}
