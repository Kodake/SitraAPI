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
        private readonly ILogger<AuthenticationService> _logger;

        private const string PermissionClaimType = "Permission";

        public AuthenticationService(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            ILogger<AuthenticationService> logger)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
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
            var issuer = _configuration["Authentication:Schemes:Bearer:Issuer"] 
                ?? _configuration["Authentication:Schemes:Bearer:ValidIssuer"] 
                ?? _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expire = DateTimeOffset.UtcNow.AddHours(12);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id?.ToString() ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userName", user.UserName ?? string.Empty),
                new Claim("tenantId", user.TenantId.ToString()),
                new Claim("language", user.Language.ToString()),
                new Claim("email", user.Email ?? string.Empty),
                new Claim("address", user.Address ?? string.Empty),
                new Claim("province", user.Province ?? string.Empty),
                new Claim("municipality", user.Municipality ?? string.Empty),
                new Claim("companyName", user.CompanyName ?? string.Empty),
                new Claim("phoneNumber", user.PhoneNumber ?? string.Empty),
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            var permissionClaims = userClaims.Where(c => c.Type == PermissionClaimType).ToList();

            bool isAssociatedUser = !string.IsNullOrEmpty(user.ParentUserId);

            // Re-evaluate permissions if the user has NO permission claims, OR if they are an associated user 
            // because associated users' role permissions might have changed.
            if (!permissionClaims.Any() || isAssociatedUser)
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

                var distinctPermissionKeys = permissionKeys.Distinct().ToList();
                var currentPermissionKeys = permissionClaims.Select(c => c.Value ?? string.Empty).ToList();

                // Only make changes to the database claims if there is a mismatch
                bool claimsChanged = distinctPermissionKeys.Count != currentPermissionKeys.Count ||
                                     distinctPermissionKeys.Except(currentPermissionKeys).Any();

                if (claimsChanged)
                {
                    if (permissionClaims.Any())
                    {
                        var removeResult = await _userManager.RemoveClaimsAsync(user, permissionClaims);
                        if (!removeResult.Succeeded)
                        {
                            _logger.LogError(Utilities.FailedToUpdateUserClaimsMessage(
                                string.Join(", ", removeResult.Errors.Select(e => e.Description))));
                        }
                    }

                    permissionClaims = distinctPermissionKeys
                        .Select(pk => new Claim(PermissionClaimType, pk ?? string.Empty))
                        .ToList();

                    if (permissionClaims.Any())
                    {
                        var addClaimsResult = await _userManager.AddClaimsAsync(user, permissionClaims);

                        if (!addClaimsResult.Succeeded)
                        {
                            _logger.LogError(Utilities.FailedToUpdateUserClaimsMessage(
                                string.Join(", ", addClaimsResult.Errors.Select(e => e.Description))));
                        }
                    }
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
