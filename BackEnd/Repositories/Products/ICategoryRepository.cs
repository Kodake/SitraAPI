using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Products
{
    public interface ICategoryRepository : IRepositoryBase<Category>
    {
        Task<List<Category>> GetAll(StatusType status);
        Task<bool> Exists(long id, string code);
    }
}
