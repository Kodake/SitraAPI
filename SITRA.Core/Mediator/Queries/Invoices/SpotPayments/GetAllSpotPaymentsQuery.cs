using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.SpotPayments;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.SpotPayments
{
    public class GetAllSpotPaymentsQuery : GetAllQuery<SpotPaymentListDTO>
    {
        public GetAllSpotPaymentsQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<SpotPaymentListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var spotPayments = await unitOfWork.SpotPaymentRepository.GetAllAsync(Pagination);
            return mapper.Map<List<SpotPaymentListDTO>>(spotPayments);
        }
    }
}
