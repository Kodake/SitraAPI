using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services;
using SITRA.Infrastructure.Persistence;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SITRA.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionClaimService _permissionClaimService;
        private readonly ILogger<AuthenticationService> _logger;

        private const string PermissionClaimType = "Permission";

        public AuthenticationService(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IPermissionClaimService permissionClaimService,
            ILogger<AuthenticationService> logger)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _permissionClaimService = permissionClaimService;
            _logger = logger;
        }

        public async Task<TokenResponseDTO?> LoginAsync(CreateLoginUserDTO dto, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == dto.Email, cancellationToken);

            if (user == null) return null;

            if (user.TenantId == Guid.Empty)
            {
                _logger.LogWarning(Utilities.UserEmptyTenantIdMessage(user.Email!));
                user.TenantId = Guid.NewGuid();
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    _logger.LogError(Utilities.FailedToUpdateUserTenantIdMessage(
                        string.Join(", ", updateResult.Errors.Select(e => e.Description))));
                }
            }

            _logger.LogInformation(Utilities.LoginUsingTenantIdMessage(user.TenantId));

            var keys = Keys.GetAllKeys(_configuration).ToList();
            var signingKey = keys.FirstOrDefault()
                ?? throw new InvalidOperationException(Utilities.NoSigningKeysFoundMessage);

            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var issuer = _configuration["Authentication:Schemes:Bearer:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expire = DateTimeOffset.UtcNow.AddHours(12);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userName", user.UserName!),
                new Claim("tenantId", user.TenantId.ToString()),
                new Claim("language", user.Language.ToString())
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            var permissionClaims = userClaims.Where(c => c.Type == PermissionClaimType).ToList();

            if (!permissionClaims.Any())
            {
                var roleIds = new List<Guid>();

                if (user.IsOwner)
                {
                    var adminRole = await _context.TenantRoles
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(r => r.Name == "Admin" && r.TenantId == user.TenantId, cancellationToken);

                    if (adminRole != null)
                    {
                        roleIds.Add(adminRole.Id);
                    }
                }
                else
                {
                    roleIds = await _context.TenantRoleUsers
                        .Where(ur => ur.UserId == user.Id)
                        .Select(ur => ur.RoleId)
                        .ToListAsync(cancellationToken);
                }

                var permissionKeys = new List<string>();
                foreach (var roleId in roleIds)
                {
                    var rolePermissions = await _unitOfWork.RolePermissionRepository
                        .GetPermissionsByRoleIdAsync(roleId, cancellationToken);

                    permissionKeys.AddRange(rolePermissions.Select(p => p.Key));
                }

                permissionClaims = permissionKeys
                    .Distinct()
                    .Select(pk => new Claim(PermissionClaimType, pk))
                    .ToList();

                var addClaimsResult = await _userManager.AddClaimsAsync(user, permissionClaims);

                if (!addClaimsResult.Succeeded)
                {
                    _logger.LogError(Utilities.FailedToUpdateUserClaimsMessage(
                        string.Join(", ", addClaimsResult.Errors.Select(e => e.Description))));
                }
            }

            claims.AddRange(permissionClaims);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expire.DateTime,
                signingCredentials: creds);

            return new TokenResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expire = expire,
                IsAuthenticated = true
            };
        }
    }
}
