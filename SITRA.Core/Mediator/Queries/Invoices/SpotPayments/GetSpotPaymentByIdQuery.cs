using AutoMapper;
using SITRA.Core.DTOs.Invoices.SpotPayments;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.SpotPayments
{
    public class GetSpotPaymentByIdQuery : GetByIdQuery<SpotPaymentDTO>
    {
        public GetSpotPaymentByIdQuery(long id) : base(id) { }

        public override async Task<SpotPaymentDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var spotPayment = await unitOfWork.SpotPaymentRepository.GetByIdAsync(Id);
            return mapper.Map<SpotPaymentDTO>(spotPayment);
        }
    }
}
