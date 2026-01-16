using AutoMapper;
using SITRA.Core.DTOs.Finances.Expenses;
using SITRA.Core.Entities.Finances;

namespace SITRA.API.Profiles.Finances
{
    public class ExpensesProfile : Profile
    {
        public ExpensesProfile()
        {
            CreateMap<CreateExpenseDTO, Expense>();
            CreateMap<Expense, ExpenseListDTO>();
            CreateMap<Expense, ExpenseDTO>();
        }
    }
}

