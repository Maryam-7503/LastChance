using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BankingController : ControllerBase
    {
        private readonly BankingService _bankingService;

        public BankingController(BankingService bankingService)
        {
            _bankingService = bankingService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }

        // ── US-03: View Balance ──
        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            var userId = GetCurrentUserId();
            var account = await _bankingService.GetBalanceAsync(userId);
            if (account == null)
                return NotFound(new { message = "Account not found." });

            return Ok(account);
        }

        // ── US-05: Transaction History ──
        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions()
        {
            var userId = GetCurrentUserId();
            var transactions = await _bankingService.GetTransactionHistoryAsync(userId);
            return Ok(transactions);
        }

        // ── US-04: Transfer ──
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
        {
            var userId = GetCurrentUserId();
            var (success, message) = await _bankingService.TransferAsync(userId, request);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        // ── US-06: Pay Bill ──
        [HttpPost("pay-bill")]
        public async Task<IActionResult> PayBill([FromBody] BillPaymentRequest request)
        {
            var userId = GetCurrentUserId();
            var (success, message) = await _bankingService.PayBillAsync(userId, request);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        // ── Beneficiaries ──
        [HttpGet("beneficiaries")]
        public async Task<IActionResult> GetBeneficiaries()
        {
            var userId = GetCurrentUserId();
            var beneficiaries = await _bankingService.GetBeneficiariesAsync(userId);
            return Ok(beneficiaries);
        }

        [HttpPost("beneficiaries")]
        public async Task<IActionResult> AddBeneficiary([FromBody] AddBeneficiaryRequest request)
        {
            var userId = GetCurrentUserId();
            var (success, message) = await _bankingService.AddBeneficiaryAsync(userId, request);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpDelete("beneficiaries/{id}")]
        public async Task<IActionResult> DeleteBeneficiary(int id)
        {
            var userId = GetCurrentUserId();
            var (success, message) = await _bankingService.DeleteBeneficiaryAsync(userId, id);

            if (!success)
                return NotFound(new { message });

            return Ok(new { message });
        }

        // ── US-08: Profile Management ──
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = GetCurrentUserId();
            var (success, message) = await _bankingService.UpdateProfileAsync(userId, request);

            if (!success)
                return NotFound(new { message });

            return Ok(new { message });
        }
    }
}