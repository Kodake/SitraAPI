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
                    // Owner implicitly has the "Admin" role for their tenant
                    var adminRole = await context.TenantRoles
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(r => r.Name == "Admin" && r.TenantId == user.TenantId, cancellationToken);
                    
                    if (adminRole != null)
                    {
                        roleIds.Add(adminRole.Id);
                    }
                }
                else
                {
                    // For sub-users, get RoleIds from UserRoles join table
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

            // Tenant is saved via DbContext directly since we're in Infrastructure
            context.Set<Core.Entities.Tenants.Tenant>().Add(tenant);
            await context.SaveChangesAsync(cancellationToken);

            user.TenantId = tenant.TenantId;

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                // 1. Create Business Role "Admin"
                var adminRole = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = AdminRoleName,
                    Description = "ADMINISTRATOR ROLE WITH ALL PERMISSIONS",
                    IsSystemRole = true,
                    TenantId = tenant.TenantId
                };
                context.Set<Role>().Add(adminRole);

                // 2. Sync and Link Permissions
                var config = _configuration
                    .GetSection("PermissionManagement")
                    .Get<Core.DTOs.AdministrativeTools.PermissionManagementConfigDTO>();

                if (config != null)
                {
                    // Sync config permissions to DB first
                    var existingPermissions = await context.TenantPermissions.ToListAsync(cancellationToken);
                    var existingKeys = existingPermissions.Select(p => p.Key).ToHashSet();

                    foreach (var module in config.Modules)
                    {
                        foreach (var action in module.Actions)
                        {
                            var key = $"{module.Key}:{action.Name}";
                            if (!existingKeys.Contains(key))
                            {
                                if (existingPermissions.Any(p => p.Id == action.Id))
                                {
                                    continue;
                                }

                                var newPermission = new Permission
                                {
                                    Id = action.Id,
                                    Name = action.Name,
                                    Key = key,
                                    Section = module.Section
                                };
                                context.TenantPermissions.Add(newPermission);
                                existingPermissions.Add(newPermission); // Add to local list to link later
                            }
                        }
                    }

                    // Now link ALL permissions in the system to this new Admin role
                    // (This ensures Admin gets everything, even if something was in DB but not in current module config)
                    foreach (var permission in existingPermissions)
                    {
                        context.TenantRolePermissions.Add(new RolePermission
                        {
                            RoleId = adminRole.Id,
                            PermissionId = permission.Id
                        });
                    }

                    // 4. Associate User with Role in TenantRoleUsers
                    context.TenantRoleUsers.Add(new TenantRoleUser
                    {
                        RoleId = adminRole.Id,
                        UserId = user.Id
                    });
                }

                await context.SaveChangesAsync(cancellationToken);

                // 4. Assign Permission Claims immediately
                await AddPermissionClaimsFromRolesAsync(user, new[] { adminRole.Id }, cancellationToken);
            }

            return result;
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
                PhoneNumber = currentUser.PhoneNumber, // Or should it be dto.PhoneNumber? Old code said currentUser.PhoneNumber often but let's check current file.
                TenantId = currentUser.TenantId,
                ParentUserId = currentUser.Id,
                IsOwner = false
            };

            // Wait, checking line 294 in current file... it says dto.PhoneNumber. I'll stick with dto.
            subUser.PhoneNumber = dto.PhoneNumber;

            var result = await _userManager.CreateAsync(subUser, dto.Password);

            if (!result.Succeeded)
                return result;

            // 1. Resolve role
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(dto.RoleId, cancellationToken);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Role not found." });
            }

            // 2. Associate User with Role in TenantRoleUsers
            context.TenantRoleUsers.Add(new TenantRoleUser
            {
                RoleId = role.Id,
                UserId = subUser.Id
            });

            await context.SaveChangesAsync(cancellationToken);

            // 3. Assign Permission Claims immediately
            await AddPermissionClaimsFromRolesAsync(subUser, new[] { role.Id }, cancellationToken);

            return result;
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
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            // Add new role
            var role = await context.Set<Role>().FindAsync([dto.RoleId], cancellationToken);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Role not found." });
            }

            // Update TenantRoleUsers
            var existingAssociation = await context.TenantRoleUsers
                .FirstOrDefaultAsync(tru => tru.UserId == user.Id, cancellationToken);

            if (existingAssociation != null)
            {
                context.TenantRoleUsers.Remove(existingAssociation);
            }

            context.TenantRoleUsers.Add(new TenantRoleUser
            {
                RoleId = role.Id,
                UserId = user.Id
            });

            await context.SaveChangesAsync(cancellationToken);
            
            // Re-assign claims
            // Clear old permission claims first
            var currentClaims = await _userManager.GetClaimsAsync(user);
            var permissionClaims = currentClaims.Where(c => c.Type == PermissionClaimType).ToList();
            await _userManager.RemoveClaimsAsync(user, permissionClaims);

            await AddPermissionClaimsFromRolesAsync(user, new[] { role.Id }, cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<List<User>> GetAllAssociatedUsersAsync(CancellationToken cancellationToken)
        {
            var queryable = _userManager.Users
                .Include(u => u.TenantRoleUsers)
                .Where(u => u.ParentUserId != null);

            return await queryable
                .OrderBy(u => u.UserName)
                .ToListAsync(cancellationToken);
        }
    }
}
