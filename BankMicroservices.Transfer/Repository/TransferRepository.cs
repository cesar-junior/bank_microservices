using AutoMapper;
using BankMicroservices.Transfer.Data.ValueObjects;
using BankMicroservices.Transfer.Integration;
using BankMicroservices.Transfer.Messages;
using BankMicroservices.Transfer.Model;
using BankMicroservices.Transfer.Model.Context;
using BankMicroservices.Transfer.RabbitMQSender;
using BankMicroservices.Transfer.Utils;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;

namespace BankMicroservices.Transfer.Repository
{
    public class TransferRepository : ITransferRepository
    {
        private readonly MySQLContext _context;
        private IMapper _mapper;
        private IRabbitMQMessageSender _rabbitMQMessageSender;
        private IRabbitMQMessageSender _rabbitMQLogSender;

        public TransferRepository(MySQLContext context, IMapper mapper, [FromKeyedServices("Notification")] IRabbitMQMessageSender rabbitMQMessageSender, [FromKeyedServices("Log")] IRabbitMQMessageSender rabbitMQLogSender)
        {
            _context = context;
            _mapper = mapper;
            _rabbitMQMessageSender = rabbitMQMessageSender;
            _rabbitMQLogSender = rabbitMQLogSender;
        }

        public async Task<TransferVO> Create(SendTransferVO vo, string token, string userEmail)
        {
            TransferModel transfer = new TransferModel
            {
                SenderUserId = vo.SenderUserId,
                ReceiverUserId = vo.ReceiverUserId,
                Description = vo.Description,
                Amount = vo.Amount,
                SentDate = DateTime.Now,
                Status = TransferStatus.Processing
            };
            _context.Transfers.Add(transfer);
            await _context.SaveChangesAsync();

            var notificationMessage = new NotificationMessage
            {
                UserId = vo.SenderUserId,
                Email = userEmail
            };

            try
            {
                ClientConsumer clientConsumer = new ClientConsumer(token);
                if(await clientConsumer.UserHasBalance(vo.SenderUserId, vo.Amount))
                {
                    await clientConsumer.TransferBalance(vo.SenderUserId, vo.ReceiverUserId, vo.Amount);

                    transfer.Status = TransferStatus.Success;
                    _context.Transfers.Update(transfer);
                    await _context.SaveChangesAsync();

                    notificationMessage.Title = "Your transfer was completed.";
                    notificationMessage.Message = "Transfer completed with success!";
                    _rabbitMQMessageSender.SendMessage(notificationMessage);

                } else
                {
                    notificationMessage.Title = "Your transfer was not completed.";
                    notificationMessage.Message = "You don't have enough funds to complete this transfer.";
                    _rabbitMQMessageSender.SendMessage(notificationMessage);
                    
                    throw new Exception($"User {transfer.SenderUserId} does not have sufficient balance to make this transfer. TransferId: {transfer.Id}. Transfer Amount {transfer.Amount}");
                }
            }
            catch (HttpRequestException e) {
                
                transfer.Status = TransferStatus.Cancelled;
                _context.Transfers.Update(transfer);
                await _context.SaveChangesAsync();

                var logMessage = new LogMessage
                {
                    Type = "Error",
                    Message = $"User {transfer.SenderUserId} could not complete a transfer. TransferId: {transfer.Id}. Error message: {e.Message}"
                };
                _rabbitMQLogSender.SendMessage(logMessage);

                throw new Exception("Transfer could not be completed.");
            }
            catch (Exception e) {
            
                transfer.Status = TransferStatus.Cancelled;
                _context.Transfers.Update(transfer);
                await _context.SaveChangesAsync();

                var logMessage = new LogMessage
                {
                    Type = "Error",
                    Message = $"User {transfer.SenderUserId} could not complete a transfer. TransferId: {transfer.Id}. Error message: {e.Message}"
                };
                _rabbitMQLogSender.SendMessage(logMessage);

                throw new Exception("Transfer could not be completed.");
            }

            return _mapper.Map<TransferVO>(transfer);
        }

        public async Task<TransferVO> GetTransferById(long id)
        {
            var transfer = await _context.Transfers.Where(u => u.Id == id).FirstOrDefaultAsync();

            return _mapper.Map<TransferVO>(transfer);
        }

        public async Task<List<TransferVO>> GetTransfersByUser(string userId)
        {
            var transfers = await _context.Transfers.Where(u => u.SenderUserId == userId ||
            (u.ReceiverUserId == userId && (u.Status != TransferStatus.Processing && u.Status != TransferStatus.Cancelled))).ToListAsync();

            return _mapper.Map<List<TransferVO>>(transfers);
        }

        public async Task<bool> ReturnTransfer(long id, string userId, string token)
        {
            try
            {
                var transfer = await _context.Transfers.Where(u => u.Id == id).FirstOrDefaultAsync();
                if (transfer == null || transfer.SenderUserId != userId) return false;

                ClientConsumer clientConsumer = new ClientConsumer(token);
                if (await clientConsumer.UserHasBalance(transfer.ReceiverUserId, transfer.Amount))
                {
                    await clientConsumer.TransferBalance(transfer.ReceiverUserId, transfer.SenderUserId, transfer.Amount);

                    transfer.Status = TransferStatus.Returned;
                    _context.Transfers.Update(transfer);
                    await _context.SaveChangesAsync();

                    var notificationMessage = new NotificationMessage
                    {
                        UserId = transfer.ReceiverUserId,
                        Title = "Your transfer was returned.",
                        Message = "Transfer returned with success!"
                    };
                    _rabbitMQMessageSender.SendMessage(notificationMessage);

                }
                else
                {
                    var logMessage = new LogMessage
                    {
                        Type = "Warning",
                        Message = $"User {transfer.SenderUserId} does not have sufficient balance to return this transfer. TransferId: {transfer.Id}. Transfer Amount {transfer.Amount}"
                    };
                    _rabbitMQLogSender.SendMessage(logMessage);

                    return false;
                }

                return true;
            } catch (Exception e) {
                var logMessage = new LogMessage
                {
                    Type = "Error",
                    Message = e.Message
                };
                _rabbitMQLogSender.SendMessage(logMessage);

                return false;
            }
        }
    }
}
