using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupportController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SupportController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("tickets")]
        public async Task<IActionResult> CreateTicket([FromBody] SupportRequest request)
        {
            var ticket = new SupportTicket
            {
                Subject = request.Subject,
                Message = request.Message
            };

            _context.SupportTickets.Add(ticket);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Support ticket submitted successfully." });
        }
    }

    public class SupportRequest
    {
        public string Subject { get; set; } = "";
        public string Message { get; set; } = "";
    }
}