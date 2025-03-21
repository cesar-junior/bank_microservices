namespace BankMicroservices.Log.Data.ValueObjects
{
    public class LogVO
    {
        public long Id { get; set; }
        public required string Type { get; set; }
        public required string Message { get; set; }
        public DateTime Date { get; set; }
    }
}
