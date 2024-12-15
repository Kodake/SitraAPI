using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Products
{
    public interface IUnitRepository : IRepositoryBase<Unit>
    {
        Task<List<Unit>> GetAll(StatusType status);
        Task<bool> Exists(long id, string name);
    }
}
