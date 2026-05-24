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
namespace SITRA.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryBase<T> Repository<T>() where T : class;
        IPayableRepository PayableRepository { get; }
        IReceivableRepository ReceivableRepository { get; }
        IPayableReceiptRepository PayableReceiptRepository { get; }
        IPaymentReceiptRepository PaymentReceiptRepository { get; }
        IDeadlineRepository DeadlineRepository { get; }
        IFiscalSupportRepository FiscalSupportRepository { get; }
        IContactRepository ContactRepository { get; }
        ICurrencyRepository CurrencyRepository { get; }
        ITaxRepository TaxRepository { get; }
        IExpenseRepository ExpenseRepository { get; }
        ISaleInvoiceRepository SaleInvoiceRepository { get; }
        IPurchaseInvoiceRepository PurchaseInvoiceRepository { get; }
        ISpotPaymentRepository SpotPaymentRepository { get; }
        ILogInvoiceRepository LogInvoiceRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        ISpotPayableRepository SpotPayableRepository { get; }
        IQuotationRepository QuotationRepository { get; }
        IItemRepository ItemRepository { get; }
        IProductRepository ProductRepository { get; }
        IServiceRepository ServiceRepository { get; }
        IComboRepository ComboRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ISubCategoryRepository SubCategoryRepository { get; }
        IBrandRepository BrandRepository { get; }
        IModelRepository ModelRepository { get; }
        IPricingItemRepository PricingItemRepository { get; }
        IPricingRepository PricingRepository { get; }
        IStockRepository StockRepository { get; }
        ITransferRepository TransferRepository { get; }
        IWarehouseRepository WarehouseRepository { get; }
        IPettyCashRepository PettyCashRepository { get; }
        IPettyCashMovementRepository PettyCashMovementRepository { get; }
        IRoleRepository RoleRepository { get; }
        IPermissionRepository PermissionRepository { get; }
        IRolePermissionRepository RolePermissionRepository { get; }
        IAppointmentRepository AppointmentRepository { get; }
        IUserService UserService { get; }
        IAuthenticationService AuthenticationService { get; }
        IUserRegistrationService UserRegistrationService { get; }
        IUserRoleService UserRoleService { get; }
        IPermissionClaimService PermissionClaimService { get; }
        IPlanService PlanService { get; }
        IFileStorageService FileStorageService { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

