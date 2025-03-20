namespace BankMicroservices.Transfer.Integration
{
    public interface IClientConsumer
    {
        Task<bool> UserHasBalance(string userId, float amount);
        Task TransferBalance(string senderUserId, string receiverUserId, float amount);
    }
}
