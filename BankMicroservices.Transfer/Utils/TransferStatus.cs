namespace BankMicroservices.Transfer.Utils
{
    public static class TransferStatus
    {
        public const int Processing = 0;
        public const int Success = 1;
        public const int Returned = 2;
        public const int Cancelled = 3;
    }
}