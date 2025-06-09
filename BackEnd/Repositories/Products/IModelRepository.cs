using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Products
{
    public interface IModelRepository : IRepositoryBase<Model>
    {
        Task<List<Model>> GetAll(StatusType status);
        Task<bool> Exists(long id, string code);
    }
}
