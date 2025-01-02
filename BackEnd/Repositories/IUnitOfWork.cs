using BackEnd.Repositories.Accounts;
using BackEnd.Repositories.AdministrativeTools;
using BackEnd.Repositories.Contacts;
using BackEnd.Repositories.Finances;
using BackEnd.Repositories.Generics;
using BackEnd.Repositories.Invoices;
using BackEnd.Repositories.Products;
using BackEnd.Repositories.Warehouses;
namespace BackEnd.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryBase<T> Repository<T>() where T : class;
        IReceivableRepository ReceivableRepository { get; }
        IPaymentReceiptRepository PaymentReceiptRepository { get; }
        IDeadlineRepository DeadlineRepository { get; }
        IFiscalSupportRepository FiscalSupportRepository { get; }
        IContactRepository ContactRepository { get; }
        ICurrencyRepository CurrencyRepository { get; }
        ITaxRepository TaxRepository { get; }
        ISaleInvoiceRepository SaleInvoiceRepository { get; }
        ISpotPaymentRepository SpotPaymentRepository { get; }
        IItemRepository ItemRepository { get; }
        IProductRepository ProductRepository { get; }
        IServiceRepository ServiceRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IBrandRepository BrandRepository { get; }
        IUnitRepository UnitRepository { get; }
        IPricingItemRepository PricingItemRepository { get; }
        IPricingRepository PricingRepository { get; }
        IStockRepository StockRepository { get; }
        ITransferRepository TransferRepository { get; }
        IWarehouseRepository WarehouseRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
