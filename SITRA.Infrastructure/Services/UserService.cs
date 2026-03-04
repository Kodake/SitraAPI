using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Services;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SITRA.Infrastructure.Services
{
    public class UserService : RepositoryBase<User>, IUserService
    {
        private new readonly HttpContext httpContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public UserService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
            : base(context, httpContextAccessor)
        {
            this.httpContext = httpContextAccessor.HttpContext!;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
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
                Console.WriteLine(Utilities.UserEmptyTenantIdMessage(user.Email!));
                user.TenantId = Guid.NewGuid();
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    Console.WriteLine(Utilities.FailedToUpdateUserTenantIdMessage(
                        string.Join(", ", updateResult.Errors.Select(e => e.Description))));
            }
            Console.WriteLine(Utilities.LoginUsingTenantIdMessage(user.TenantId));

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
            claims.AddRange(userClaims);

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
                UserName = dto.Email,
                Email = dto.Email,
                IsOwner = true,
            };

            var tenant = new Core.Entities.Tenants.Tenant
            {
                TenantId = Guid.NewGuid(),
                Name = dto.CompanyName,
                Plan = dto.Plan
            };

            // Tenant is saved via DbContext directly since we're in Infrastructure
            context.Set<Core.Entities.Tenants.Tenant>().Add(tenant);
            await context.SaveChangesAsync(cancellationToken);

            user.TenantId = tenant.TenantId;

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                var config = _configuration
                    .GetSection("PermissionManagement")
                    .Get<Core.DTOs.AdministrativeTools.PermissionManagementConfigDTO>();

                if (config != null)
                    foreach (var module in config.Modules)
                        foreach (var action in module.Actions)
                            await _userManager.AddClaimAsync(user, new Claim("Permission", $"{module.Key}:{action.Name}"));

                await _userManager.AddClaimAsync(user, new Claim("Permission", "*:*"));
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
                PhoneNumber = dto.PhoneNumber,
                TenantId = currentUser.TenantId,
                ParentUserId = currentUser.Id,
                IsOwner = false
            };

            var result = await _userManager.CreateAsync(subUser, dto.Password);

            if (result.Succeeded)
            {
                var role = await _roleManager.FindByIdAsync(dto.RoleId.ToString());
                if (role != null && role.Name != null)
                {
                    await _userManager.AddToRoleAsync(subUser, role.Name);
                    
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var claim in roleClaims)
                    {
                        await _userManager.AddClaimAsync(subUser, claim);
                    }
                }
            }

            return result;
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
    }
}
