using BackEnd.Entities.Invoices;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Invoices
{
    public interface IPaymentRepository : IRepositoryBase<Payment>
    {
    }
}