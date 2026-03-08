using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services;
using SITRA.Infrastructure.Persistence;

namespace SITRA.Infrastructure.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ILogger<UserRegistrationService> _logger;

        private const string AdminRoleName = "ADMIN";

        public UserRegistrationService(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IUserService userService,
            ILogger<UserRegistrationService> logger)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _logger = logger;
        }

        public async Task<RegistrationDTO> RegisterAsync(CreateRegistrationUserDTO dto, CancellationToken cancellationToken)
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

            _context.Set<Core.Entities.Tenants.Tenant>().Add(tenant);
            await _context.SaveChangesAsync(cancellationToken);

            user.TenantId = tenant.TenantId;

            var result = await _userManager.CreateAsync(user, dto.Password);

            Guid? adminRoleId = null;

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
                _context.Set<Role>().Add(adminRole);
                adminRoleId = adminRole.Id;

                // 2. Sync and Link Permissions
                var config = _configuration
                    .GetSection("PermissionManagement")
                    .Get<Core.DTOs.AdministrativeTools.PermissionManagementConfigDTO>();

                if (config != null)
                {
                    var existingPermissions = await _context.TenantPermissions.ToListAsync(cancellationToken);
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
                                _context.TenantPermissions.Add(newPermission);
                                existingPermissions.Add(newPermission);
                            }
                        }
                    }

                    foreach (var permission in existingPermissions)
                    {
                        _context.TenantRolePermissions.Add(new RolePermission
                        {
                            RoleId = adminRole.Id,
                            PermissionId = permission.Id
                        });
                    }

                    _context.TenantRoleUsers.Add(new TenantRoleUser
                    {
                        RoleId = adminRole.Id,
                        UserId = user.Id
                    });
                }

                await _context.SaveChangesAsync(cancellationToken);
            }

            return new RegistrationDTO
            {
                IdentityResult = result,
                UserId = result.Succeeded ? user.Id : null,
                AdminRoleId = adminRoleId
            };
        }

        public async Task<RegistrationDTO> RegisterAssociatedUserAsync(CreateAssociatedUserDTO dto, CancellationToken cancellationToken)
        {
            var currentUser = await _userService.GetUserAsync();
            if (currentUser == null)
            {
                return new RegistrationDTO
                {
                    IdentityResult = IdentityResult.Failed(new IdentityError { Description = "Current user not found." })
                };
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

            if (!result.Succeeded)
                return new RegistrationDTO { IdentityResult = result };

            // 1. Resolve role
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(dto.RoleId, cancellationToken);
            if (role == null)
            {
                return new RegistrationDTO
                {
                    IdentityResult = IdentityResult.Failed(new IdentityError { Description = "Role not found." })
                };
            }

            // 2. Associate User with Role in TenantRoleUsers
            _context.TenantRoleUsers.Add(new TenantRoleUser
            {
                RoleId = role.Id,
                UserId = subUser.Id
            });

            await _context.SaveChangesAsync(cancellationToken);

            return new RegistrationDTO
            {
                IdentityResult = result,
                UserId = subUser.Id,
                AdminRoleId = role.Id
            };
        }
    }
}
