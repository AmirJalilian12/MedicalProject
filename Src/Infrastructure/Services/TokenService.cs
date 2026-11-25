using Application.Interfaces;
using Domain.Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration configuration, UserManager<User> userManager, SymmetricSecurityKey key)
        {
            _configuration = configuration;
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTConfiguration:Key"]));
        }
        public async Task<string> CreateToken(User user)
        {
            if (user.PhoneNumber == null)
                return null;
            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.GivenName,user.DisplayName ?? ""),
                new (JwtRegisteredClaimNames.NameId , user.Id ?? ""),
                new ("PhoneNumber",user.PhoneNumber??"")
            };
            var roles = await _userManager.GetRolesAsync(user);
            if (roles != null && roles.Any())
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            var cred = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptior = new SecurityTokenDescriptor()
            {
                Issuer = _configuration["JWTConfiguration:Issuer"],
                Audience = _configuration["JWTConfiguration:Audience"],
                IssuedAt = DateTime.Now,
                Expires = DateTime.UtcNow.AddDays(10),
                SigningCredentials = cred,
                Subject = new ClaimsIdentity(claims)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptior);
            return await Task.Run(() => tokenHandler.WriteToken(token)).ConfigureAwait(false);
        }
    }

}
