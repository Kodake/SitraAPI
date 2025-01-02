using BackEnd.Context;
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
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Dictionary<Type, object> _repositories = new();

        public IReceivableRepository ReceivableRepository { get; }
        public IPaymentReceiptRepository PaymentReceiptRepository { get; }
        public IDeadlineRepository DeadlineRepository { get; }
        public IFiscalSupportRepository FiscalSupportRepository { get; }
        public IContactRepository ContactRepository { get; }
        public ICurrencyRepository CurrencyRepository { get; }
        public ITaxRepository TaxRepository { get; }
        public ISaleInvoiceRepository SaleInvoiceRepository { get; }
        public ISpotPaymentRepository SpotPaymentRepository { get; }
        public IItemRepository ItemRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IServiceRepository ServiceRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public IBrandRepository BrandRepository { get; }
        public IUnitRepository UnitRepository { get; }
        public IPricingItemRepository PricingItemRepository { get; }
        public IPricingRepository PricingRepository { get; }
        public IStockRepository StockRepository { get; }
        public ITransferRepository TransferRepository { get; }
        public IWarehouseRepository WarehouseRepository { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            ReceivableRepository = new ReceivableRepository(context, httpContextAccessor);
            PaymentReceiptRepository = new PaymentReceiptRepository(context, httpContextAccessor);
            DeadlineRepository = new DeadlineRepository(context, httpContextAccessor);
            FiscalSupportRepository = new FiscalSupportRepository(context, httpContextAccessor);
            ContactRepository = new ContactRepository(context, httpContextAccessor);
            CurrencyRepository = new CurrencyRepository(context, httpContextAccessor);
            TaxRepository = new TaxRepository(context, httpContextAccessor);
            SaleInvoiceRepository = new SaleInvoiceRepository(context, httpContextAccessor);
            SpotPaymentRepository = new SpotPaymentRepository(context, httpContextAccessor);
            ItemRepository = new ItemRepository(context, httpContextAccessor);
            ProductRepository = new ProductRepository(context, httpContextAccessor);
            ServiceRepository = new ServiceRepository(context, httpContextAccessor);
            CategoryRepository = new CategoryRepository(context, httpContextAccessor);
            BrandRepository = new BrandRepository(context, httpContextAccessor);
            UnitRepository = new UnitRepository(context, httpContextAccessor);
            PricingItemRepository = new PricingItemRepository(context, httpContextAccessor);
            PricingRepository = new PricingRepository(context, httpContextAccessor);
            StockRepository = new StockRepository(context, httpContextAccessor);
            TransferRepository = new TransferRepository(context, httpContextAccessor);
            WarehouseRepository = new WarehouseRepository(context, httpContextAccessor);
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
