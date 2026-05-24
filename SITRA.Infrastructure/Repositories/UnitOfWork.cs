using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Accounts;
using SITRA.Core.Interfaces.AdministrativeTools;
using SITRA.Core.Interfaces.Appointments;
using SITRA.Core.Interfaces.CashBoxes;
using SITRA.Core.Interfaces.Contacts;
using SITRA.Core.Interfaces.Finances;
using SITRA.Core.Interfaces.Generics;
using SITRA.Core.Interfaces.Invoices;
using SITRA.Core.Interfaces.Products;
using SITRA.Core.Interfaces.Services;
using SITRA.Core.Interfaces.Warehouses;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Accounts;
using SITRA.Infrastructure.Repositories.AdministrativeTools;
using SITRA.Infrastructure.Repositories.Appointments;
using SITRA.Infrastructure.Repositories.CashBoxes;
using SITRA.Infrastructure.Repositories.Contacts;
using SITRA.Infrastructure.Repositories.Finances;
using SITRA.Infrastructure.Repositories.Generics;
using SITRA.Infrastructure.Repositories.Invoices;
using SITRA.Infrastructure.Repositories.Products;
using SITRA.Infrastructure.Repositories.Warehouses;
using SITRA.Infrastructure.Services;
namespace SITRA.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<Type, object> _repositories = [];

        public IPayableRepository PayableRepository { get; }
        public IReceivableRepository ReceivableRepository { get; }
        public IPayableReceiptRepository PayableReceiptRepository { get; }
        public IPaymentReceiptRepository PaymentReceiptRepository { get; }
        public IDeadlineRepository DeadlineRepository { get; }
        public IFiscalSupportRepository FiscalSupportRepository { get; }
        public IContactRepository ContactRepository { get; }
        public ICurrencyRepository CurrencyRepository { get; }
        public ITaxRepository TaxRepository { get; }
        public IExpenseRepository ExpenseRepository { get; }
        public ISaleInvoiceRepository SaleInvoiceRepository { get; }
        public IPurchaseInvoiceRepository PurchaseInvoiceRepository { get; }
        public ILogInvoiceRepository LogInvoiceRepository { get; }
        public IQuotationRepository QuotationRepository { get; }
        public ISpotPaymentRepository SpotPaymentRepository { get; }
        public IPaymentRepository PaymentRepository { get; }
        public ISpotPayableRepository SpotPayableRepository { get; }
        public IItemRepository ItemRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IServiceRepository ServiceRepository { get; }
        public IComboRepository ComboRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public ISubCategoryRepository SubCategoryRepository { get; }
        public IBrandRepository BrandRepository { get; }
        public IModelRepository ModelRepository { get; }
        public IPricingItemRepository PricingItemRepository { get; }
        public IPricingRepository PricingRepository { get; }
        public IStockRepository StockRepository { get; }
        public ITransferRepository TransferRepository { get; }
        public IWarehouseRepository WarehouseRepository { get; }
        public IPettyCashRepository PettyCashRepository { get; }
        public IPettyCashMovementRepository PettyCashMovementRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IPermissionRepository PermissionRepository { get; }
        public IRolePermissionRepository RolePermissionRepository { get; }
        public IAppointmentRepository AppointmentRepository { get; }
        public IUserService UserService { get; }
        public IAuthenticationService AuthenticationService { get; }
        public IUserRegistrationService UserRegistrationService { get; }
        public IUserRoleService UserRoleService { get; }
        public IPermissionClaimService PermissionClaimService { get; }
        public IPlanService PlanService { get; }
        public IFileStorageService FileStorageService { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ILogger<UserService> userLogger,
            ILogger<PermissionClaimService> permissionClaimLogger,
            ILogger<AuthenticationService> authLogger,
            ILogger<UserRegistrationService> registrationLogger,
            ILogger<UserRoleService> roleServiceLogger,
            IServiceProvider serviceProvider)
        {
            PayableRepository = new PayableRepository(context, httpContextAccessor);
            ReceivableRepository = new ReceivableRepository(context, httpContextAccessor);
            PayableReceiptRepository = new PayableReceiptRepository(context, httpContextAccessor);
            PaymentReceiptRepository = new PaymentReceiptRepository(context, httpContextAccessor);
            DeadlineRepository = new DeadlineRepository(context, httpContextAccessor);
            FiscalSupportRepository = new FiscalSupportRepository(context, httpContextAccessor);
            ContactRepository = new ContactRepository(context, httpContextAccessor);
            CurrencyRepository = new CurrencyRepository(context, httpContextAccessor);
            TaxRepository = new TaxRepository(context, httpContextAccessor);
            ExpenseRepository = new ExpenseRepository(context, httpContextAccessor);
            SaleInvoiceRepository = new SaleInvoiceRepository(context, httpContextAccessor);
            PurchaseInvoiceRepository = new PurchaseInvoiceRepository(context, httpContextAccessor);
            LogInvoiceRepository = new LogInvoiceRepository(context, httpContextAccessor);
            QuotationRepository = new QuotationRepository(context, httpContextAccessor);
            SpotPaymentRepository = new SpotPaymentRepository(context, httpContextAccessor);
            PaymentRepository = new PaymentRepository(context, httpContextAccessor);
            SpotPayableRepository = new SpotPayableRepository(context, httpContextAccessor);
            ItemRepository = new ItemRepository(context, httpContextAccessor);
            ProductRepository = new ProductRepository(context, httpContextAccessor);
            ServiceRepository = new ServiceRepository(context, httpContextAccessor);
            ComboRepository = new ComboRepository(context, httpContextAccessor);
            CategoryRepository = new CategoryRepository(context, httpContextAccessor);
            SubCategoryRepository = new SubCategoryRepository(context, httpContextAccessor);
            BrandRepository = new BrandRepository(context, httpContextAccessor);
            ModelRepository = new ModelRepository(context, httpContextAccessor);
            PricingItemRepository = new PricingItemRepository(context, httpContextAccessor);
            PricingRepository = new PricingRepository(context, httpContextAccessor);
            StockRepository = new StockRepository(context, httpContextAccessor);
            TransferRepository = new TransferRepository(context, httpContextAccessor);
            WarehouseRepository = new WarehouseRepository(context, httpContextAccessor);
            PettyCashRepository = new PettyCashRepository(context, httpContextAccessor);
            PettyCashMovementRepository = new PettyCashMovementRepository(context, httpContextAccessor);
            RoleRepository = new RoleRepository(context, httpContextAccessor);
            PermissionRepository = new PermissionRepository(context);
            RolePermissionRepository = new RolePermissionRepository(context);
            AppointmentRepository = new AppointmentRepository(context, httpContextAccessor);

            // Services — no inter-service dependencies, all are peers
            UserService = new UserService(context, httpContextAccessor, userManager);
            PermissionClaimService = new PermissionClaimService(userManager, this, permissionClaimLogger);
            AuthenticationService = new AuthenticationService(context, userManager, configuration, this, authLogger);
            UserRegistrationService = new UserRegistrationService(context, userManager, configuration, this, UserService, registrationLogger);
            var dgiiStorageService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<IDgiiStorageService>(serviceProvider);
            var dataProtectionProvider = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<Microsoft.AspNetCore.DataProtection.IDataProtectionProvider>(serviceProvider);
            UserRoleService = new UserRoleService(context, userManager, httpContextAccessor, roleServiceLogger, dgiiStorageService, dataProtectionProvider);

            PlanService = new PlanService();
            FileStorageService = new AzureFileService(configuration);

            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public IRepositoryBase<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = new RepositoryBase<T>(_context, _httpContextAccessor);
                _repositories.Add(type, repositoryInstance);
            }

            return (IRepositoryBase<T>)_repositories[type];
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

