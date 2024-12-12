using BackEnd.Context;
using BackEnd.Repositories.AdministrativeTools;
using BackEnd.Repositories.Contacts;
using BackEnd.Repositories.Finances;
using BackEnd.Repositories.Generics;
using BackEnd.Repositories.Products;
using BackEnd.Repositories.Warehouses;

namespace BackEnd.Repositories
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Dictionary<Type, object> _repositories = new();

        public IDeadlineRepository DeadlineRepository { get; }
        public IContactRepository ContactRepository { get; }
        public ICurrencyRepository CurrencyRepository { get; }
        public ITaxRepository TaxRepository { get; }
        public IWarehouseRepository WarehouseRepository { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            ContactRepository = new ContactRepository(context, httpContextAccessor);
            DeadlineRepository = new DeadlineRepository(context, httpContextAccessor);
            CurrencyRepository = new CurrencyRepository(context, httpContextAccessor);
            TaxRepository = new TaxRepository(context, httpContextAccessor);
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
