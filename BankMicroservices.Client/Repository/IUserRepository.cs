using BankMicroservices.Client.Data.ValueObjects;

namespace BankMicroservices.Client.Repository
{
    public interface IUserRepository
    {
        Task<List<UserVO>> GetByNameOrEmail(string name, string email);
        Task<UserVO> GetByUserId(string userId);
        Task<bool> UserHasBalance(string userId, float quantity);
        Task<UserVO> TransferBalance(string senderUserId, string receiverUserId, float quantity);
        Task<UserVO> Create(UserVO user);
        Task<UserVO> Update(UserVO user, bool isAdmin);
    }
}
