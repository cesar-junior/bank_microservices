namespace BankMicroservices.Transfer.Data.ValueObjects
{
    public class TransferVO
    {
        public long Id { get; set; }
        public required string SenderUserId { get; set; }
        public required string ReceiverUserId { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
        public DateTime SentDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public long Amount { get; set; }
    }
}
