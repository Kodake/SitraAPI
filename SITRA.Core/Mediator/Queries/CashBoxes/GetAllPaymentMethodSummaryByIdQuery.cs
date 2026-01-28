using AutoMapper;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.CashBoxes
{
    public class GetAllPaymentMethodSummaryByIdQuery(Guid pettyCashId) : IQuery<List<PaymentMethodAmountListDTO>>
    {
        public Guid PettyCashId { get; } = pettyCashId;

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
