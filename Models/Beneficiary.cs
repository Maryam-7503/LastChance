using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Beneficiary
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string AccountNumber { get; set; } = string.Empty;

        public string? BankName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}