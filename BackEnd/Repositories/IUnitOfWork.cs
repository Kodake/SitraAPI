using BackEnd.Repositories.Accounts;
using BackEnd.Repositories.AdministrativeTools;
using BackEnd.Repositories.Appointments;
using BackEnd.Repositories.CashBoxes;
using BackEnd.Repositories.Contacts;
using BackEnd.Repositories.Finances;
using BackEnd.Repositories.Generics;
using BackEnd.Repositories.Invoices;
using BackEnd.Repositories.Products;
using BackEnd.Repositories.Warehouses;
using BackEnd.Services;
namespace BackEnd.Repositories
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
        IUnitRepository UnitRepository { get; }
        IPricingItemRepository PricingItemRepository { get; }
        IPricingRepository PricingRepository { get; }
        IStockRepository StockRepository { get; }
        ITransferRepository TransferRepository { get; }
        IWarehouseRepository WarehouseRepository { get; }
        IPettyCashRepository PettyCashRepository { get; }
        IPettyCashMovementRepository PettyCashMovementRepository { get; }
        IAppointmentRepository AppointmentRepository { get; }
        IUserService UserService { get; }
        IFileStorageService FileStorageService { get; }
        Task<int> SaveChangesAsync();
    }
}
