using AutoMapper;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.CashBoxes
{
    public class GetPettyCashByUserIdQuery : GetByQuery<PettyCashDTO?>
    {
        public GetPettyCashByUserIdQuery() : base() { }

        public override async Task<PettyCashDTO?> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var user = await unitOfWork.UserService.GetUserAsync();
            if (user == null)
                return null;

            var pettyCash = await unitOfWork.PettyCashRepository.GetAllByUserIdAsync(user.Id);
            if (pettyCash == null)
                return null;

            var paymentMethods = await unitOfWork.PettyCashMovementRepository
                .GetPaymentMethodSumsAsync(pettyCash.Id);

            var totalWithdrawals = await unitOfWork.PettyCashMovementRepository
                .GetTotalWithdrawalsAsync(pettyCash.Id);

            var dto = mapper.Map<PettyCashDTO>(pettyCash);

            dto.PaymentMethods = paymentMethods
                .Where(kvp => kvp.Key is PaymentMethod)
                .Select(kvp => new PaymentMethodAmountListDTO
                {
                    MethodType = (PaymentMethod)kvp.Key,
                    Amount = kvp.Value
                })
                .ToList();

            dto.CashBilled = paymentMethods.TryGetValue(PaymentMethod.Cash, out var cash)
                ? cash
                : 0;

            dto.TotalBilled = dto.PaymentMethods.Sum(p => p.Amount);

            dto.WithdrawalAmount = totalWithdrawals;

            //dto.ClosingAmount = dto.OpeningAmount + dto.CashBilled - dto.WithdrawalAmount;

            return dto;
        }
    }
}

