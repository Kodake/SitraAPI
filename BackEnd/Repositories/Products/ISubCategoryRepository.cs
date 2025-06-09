using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Products
{
    public interface ISubCategoryRepository : IRepositoryBase<SubCategory>
    {
        Task<List<SubCategory>> GetAll(StatusType status);
        Task<bool> Exists(long id, string code);
    }
}
