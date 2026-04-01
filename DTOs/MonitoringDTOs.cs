namespace WebApplication1.DTOs
{
    public class AdjustBalanceRequest
    {
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}