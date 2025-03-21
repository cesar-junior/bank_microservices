namespace BankMicroservices.Client.Data.ValueObjects
{
    public class UserVO
    {
        public required string UserId { get; set; }
        public required string FullName { get; set; }
        public required string Address { get; set; }
        public required string Email { get; set; }
        public required string Gender { get; set; }
        public DateTime DateRegistered { get; set; }
        public required BankingDetailsVO BankingDetails { get; set; }
    }
}
