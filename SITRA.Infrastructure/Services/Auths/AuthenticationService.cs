using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using System.Security.Cryptography;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Interfaces.Services.Messaging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
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
using SITRA.Core.Exceptions;

namespace SITRA.Infrastructure.Services.Auths
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string PermissionClaimType = "Permission";

        public AuthenticationService(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            ILogger<AuthenticationService> logger,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TokenResponseDTO?> LoginAsync(CreateLoginUserDTO dto, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == dto.Email, cancellationToken);

            if (user == null) return null;

            bool isSuperAdmin = !string.IsNullOrWhiteSpace(user.RucNumber) && user.RucNumber.Trim() == "133556545";

            // 🔒 1. Check if user has been disabled by Admin or locked out
            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning($"Login rejected: User {user.Email} is locked out / disabled.");
                if (user.LockoutEnd.HasValue && user.LockoutEnd.Value >= DateTimeOffset.UtcNow.AddYears(10))
                {
                    throw new AccountLockedOutException("Su cuenta ha sido desactivada por el administrador. Por favor comuníquese con SISTRA SRL para reactivar su suscripción.");
                }

                throw new AccountLockedOutException("Su cuenta se encuentra bloqueada temporalmente. Por favor intente más tarde o comuníquese con el administrador.");
            }

            // 🔒 2. Check if Tenant membership / subscription has expired (except SuperAdmin)
            if (!isSuperAdmin && user.TenantId != Guid.Empty)
            {
                var tenant = await _context.Tenants
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(t => t.TenantId == user.TenantId, cancellationToken);

                if (tenant != null && tenant.SubscriptionEndDate.HasValue && tenant.SubscriptionEndDate.Value.Date < DateTime.UtcNow.Date)
                {
                    _logger.LogWarning($"Login rejected: User {user.Email} tenant subscription expired on {tenant.SubscriptionEndDate.Value:yyyy-MM-dd}.");
                    throw new AccountLockedOutException($"Su membresía de SISTRA ha expirado (venció el {tenant.SubscriptionEndDate.Value:dd/MM/yyyy}). Por favor comuníquese con el administrador para renovar su plan.");
                }
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
            {
                if (user.IsOwner && _httpContextAccessor.HttpContext != null)
                {
                    _httpContextAccessor.HttpContext.Items["IsOwnerLoginAttempt"] = true;
                }

                await _userManager.AccessFailedAsync(user);
                
                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning($"User {user.Email} has been locked out after multiple failed login attempts.");
                    throw new AccountLockedOutException("Su cuenta ha sido bloqueada temporalmente por múltiples intentos fallidos de inicio de sesión. Por favor intente más tarde.");
                }
                
                return null;
            }

            // Reset failed count on successful login
            await _userManager.ResetAccessFailedCountAsync(user);

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

            bool isElectronicTransmitter = false;
            string? companyLogoUrl = user.CompanyLogoUrl;
            if (user.IsOwner)
            {
                isElectronicTransmitter = user.IsElectronicTransmitter;
            }
            else
            {
                var ownerUser = await _context.Users
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.TenantId == user.TenantId && u.IsOwner, cancellationToken);
                isElectronicTransmitter = ownerUser?.IsElectronicTransmitter ?? false;
                if (string.IsNullOrEmpty(companyLogoUrl) && ownerUser != null)
                {
                    companyLogoUrl = ownerUser.CompanyLogoUrl;
                }
            }

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
                new Claim("rucNumber", user.RucNumber ?? string.Empty),
                new Claim("isElectronicTransmitter", isElectronicTransmitter.ToString().ToLower()),
                new Claim("companyLogoUrl", companyLogoUrl ?? string.Empty),
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
                    try
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
                    catch (DbUpdateException ex)
                    {
                        _logger.LogWarning(ex, "Concurrency or DB update exception while updating claims for user {Email}. Another request likely updated them simultaneously.", user.Email);
                        // We safely ignore this because the concurrent thread has successfully updated the claims.
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

            // Generate Refresh Token
            var refreshTokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                UserId = user.Id!,
                Expires = refreshTokenExpiration,
                Created = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new TokenResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expire = expire,
                IsAuthenticated = true,
                RefreshToken = refreshTokenString,
                RefreshTokenExpiration = refreshTokenExpiration
            };
        }

        public async Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
                
            if (user == null)
            {
                // Return true even if user doesn't exist to prevent email enumeration
                return true;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            // Generate a fake link for now. 
            // In a real app, this would be your frontend URL + /reset-password?token=...&email=...
            var resetLink = $"http://localhost:3000/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";
            
            var body = $"Please reset your SITRA POS password by clicking here: {resetLink}";
            await _emailService.SendEmailAsync(user.Email!, "SITRA POS - Password Reset", body, cancellationToken);
            
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
                
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

        public async Task<TokenResponseDTO?> RefreshTokenAsync(string expiredToken, string refreshToken, CancellationToken cancellationToken)
        {
            var principal = GetPrincipalFromExpiredToken(expiredToken);
            if (principal == null) return null;

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? principal.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId)) return null;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("Refresh token rejected: User {UserId} is locked out / disabled.", user.Id);
                return null;
            }

            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshToken && t.UserId == user.Id, cancellationToken);

            if (storedToken == null || !storedToken.IsActive)
            {
                if (storedToken != null && storedToken.IsRevoked)
                {
                    await RevokeTokenFamilyAsync(storedToken.UserId, cancellationToken);
                }
                return null;
            }

            var newRefreshTokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            storedToken.Revoked = DateTime.UtcNow;
            storedToken.ReplacedByToken = newRefreshTokenString;

            var newRefreshTokenExpiration = DateTime.UtcNow.AddDays(7);
            var newRefreshToken = new RefreshToken
            {
                Token = newRefreshTokenString,
                UserId = user.Id,
                Expires = newRefreshTokenExpiration,
                Created = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync(cancellationToken);

            var keys = Keys.GetAllKeys(_configuration).ToList();
            var signingKey = keys.FirstOrDefault()
                ?? throw new InvalidOperationException(Utilities.NoSigningKeysFoundMessage);

            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var issuer = _configuration["Authentication:Schemes:Bearer:Issuer"] 
                ?? _configuration["Authentication:Schemes:Bearer:ValidIssuer"] 
                ?? _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expire = DateTimeOffset.UtcNow.AddHours(12);

            var claims = principal.Claims.ToList();
            var jtiClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
            if (jtiClaim != null) claims.Remove(jtiClaim);
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

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
                IsAuthenticated = true,
                RefreshToken = newRefreshTokenString,
                RefreshTokenExpiration = newRefreshTokenExpiration
            };
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = Keys.GetAllKeys(_configuration),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating/parsing expired JWT token during refresh");
                return null;
            }
        }

        public async Task<TokenResponseDTO?> GenerateTokenForUserAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("GenerateTokenForUserAsync rejected: User {UserId} is locked out / disabled.", userId);
                return null;
            }

            var keys = Keys.GetAllKeys(_configuration).ToList();
            var signingKey = keys.FirstOrDefault()
                ?? throw new InvalidOperationException(Utilities.NoSigningKeysFoundMessage);

            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var issuer = _configuration["Authentication:Schemes:Bearer:Issuer"] 
                ?? _configuration["Authentication:Schemes:Bearer:ValidIssuer"] 
                ?? _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expire = DateTimeOffset.UtcNow.AddHours(12);

            bool isElectronicTransmitter = false;
            if (user.IsOwner)
            {
                isElectronicTransmitter = user.IsElectronicTransmitter;
            }
            else
            {
                var ownerUser = await _context.Users
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.TenantId == user.TenantId && u.IsOwner, cancellationToken);
                isElectronicTransmitter = ownerUser?.IsElectronicTransmitter ?? false;
            }

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
                new Claim("isElectronicTransmitter", isElectronicTransmitter.ToString().ToLower()),
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            var permissionClaims = userClaims.Where(c => c.Type == PermissionClaimType).ToList();
            claims.AddRange(permissionClaims);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expire.DateTime,
                signingCredentials: creds);

            // Generate Refresh Token
            var refreshTokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                UserId = user.Id!,
                Expires = refreshTokenExpiration,
                Created = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new TokenResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expire = expire,
                IsAuthenticated = true,
                RefreshToken = refreshTokenString,
                RefreshTokenExpiration = refreshTokenExpiration
            };
        }

        private async Task RevokeTokenFamilyAsync(string userId, CancellationToken cancellationToken)
        {
            _logger.LogWarning($"Potential refresh token reuse detected for user {userId}! Revoking all refresh tokens for this user.");
            var tokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId && t.Revoked == null)
                .ToListAsync(cancellationToken);

            foreach (var token in tokens)
            {
                token.Revoked = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
