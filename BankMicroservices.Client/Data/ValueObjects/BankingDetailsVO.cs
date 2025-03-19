namespace BankMicroservices.Client.Data.ValueObjects
{
    public class BankingDetailsVO
    {
        public required string Agency { get; set; }
        public required string AccountNumber { get; set; }
        public float Balance { get; set; }
    }
}
