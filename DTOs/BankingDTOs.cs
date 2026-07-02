namespace WebApplication1.DTOs
{
    // Balance
    public class AccountDto
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string AccountType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // Transaction History
    public class TransactionDto
    {
        public int Id { get; set; }

        public int AccountId { get; set; }
        public int? ToAccountId { get; set; }

        public decimal Amount { get; set; }

        public string Type { get; set; } = string.Empty;

        public bool IsOutgoing { get; set; }   // أضف ده

        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? OtherAccountNumber { get; set; } // بدل ToAccountNumber
    }

    // Transfer
    public class TransferRequest
    {
        public string ToAccountNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }

    // Bill Payment
    public class BillPaymentRequest
    {
        public string BillType { get; set; } = string.Empty;
        public string BillNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    // Beneficiary
    public class BeneficiaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string? BankName { get; set; }
    }

    public class AddBeneficiaryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string? BankName { get; set; }
    }

    // Profile
    public class UpdateProfileRequest
    {
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}