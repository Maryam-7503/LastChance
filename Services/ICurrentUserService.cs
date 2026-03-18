namespace WebApplication1.Services
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? Email { get; }
        string? Username { get; }
        string? Role { get; }
        bool IsAuthenticated { get; }
    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                return value == null ? null : int.Parse(value);
            }
        }

        public string? Email => _httpContextAccessor.HttpContext?
            .User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        public string? Username => _httpContextAccessor.HttpContext?
            .User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        public string? Role => _httpContextAccessor.HttpContext?
            .User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }
}