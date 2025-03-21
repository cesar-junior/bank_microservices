using AutoMapper;
using BankMicroservices.Client.Data.ValueObjects;
using BankMicroservices.Client.Model;
using BankMicroservices.Client.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace BankMicroservices.Client.Repository
{
    public class UserProfilePictureRepository : IUserProfilePictureRepository
    {
        private readonly MySQLContext _context;
        private IMapper _mapper;

        public UserProfilePictureRepository(DbContextOptions<MySQLContext> context, IMapper mapper)
        {
            _context = new MySQLContext(context);
            _mapper = mapper;
        }

        async Task<UserProfilePictureVO> IUserProfilePictureRepository.GetByUserId(string userId)
        {
            var profilePicture = await _context.ProfilePictures.Where(u => u.UserId == userId).FirstOrDefaultAsync();
            return _mapper.Map<UserProfilePictureVO>(profilePicture);
        }

        public async Task<UserProfilePictureVO> Create(UserProfilePictureVO profilePictureVO)
        {
            UserProfilePicture profilePicture = _mapper.Map<UserProfilePicture>(profilePictureVO);
            _context.ProfilePictures.Add(profilePicture);
            await _context.SaveChangesAsync();
            return _mapper.Map<UserProfilePictureVO>(profilePicture);
        }

        public async Task<UserProfilePictureVO> Update(UserProfilePictureVO profilePictureVO)
        {
            UserProfilePicture profilePicture = _mapper.Map<UserProfilePicture>(profilePictureVO);
            var profilePictureId = (await _context.ProfilePictures.Where(u => u.UserId == profilePicture.UserId).FirstOrDefaultAsync())?.Id;
            if(profilePictureId == null)
                throw new Exception("User not found");

            profilePicture.Id = (long)profilePictureId;
            _context.ProfilePictures.Update(profilePicture);
            await _context.SaveChangesAsync();
            return _mapper.Map<UserProfilePictureVO>(profilePicture);
        }
    }
}
