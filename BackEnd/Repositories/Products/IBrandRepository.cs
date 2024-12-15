using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Products
{
    public interface IBrandRepository : IRepositoryBase<Brand>
    {
        Task<List<Brand>> GetAll(StatusType status);
        Task<bool> Exists(long id, string code);
    }
}
