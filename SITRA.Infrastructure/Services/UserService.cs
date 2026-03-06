using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Services;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;
using SITRA.Core.Entities.AdministrativeTools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SITRA.Core.Interfaces;

namespace SITRA.Infrastructure.Services
{
    public class UserService : RepositoryBase<User>, IUserService
    {
        private new readonly HttpContext httpContext;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;

        private const string PermissionClaimType = "Permission";
        private const string AdminRoleName = "ADMIN";

        public UserService(
            IUnitOfWork unitOfWork,
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager,
            IConfiguration configuration,
            ILogger<UserService> logger)
            : base(context, httpContextAccessor)
        {
            this.httpContext = httpContextAccessor.HttpContext!;
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<User?> GetUserAsync()
        {
            var emailClaim = httpContext.User.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Email || x.Type == "email");

            if (emailClaim is null)
                return null;

            return (User?)await _userManager.FindByEmailAsync(emailClaim.Value);
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

            var userClaims = await _userManager.GetClaimsAsync(user);
            var permissionClaims = userClaims.Where(c => c.Type == PermissionClaimType).ToList();

            if (!permissionClaims.Any())
            {
                permissionClaims = await RefreshUserPermissionClaimsAsync(user, cancellationToken);
            }

            var (token, expire) = GenerateJwtToken(user, permissionClaims);

            return new TokenResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expire = expire,
                IsAuthenticated = true
            };
        }

        private (JwtSecurityToken Token, DateTimeOffset Expiration) GenerateJwtToken(User user, IEnumerable<Claim> permissionClaims)
        {
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

            claims.AddRange(permissionClaims);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expire.DateTime,
                signingCredentials: creds);

