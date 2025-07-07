using AutoMapper;
using BackEnd.DTOs.Finances.Expenses;
using BackEnd.Entities.Finances;

namespace BackEnd.Profiles.Finances
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
