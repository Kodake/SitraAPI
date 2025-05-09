using BackEnd.Context;
using BackEnd.Entities.Auths;
using BackEnd.Repositories.Accounts;
using BackEnd.Repositories.AdministrativeTools;
using BackEnd.Repositories.CashBoxes;
using BackEnd.Repositories.Contacts;
using BackEnd.Repositories.Finances;
using BackEnd.Repositories.Generics;
using BackEnd.Repositories.Invoices;
using BackEnd.Repositories.Products;
using BackEnd.Repositories.Warehouses;
using BackEnd.Services;
using Microsoft.AspNetCore.Identity;

namespace BackEnd.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Dictionary<Type, object> _repositories = new();

        public IPayableRepository PayableRepository { get; }
        public IReceivableRepository ReceivableRepository { get; }
        public IPayableReceiptRepository PayableReceiptRepository { get; }
        public IPaymentReceiptRepository PaymentReceiptRepository { get; }
        public IDeadlineRepository DeadlineRepository { get; }
        public IFiscalSupportRepository FiscalSupportRepository { get; }
        public IContactRepository ContactRepository { get; }
        public ICurrencyRepository CurrencyRepository { get; }
        public ITaxRepository TaxRepository { get; }
        public ISaleInvoiceRepository SaleInvoiceRepository { get; }
        public IPurchaseInvoiceRepository PurchaseInvoiceRepository { get; }
        public IQuotationRepository QuotationRepository { get; }
        public ISpotPaymentRepository SpotPaymentRepository { get; }
        public IPaymentRepository PaymentRepository { get; }
        public ISpotPayableRepository SpotPayableRepository { get; }
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
        public IPettyCashRepository PettyCashRepository { get; }
        public IPettyCashMovementRepository PettyCashMovementRepository { get; }
        public IUserService UserService { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor, 
            UserManager<User> userManager)
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
            SaleInvoiceRepository = new SaleInvoiceRepository(context, httpContextAccessor);
            PurchaseInvoiceRepository = new PurchaseInvoiceRepository(context, httpContextAccessor);
            QuotationRepository = new QuotationRepository(context, httpContextAccessor);
            SpotPaymentRepository = new SpotPaymentRepository(context, httpContextAccessor);
            PaymentRepository = new PaymentRepository(context, httpContextAccessor);
            SpotPayableRepository = new SpotPayableRepository(context, httpContextAccessor);
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
            PettyCashRepository = new PettyCashRepository(context, httpContextAccessor);
            PettyCashMovementRepository = new PettyCashMovementRepository(context, httpContextAccessor);
            UserService = new UserService(context, httpContextAccessor, userManager);
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
