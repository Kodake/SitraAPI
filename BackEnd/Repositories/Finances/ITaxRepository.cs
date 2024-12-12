using BackEnd.Entities.Finances;
using BackEnd.Enums;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Finances
{
    public interface ITaxRepository : IRepositoryBase<Tax>
    {
        Task<List<Tax>> GetAll(StatusType status);
    }
}
