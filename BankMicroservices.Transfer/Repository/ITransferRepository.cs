using BankMicroservices.Transfer.Data.ValueObjects;

namespace BankMicroservices.Transfer.Repository
{
    public interface ITransferRepository
    {
        Task<TransferVO> GetTransferById(long id);
        Task<List<TransferVO>> GetTransfersByUser(string userId);
        Task<TransferVO> Create(SendTransferVO vo, string token, string userEmail);
        Task<bool> ReturnTransfer(long id, string userId, string token);
    }
}
