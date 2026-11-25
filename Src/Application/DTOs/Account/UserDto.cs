using Domain.Entities.IdentityEntities;

namespace Application.DTOs.Account
{
    public class UserDto
    {
        public string Email { get; set; } = string.Empty;
        public string? UserName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NationalCode { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }
}
