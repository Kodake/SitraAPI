using AutoMapper;
using BackEnd.DTOs.CashBoxes;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.CashBoxes
{
    public class GetAllPaymentMethodSummaryByIdQuery(long pettyCashId) : IQuery<List<PaymentMethodAmountListDTO>>
    {
        public long PettyCashId { get; } = pettyCashId;

        public async Task<List<PaymentMethodAmountListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var paymentMethodAmounts = await unitOfWork.PettyCashMovementRepository.GetPaymentMethodAmountListAsync(PettyCashId);
            return mapper.Map<List<PaymentMethodAmountListDTO>>(paymentMethodAmounts);
        }
    }
}
