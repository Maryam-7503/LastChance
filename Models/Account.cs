using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required]
        public string AccountNumber { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        public string AccountType { get; set; } = "Savings";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Navigation
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}