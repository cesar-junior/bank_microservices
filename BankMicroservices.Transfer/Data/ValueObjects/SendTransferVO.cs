namespace BankMicroservices.Transfer.Data.ValueObjects
{
    public class SendTransferVO
    {
        public required string SenderUserId { get; set; }
        public required string ReceiverUserId { get; set; }
        public string? Description { get; set; }
        public float Amount { get; set; }
    }
}
