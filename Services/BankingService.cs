using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class BankingService
    {
        private readonly AppDbContext _context;
        private readonly IActivityLogger _activityLogger;

        public BankingService(AppDbContext context, IActivityLogger activityLogger)
        {
            _context = context;
            _activityLogger = activityLogger;
        }

        // ── Generate Account Number ──
        private string GenerateAccountNumber()
        {
            return "WIN" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString().Substring(5);
        }

        // ── Create Account for New User ──
        public async Task<Account> CreateAccountAsync(int userId)
        {
            var account = new Account
            {
                UserId = userId,
                AccountNumber = GenerateAccountNumber(),
                Balance = 1000,
                AccountType = "Savings",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        // ── Get Account by UserId ──
        public async Task<Account?> GetAccountByUserIdAsync(int userId)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId && a.IsActive);
        }

        // ── View Balance ──
        public async Task<AccountDto?> GetBalanceAsync(int userId)
        {
            var account = await GetAccountByUserIdAsync(userId);
            if (account == null) return null;

            return new AccountDto
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                AccountType = account.AccountType,
                CreatedAt = account.CreatedAt
            };
        }

        // ── Transaction History ──
        public async Task<List<TransactionDto>> GetTransactionHistoryAsync(int userId)
        {
            var account = await GetAccountByUserIdAsync(userId);
            if (account == null)
                return new List<TransactionDto>();

            var list = await _context.Transactions
                .Where(t => t.AccountId == account.Id || t.ToAccountId == account.Id)
                .ToListAsync();

            foreach (var t in list)
            {
                Console.WriteLine($"Transaction {t.Id}");
                Console.WriteLine($"Current Account = {account.Id}");
                Console.WriteLine($"Sender = {t.AccountId}");
                Console.WriteLine($"Receiver = {t.ToAccountId}");
            }

            var transactions = await _context.Transactions
       .Where(t => t.AccountId == account.Id || t.ToAccountId == account.Id)
       .OrderByDescending(t => t.CreatedAt)
       .Select(t => new TransactionDto
       {
           Id = t.Id,
           Amount = t.Amount,
           Type = t.Type,
           Status = t.Status,
           Description = t.Description,
           ReferenceNumber = t.ReferenceNumber,
           CreatedAt = t.CreatedAt,
           ToAccountNumber = t.ToAccount != null ? t.ToAccount.AccountNumber : null,
           Direction = t.AccountId == account.Id ? "Sent" : "Received"
       })
       .ToListAsync();

            foreach (var tx in transactions)
            {
                Console.WriteLine($"Direction = {tx.Direction}");
            }

            return transactions;
        }

        // ── Transfer ──
        public async Task<(bool Success, string Message)> TransferAsync(int userId, TransferRequest request)
        {
            if (request.Amount <= 0)
                return (false, "Amount must be greater than zero.");

            var senderAccount = await GetAccountByUserIdAsync(userId);
            if (senderAccount == null)
                return (false, "Your account was not found.");

            if (senderAccount.Balance < request.Amount)
                return (false, "Insufficient balance.");

            var receiverAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == request.ToAccountNumber && a.IsActive);
            if (receiverAccount == null)
                return (false, "Recipient account not found.");

            if (senderAccount.Id == receiverAccount.Id)
                return (false, "Cannot transfer to the same account.");

            // Business Rule: daily transfer limit 10,000
            var todayTransfers = await _context.Transactions
                .Where(t => t.AccountId == senderAccount.Id
                    && t.Type == "Transfer"
                    && t.CreatedAt.Date == DateTime.UtcNow.Date)
                .SumAsync(t => t.Amount);

            if (todayTransfers + request.Amount > 10000)
                return (false, "Daily transfer limit exceeded (10,000).");

            // Execute Transfer
            senderAccount.Balance -= request.Amount;
            receiverAccount.Balance += request.Amount;

            var transaction = new Transaction
            {
                AccountId = senderAccount.Id,
                ToAccountId = receiverAccount.Id,
                Amount = request.Amount,
                Type = "Transfer",
                Status = "Completed",
                Description = request.Description ?? "Bank Transfer",
                ReferenceNumber = "TXN" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            await _activityLogger.Log(userId, "Transfer", "Transaction", null, null, $"Transferred {request.Amount} to {request.ToAccountNumber}");

            return (true, "Transfer completed successfully.");
        }

        // ── Pay Bill ──
        public async Task<(bool Success, string Message)> PayBillAsync(int userId, BillPaymentRequest request)
        {
            if (request.Amount <= 0)
                return (false, "Amount must be greater than zero.");

            var account = await GetAccountByUserIdAsync(userId);
            if (account == null)
                return (false, "Your account was not found.");

            if (account.Balance < request.Amount)
                return (false, "Insufficient balance.");

            account.Balance -= request.Amount;

            var transaction = new Transaction
            {
                AccountId = account.Id,
                Amount = request.Amount,
                Type = "BillPayment",
                Status = "Completed",
                Description = $"Bill Payment - {request.BillType} - {request.BillNumber}",
                ReferenceNumber = "BILL" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            await _activityLogger.Log(userId, "BillPayment", "Transaction", null, null, $"Paid {request.BillType} bill, amount: {request.Amount}");

            return (true, "Bill paid successfully.");
        }

        // ── Beneficiaries ──
        public async Task<List<BeneficiaryDto>> GetBeneficiariesAsync(int userId)
        {
            return await _context.Beneficiaries
                .Where(b => b.UserId == userId)
                .Select(b => new BeneficiaryDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    AccountNumber = b.AccountNumber,
                    BankName = b.BankName
                })
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> AddBeneficiaryAsync(int userId, AddBeneficiaryRequest request)
        {
            var exists = await _context.Beneficiaries
                .AnyAsync(b => b.UserId == userId && b.AccountNumber == request.AccountNumber);
            if (exists)
                return (false, "Beneficiary already exists.");

            var beneficiary = new Beneficiary
            {
                UserId = userId,
                Name = request.Name,
                AccountNumber = request.AccountNumber,
                BankName = request.BankName,
                CreatedAt = DateTime.UtcNow
            };

            _context.Beneficiaries.Add(beneficiary);
            await _context.SaveChangesAsync();
            return (true, "Beneficiary added successfully.");
        }

        public async Task<(bool Success, string Message)> DeleteBeneficiaryAsync(int userId, int beneficiaryId)
        {
            var beneficiary = await _context.Beneficiaries
                .FirstOrDefaultAsync(b => b.Id == beneficiaryId && b.UserId == userId);
            if (beneficiary == null)
                return (false, "Beneficiary not found.");

            _context.Beneficiaries.Remove(beneficiary);
            await _context.SaveChangesAsync();
            return (true, "Beneficiary deleted successfully.");
        }

        // ── Profile Management ──
        public async Task<(bool Success, string Message)> UpdateProfileAsync(int userId, UpdateProfileRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return (false, "User not found.");

            if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber;
            if (request.Address != null) user.Address = request.Address;

            await _context.SaveChangesAsync();
            return (true, "Profile updated successfully.");
        }
    }
}