using AutoMapper;
using BackEnd.DTOs.Accounts.Receivables;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Receivables
{
    public class GetPaysByIdQuery : GetByIdQuery<ReceivableDTO>
    {
        public GetPaysByIdQuery(long id) : base(id) { }

        public override async Task<ReceivableDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var receivable = await unitOfWork.ReceivableRepository.GetPaysByIdAsync(Id);
            return mapper.Map<ReceivableDTO>(receivable);
        }
    }
}