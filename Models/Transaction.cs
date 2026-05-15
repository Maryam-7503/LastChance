using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string Type { get; set; } = string.Empty;
        // "Transfer", "BillPayment", "Deposit", "Withdrawal"

        public string Status { get; set; } = "Completed";

        public string? Description { get; set; }

        public string? ReferenceNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // From Account
        public int AccountId { get; set; }
        public Account Account { get; set; } = null!;

        // To Account (nullable - مش كل المعاملات ليها حساب مستلم)
        public int? ToAccountId { get; set; }
        public Account? ToAccount { get; set; }
    }
}