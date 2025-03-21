using BankMicroservices.Log.Data.ValueObjects;
using BankMicroservices.Log.Messages;
using BankMicroservices.Log.Utils.Wrapper;

namespace BankMicroservices.Log.Repository
{
    public interface ILogRepository
    {
        Task<PagedResponse<List<LogVO>>> GetWithOffsetPagination(int pageNumber, int pageSize);
        Task SendNotification(LogMessage message);
    }
}
