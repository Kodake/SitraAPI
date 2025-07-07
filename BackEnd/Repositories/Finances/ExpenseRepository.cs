using BackEnd.Context;
using BackEnd.Entities.Finances;

namespace BackEnd.Repositories.Finances
{
    public class ExpenseRepository : RepositoryBase<Expense>, IExpenseRepository
    {
        public ExpenseRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }
    }
}
