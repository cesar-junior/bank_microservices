using BankMicroservices.Email.Data.ValueObjects;
using BankMicroservices.Email.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankMicroservices.Email.Repository
{
    public interface IEmailRepository
    {
        Task<IEnumerable<EmailVO>> FindAll();
        Task LogEmail(NotificationMessage message);
    }
}
