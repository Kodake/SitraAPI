using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;
using SITRA.Core.Mediator.Commands.Auths;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SITRA.Core.Mediator.Handlers.Auths
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, TokenResponseDTO?>
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public LoginUserCommandHandler(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<TokenResponseDTO?> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Dto.Email!);

            if (user is null)
            {
                return null;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user,
                request.Dto.Password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await BuildToken(request.Dto, _configuration, _userManager, user);
            }

            return null;
        }

        private async Task<TokenResponseDTO> BuildToken(
            CreateLoginUserDTO dto,
            IConfiguration configuration,
            UserManager<User> userManager,
            User user)
        {
            var claims = new List<Claim>
            {
                new Claim("email", user.Email ?? string.Empty),
                new Claim("userName", user.UserName ?? user.Email ?? "unknown")
            };

            void AddIfNotNull(string type, string? value)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    claims.Add(new Claim(type, value));
            }

            AddIfNotNull("companyName", user.CompanyName);
            AddIfNotNull("rucNumber", user.RucNumber);
            AddIfNotNull("address", user.Address);
            AddIfNotNull("tenantId", user.TenantId.ToString());

            var claimsDb = await userManager.GetClaimsAsync(user);
            claims.AddRange(claimsDb.Where(c => !string.IsNullOrWhiteSpace(c.Value)));

            var allKeys = Keys.GetAllKeys(configuration);
            if (!allKeys.Any())
            {
                throw new InvalidOperationException("No signing keys found in configuration.");
            }
            var signingKey = allKeys.First();
            var issuer = configuration["Authentication:Schemes:Bearer:Issuer"];
            var audience = configuration["Jwt:Audience"];

            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddYears(1);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new TokenResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expire = expiration,
                IsAuthenticated = true
            };
        }
    }
}
