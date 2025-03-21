namespace BankMicroservices.Client.Data.ValueObjects
{
    public class CreateUserVO
    {
        public required string UserId { get; set; }
        public required string FullName { get; set; }
        public required string Address { get; set; }
        public required string Email { get; set; }
        public required string Gender { get; set; }
    }
}
