using BackEnd.Entities.Currencies;
using BackEnd.Enums;

namespace BackEnd.Repositories.Currencies
{
    public interface ITaxRepository
    {
        Task<List<Tax>> GetAll(StatusType status);
    }
}
