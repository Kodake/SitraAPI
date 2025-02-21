using BackEnd.Context;
using BackEnd.Entities.Invoices;

namespace BackEnd.Repositories.Invoices
{
    public class PaymentRepository : RepositoryBase<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }
    }
}