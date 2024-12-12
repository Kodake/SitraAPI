using BackEnd.Repositories.AdministrativeTools;
using BackEnd.Repositories.Contacts;
using BackEnd.Repositories.Finances;
using BackEnd.Repositories.Generics;
using BackEnd.Repositories.Warehouses;
namespace BackEnd.Repositories
{
    public interface IUnitOfWork: IDisposable
    {
        IRepositoryBase<T> Repository<T>() where T : class;
        IContactRepository ContactRepository { get; }
        ICurrencyRepository CurrencyRepository { get; }
        ITaxRepository TaxRepository { get; }
        IDeadlineRepository DeadlineRepository { get; }
        IWarehouseRepository WarehouseRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