            return (token, expire);
        }

        private async Task<List<Claim>> RefreshUserPermissionClaimsAsync(User user, CancellationToken cancellationToken)
        {
            var roleIds = new List<Guid>();

            if (user.IsOwner)
            {
                var adminRole = await context.TenantRoles
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(r => r.Name == "Admin" && r.TenantId == user.TenantId, cancellationToken);
                
                if (adminRole != null) roleIds.Add(adminRole.Id);
            }
            else
            {
                roleIds = await context.TenantRoleUsers
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

            var permissionClaims = permissionKeys
                .Distinct()
                .Select(pk => new Claim(PermissionClaimType, pk))
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

            return permissionClaims;
        }

        public async Task<IdentityResult> RegisterAsync(CreateRegistrationUserDTO dto, CancellationToken cancellationToken)
        {
            var user = new User
            {
                CompanyName = dto.CompanyName,
                RucNumber = dto.RucNumber,
                Address = dto.Address,
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                IsOwner = true,
            };

            var tenant = new Core.Entities.Tenants.Tenant
            {
                TenantId = Guid.NewGuid(),
                Email = dto.Email,
                Plan = dto.Plan
            };

            context.Set<Core.Entities.Tenants.Tenant>().Add(tenant);
            await context.SaveChangesAsync(cancellationToken);

            user.TenantId = tenant.TenantId;

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                var adminRole = await CreateAdminRoleAsync(tenant.TenantId, cancellationToken);
                await SyncPermissionsFromConfigAsync(adminRole.Id, cancellationToken);
                await AssociateUserWithRoleAsync(user.Id, adminRole.Id, AdminRoleName, cancellationToken);

                // Assign Permission Claims immediately
                await AddPermissionClaimsFromRolesAsync(user, new[] { adminRole.Id }, cancellationToken);
            }

            return result;
        }

        private async Task<Role> CreateAdminRoleAsync(Guid tenantId, CancellationToken cancellationToken)
        {
            var adminRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = AdminRoleName,
                Description = "ADMINISTRATOR ROLE WITH ALL PERMISSIONS",
                IsSystemRole = true,
                TenantId = tenantId
            };
            context.Set<Role>().Add(adminRole);
            await context.SaveChangesAsync(cancellationToken);
            return adminRole;
        }

        private async Task SyncPermissionsFromConfigAsync(Guid adminRoleId, CancellationToken cancellationToken)
        {
            var config = _configuration
                .GetSection("PermissionManagement")
                .Get<Core.DTOs.AdministrativeTools.PermissionManagementConfigDTO>();

            if (config == null) return;

            var existingPermissions = await context.TenantPermissions.ToListAsync(cancellationToken);
            var existingKeys = existingPermissions.Select(p => p.Key).ToHashSet();

            foreach (var module in config.Modules)
            {
                foreach (var action in module.Actions)
                {
                    var key = $"{module.Key}:{action.Name}";
                    if (!existingKeys.Contains(key))
                    {
                        if (existingPermissions.Any(p => p.Id == action.Id)) continue;

                        var newPermission = new Permission
                        {
                            Id = action.Id,
                            Name = action.Name,
                            Key = key,
                            Section = module.Section
                        };
                        context.TenantPermissions.Add(newPermission);
                        existingPermissions.Add(newPermission);
                    }
                }
            }

            foreach (var permission in existingPermissions)
            {
                context.TenantRolePermissions.Add(new RolePermission
                {
                    RoleId = adminRoleId,
                    PermissionId = permission.Id
                });
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IdentityResult> RegisterAssociatedUserAsync(CreateAssociatedUserDTO dto, CancellationToken cancellationToken)
        {
            var currentUser = await GetUserAsync();
            if (currentUser == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Current user not found." });
            }

            var subUser = new User
            {
                CompanyName = currentUser.CompanyName,
                RucNumber = currentUser.RucNumber,
                Address = currentUser.Address,
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                TenantId = currentUser.TenantId,
                ParentUserId = currentUser.Id,
                IsOwner = false
            };

            var result = await _userManager.CreateAsync(subUser, dto.Password);

            if (!result.Succeeded) return result;

            var role = await _unitOfWork.RoleRepository.GetByIdAsync(dto.RoleId, cancellationToken);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Role not found." });
            }

            // Associate User with Role
            await AssociateUserWithRoleAsync(subUser.Id, role.Id, role.Name, cancellationToken);

            // Assign Permission Claims immediately
            await AddPermissionClaimsFromRolesAsync(subUser, new[] { role.Id }, cancellationToken);

            return result;
        }

        private async Task AssociateUserWithRoleAsync(string userId, Guid roleId, string roleName, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return;

            // 1. Identity Role (Ensures standard AddToRoleAsync is called)
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(roleName))
            {
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                }
                await _userManager.AddToRoleAsync(user, roleName);
            }

            // 2. TenantRoleUsers Association
            var existingAssociation = await context.TenantRoleUsers
                .FirstOrDefaultAsync(tru => tru.UserId == userId, cancellationToken);

            if (existingAssociation != null)
            {
                context.TenantRoleUsers.Remove(existingAssociation);
            }

            context.TenantRoleUsers.Add(new TenantRoleUser
            {
                RoleId = roleId,
                UserId = userId
            });

            await context.SaveChangesAsync(cancellationToken);
        }

        private async Task AddPermissionClaimsFromRolesAsync(User user, IEnumerable<Guid> roleIds, CancellationToken cancellationToken)
        {
            var permissionKeys = new List<string>();
            foreach (var roleId in roleIds)
            {
                var rolePermissions = await _unitOfWork.RolePermissionRepository
                    .GetPermissionsByRoleIdAsync(roleId, cancellationToken);
                
                permissionKeys.AddRange(rolePermissions.Select(p => p.Key));
            }

            var permissionClaims = permissionKeys
                .Distinct()
                .Select(pk => new Claim(PermissionClaimType, pk))
                .ToList();

            if (permissionClaims.Any())
            {
                var result = await _userManager.AddClaimsAsync(user, permissionClaims);
                if (!result.Succeeded)
                {
                    _logger.LogError(Utilities.FailedToUpdateUserClaimsMessage(
                        string.Join(", ", result.Errors.Select(e => e.Description))));
                }
            }
        }

        public async Task<bool> ChangeLanguageAsync(Core.Enums.Language language, CancellationToken cancellationToken)
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            user.Language = language;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<IdentityResult> UpdateUserRoleAsync(string userId, UpdateUserRoleDTO dto, CancellationToken cancellationToken)
        {
            var role = await context.Set<Role>().FindAsync(new object[] { dto.RoleId }, cancellationToken);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Role not found." });
            }

            await AssociateUserWithRoleAsync(userId, role.Id, role.Name, cancellationToken);
            
            // Re-assign claims
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Clear old permission claims first
                var currentClaims = await _userManager.GetClaimsAsync(user);
                var permissionClaims = currentClaims.Where(c => c.Type == PermissionClaimType);
                await _userManager.RemoveClaimsAsync(user, permissionClaims);

                await AddPermissionClaimsFromRolesAsync(user, new[] { role.Id }, cancellationToken);
            }

            return IdentityResult.Success;
        }
    }
}
