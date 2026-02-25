using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;
using SITRA.Core.Mediator.Commands.Auths;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SITRA.Core.Mediator.Handlers.Auths
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, TokenResponseDTO?>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public LoginUserCommandHandler(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<TokenResponseDTO?> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Dto.Email);
            // Validation handled in DTO Validator
            if (user == null) return null; // Should not happen if validation passes, but kept for type safety or concurrency


            // ?? Lazy initialization for existing users with empty TenantId
            if (user.TenantId == Guid.Empty)
            {
                Console.WriteLine(Utilities.UserEmptyTenantIdMessage(user.Email!));
                user.TenantId = Guid.NewGuid();
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    Console.WriteLine(Utilities.FailedToUpdateUserTenantIdMessage(string.Join(", ", updateResult.Errors.Select(e => e.Description))));
                }
            }
            Console.WriteLine(Utilities.LoginUsingTenantIdMessage(user.TenantId));

            // Get Key using the helper to ensure consistency
            var keys = Keys.GetAllKeys(_configuration).ToList();
            var signingKey = keys.FirstOrDefault();

            if (signingKey == null)
            {
                // Fallback or error if no keys found
                throw new InvalidOperationException(Utilities.NoSigningKeysFoundMessage);
            }

            // Ensure the key is suitable for signing (SymmetricSecurityKey usually is)
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var issuer = _configuration["Authentication:Schemes:Bearer:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expire = DateTimeOffset.UtcNow.AddHours(12); // Standard expiry

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userName", user.UserName!),
                new Claim("tenantId", user.TenantId.ToString()), // Useful for multi-tenancy
                new Claim("language", user.Language.ToString())
            };

            // 1. Fetch User Claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            // 2. Fetch Roles and their Claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var roleName in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
                
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    claims.AddRange(roleClaims);
                }
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expire.DateTime,
                signingCredentials: creds
            );

            return new TokenResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expire = expire,
                IsAuthenticated = true
            };
        }
    }
}
