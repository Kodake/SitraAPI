using BackEnd.Context;
using BackEnd.Entities.Invoices;

namespace BackEnd.Repositories.Invoices
{
    public class SpotPaymentRepository : RepositoryBase<SpotPayment>, ISpotPaymentRepository
    {
        private new readonly HttpContext httpContext;
        public SpotPaymentRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }
    }
}
