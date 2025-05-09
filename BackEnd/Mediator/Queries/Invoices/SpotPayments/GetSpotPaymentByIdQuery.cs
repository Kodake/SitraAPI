using AutoMapper;
using BackEnd.DTOs.Invoices.SpotPayments;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Invoices.SpotPayments
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