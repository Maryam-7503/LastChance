namespace WebApplication1.Models
{
    public class Token
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}