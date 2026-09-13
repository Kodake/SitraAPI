using SITRA.Core.Interfaces.Services.Auths;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Interfaces;
using SITRA.Infrastructure.Persistence;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Entities.Finances;
using SITRA.Core.DTOs.AdministrativeTools.Roles;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Entities.Contacts;

namespace SITRA.Infrastructure.Services.Auths
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
                Province = dto.Province,
                Municipality = dto.Municipality,
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

            // Create default tax records for the tenant
            var taxes = new[]
            {
                new Tax
                {
                    Id = Guid.NewGuid(),
                    Name = "No Facturable",
                    Amount = 0,
                    TaxType = TaxType.NO_FACTURABLE,
                    IsActive = true,
                    TenantId = tenant.TenantId,
                    CreatedAt = DateTimeOffset.UtcNow
                },
                new Tax
                {
                    Id = Guid.NewGuid(),
                    Name = "ITBIS 1",
                    Amount = 18,
                    TaxType = TaxType.ITBIS,
                    IsActive = true,
                    TenantId = tenant.TenantId,
                    CreatedAt = DateTimeOffset.UtcNow
                },
                new Tax
                {
                    Id = Guid.NewGuid(),
                    Name = "ITBIS 2",
                    Amount = 16,
                    TaxType = TaxType.ITBIS,
                    IsActive = true,
                    TenantId = tenant.TenantId,
                    CreatedAt = DateTimeOffset.UtcNow
                },
                new Tax
                {
                    Id = Guid.NewGuid(),
                    Name = "ITBIS 3",
                    Amount = 0,
                    TaxType = TaxType.ITBIS,
                    IsActive = true,
                    TenantId = tenant.TenantId,
                    CreatedAt = DateTimeOffset.UtcNow
                },
                new Tax
                {
                    Id = Guid.NewGuid(),
                    Name = "Exento",
                    Amount = 0,
                    TaxType = TaxType.EXENTO,
                    IsActive = true,
                    TenantId = tenant.TenantId,
                    CreatedAt = DateTimeOffset.UtcNow
                }
            };

            foreach (var tax in taxes)
            {
                _context.Set<Tax>().Add(tax);
            }

            // Generate Receipts (B-types or E-types) for the new tenant based on user.IsElectronicTransmitter
            foreach (var ecfType in Enum.GetValues<SITRA.Core.Enums.EcfType>())
            {
                bool isElectronicType = SITRA.Core.Helpers.Utilities.IsElectronicType(ecfType);
                if (user.IsElectronicTransmitter != isElectronicType)
                {
                    continue;
                }
                var fiscalSupport = new FiscalSupport
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenant.TenantId,
                    FiscalSupportType = ecfType,
                    Name = ecfType switch
                    {
                        SITRA.Core.Enums.EcfType.B01 => "Factura de Crédito Fiscal",
                        SITRA.Core.Enums.EcfType.B02 => "Factura de Consumo",
                        SITRA.Core.Enums.EcfType.B03 => "Nota de Débito",
                        SITRA.Core.Enums.EcfType.B04 => "Nota de Crédito",
                        SITRA.Core.Enums.EcfType.B11 => "Comprobante de Compras",
                        SITRA.Core.Enums.EcfType.B12 => "Registro Único de Ingresos",
                        SITRA.Core.Enums.EcfType.B13 => "Comprobante para Gastos Menores",
                        SITRA.Core.Enums.EcfType.B14 => "Comprobante para Regímenes Especiales",
                        SITRA.Core.Enums.EcfType.B15 => "Comprobante Gubernamental",
                        SITRA.Core.Enums.EcfType.B16 => "Comprobante para Exportaciones",
                        SITRA.Core.Enums.EcfType.B17 => "Comprobante para Pagos al Exterior",

                        SITRA.Core.Enums.EcfType.E31 => "Factura de Crédito Fiscal Electrónica",
                        SITRA.Core.Enums.EcfType.E32 => "Factura de Consumo Electrónica",
                        SITRA.Core.Enums.EcfType.E33 => "Nota de Débito Electrónica",
                        SITRA.Core.Enums.EcfType.E34 => "Nota de Crédito Electrónica",
                        SITRA.Core.Enums.EcfType.E41 => "Comprobante Electrónico de Compras",
                        SITRA.Core.Enums.EcfType.E43 => "Comprobante Electrónico para Gastos Menores",
                        SITRA.Core.Enums.EcfType.E44 => "Comprobante Electrónico para Regímenes Especiales",
                        SITRA.Core.Enums.EcfType.E45 => "Comprobante Electrónico Gubernamental",
                        SITRA.Core.Enums.EcfType.E46 => "Comprobante Electrónico para Exportaciones",
                        SITRA.Core.Enums.EcfType.E47 => "Comprobante Electrónico para Pagos al Exterior",
                        _ => Enum.GetName(ecfType) ?? string.Empty
                    },
                    InitialSequence = 0,
                    FinalSequence = (ecfType == SITRA.Core.Enums.EcfType.B02 || ecfType == SITRA.Core.Enums.EcfType.E32) ? 50000000 : 10000000,
                    IsActive = true,
                    IsDefault = false
                };
                fiscalSupport.InitializeDefaults();
                _context.Set<FiscalSupport>().Add(fiscalSupport);
            }

            var pricing = new Pricing
            {
                Name = Enum.GetName(PricingName.Principal)?.ToUpper() ?? "PRINCIPAL",
                PricingType = PricingType.Value,
                IsDefault = true
            };

            await _context.Set<Pricing>().AddAsync(pricing);

            var warehouse = new Warehouse
            {
                TenantId = tenant.TenantId,
                Name = "TIENDA PRINCIPAL",
                Address = "N/A",
                IsDefault = true
            };
            
            await _context.Set<Warehouse>().AddAsync(warehouse);

            var contact = new Contact
            {
                TenantId = tenant.TenantId,
                Name = "CONSUMIDOR FINAL",
                Identification = "00000000000",
                ContactType = ContactType.Customer,
                IdentificationType = IdentificationType.Cedula,
                Province = "010000",
                Municipality = "010101",
                Address = "N/A",
                IsDefault = true
            };

            await _context.Set<Contact>().AddAsync(contact);

            foreach (var currencyType in Enum.GetValues<CurrencyType>())
            {
                var loopCurrency = new Currency
                {
                    TenantId = tenant.TenantId,
                    Name = currencyType switch
                    {
                        CurrencyType.DOP => "Peso Dominicano",
                        CurrencyType.BRL => "Real Brasileno",
                        CurrencyType.CAD => "Dolar Canadiense",
                        CurrencyType.CHF => "Franco Suizo",
                        CurrencyType.CHY => "Yuan Chino",
                        CurrencyType.XDR => "Derecho Especial de Giro",
                        CurrencyType.DKK => "Corona Danesa",
                        CurrencyType.EUR => "Euro",
                        CurrencyType.GBP => "Libra Esterlina",
                        CurrencyType.JPY => "Yen Japones",
                        CurrencyType.NOK => "Corona Noruega",
                        CurrencyType.SCP => "Libra Escocesa",
                        CurrencyType.SEK => "Corona Sueca",
                        CurrencyType.USD => "Dolar Estadounidense",
                        CurrencyType.VEF => "Bolivar Fuerte Venezolano",
                        CurrencyType.HTG => "Gurda Haitiana",
                        CurrencyType.MXN => "Peso Mexicano",
                        CurrencyType.COP => "Peso Colombiano",
                        _ => Enum.GetName(currencyType) ?? string.Empty
                    },
                    Code = currencyType.ToString(),
                    Symbol = currencyType switch
                    {
                        CurrencyType.DOP => "RD$",
                        CurrencyType.BRL => "R$",
                        CurrencyType.CAD => "C$",
                        CurrencyType.CHF => "CHF",
                        CurrencyType.CHY => "¥",
                        CurrencyType.XDR => "SDR",
                        CurrencyType.DKK => "kr",
                        CurrencyType.EUR => "€",
                        CurrencyType.GBP => "£",
                        CurrencyType.JPY => "¥",
                        CurrencyType.NOK => "kr",
                        CurrencyType.SCP => "£",
                        CurrencyType.SEK => "kr",
                        CurrencyType.USD => "$",
                        CurrencyType.VEF => "Bs.",
                        CurrencyType.HTG => "G",
                        CurrencyType.MXN => "$",
                        CurrencyType.COP => "$",
                        _ => string.Empty
                    },
                    Rate = 1.00,
                    IsDefault = currencyType == CurrencyType.DOP
                };

                await _context.Set<Currency>().AddAsync(loopCurrency);
            }

            await _context.SaveChangesAsync(cancellationToken);

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
                    .Get<PermissionManagementConfigDTO>();

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
                    IdentityResult = IdentityResult.Failed(new IdentityError { Description = SITRA.Core.Helpers.Utilities.CurrentUserNotFoundMessage })
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
                    IdentityResult = IdentityResult.Failed(new IdentityError { Description = SITRA.Core.Helpers.Utilities.RoleNotFoundMessage })
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
