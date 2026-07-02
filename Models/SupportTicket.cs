namespace WebApplication1.Models
{
    public class SupportTicket
    {
        public int Id { get; set; }

        public string Subject { get; set; } = "";

        public string Message { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}