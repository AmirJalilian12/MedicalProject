using Application.DTOs.Contracts;
using Web.Extentions;

namespace Web.Services
{
    public class CurrentService : ICurrentService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public CurrentService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public string UserId => _contextAccessor?.HttpContext?.User?.GetUserId() ?? String.Empty;
        public string PhoneNumber => _contextAccessor?.HttpContext?.User?.GetPhoneNumber() ?? String.Empty;
    }
}
