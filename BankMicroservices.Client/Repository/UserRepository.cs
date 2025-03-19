using AutoMapper;
using BankMicroservices.Client.Data.ValueObjects;
using BankMicroservices.Client.Model;
using BankMicroservices.Client.Model.Context;
using BankMicroservices.Client.Repository.Caching;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace BankMicroservices.Client.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MySQLContext _context;
        private readonly ICachingService _caching;
        private IMapper _mapper;

        public UserRepository(DbContextOptions<MySQLContext> context, IMapper mapper, ICachingService caching)
        {
            _context = new MySQLContext(context);
            _caching = caching;
            _mapper = mapper;
        }

        public async Task<List<UserVO>> GetByNameOrEmail(string name, string email)
        {
            List<User> users = await _context.Users
                .Where(a => a.FullName.ToLower().Contains(name.ToLower())
                || a.Email.ToLower().Contains(email.ToLower())).ToListAsync();
            return _mapper.Map<List<UserVO>>(users);
        }

        public async Task<UserVO> GetByUserId(string userId)
        {
            string userCache = _caching.GetAsync(userId);

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

        public async Task<UserVO> Create(UserVO userVO)
        {
            User user = _mapper.Map<User>(userVO);
            var random = new Random();
            user.AccountNumber = random.Next(100000, 1000000);
            user.Agency = random.Next(0, 100);
            user.Balance = random.Next(100, 1000000);
            user.DateRegistered = DateTime.Now;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return _mapper.Map<UserVO>(user);
        }

        public async Task<UserVO> Update(UserVO userVO, bool isAdmin)
        {
            User user = _mapper.Map<User>(userVO);
            var u = await _context.Users.Where(u => u.UserId == user.UserId).FirstOrDefaultAsync();
            if (!isAdmin)
            {
                user.AccountNumber = u.AccountNumber;
                user.Agency = u.Agency;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return _mapper.Map<UserVO>(user);
        }

        public async Task<bool> UserHasBalance(string userId, float quantity)
        {
            var u = await _context.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();
            if (u != null && quantity <= u.Balance ) return true;
            return false;
        }

        public async Task<UserVO> ModifyBalance(string userId, float quantity)
        {
            var u = await _context.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();
            if (u != null)
            {
                throw new Exception("User not found");
            }

            u.Balance += quantity;
            _context.Users.Update(u);
            await _context.SaveChangesAsync();
            return _mapper.Map<UserVO>(u);
        }
    }
}
