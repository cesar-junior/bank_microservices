using BankMicroservices.Client.Data.ValueObjects;

namespace BankMicroservices.Client.Repository
{
    public interface IUserProfilePictureRepository
    {
        Task<UserProfilePictureVO> GetByUserId(string userId);
        Task<UserProfilePictureVO> Create(UserProfilePictureVO profilePictureVO);
        Task<UserProfilePictureVO> Update(UserProfilePictureVO profilePictureVO);

    }
}
