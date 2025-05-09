using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Invoices.SpotPayments;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Invoices.SpotPayments
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