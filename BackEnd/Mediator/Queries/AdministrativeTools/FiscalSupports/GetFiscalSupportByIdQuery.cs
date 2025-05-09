using AutoMapper;
using BackEnd.DTOs.AdministrativeTools.FiscalSupports;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.AdministrativeTools.FiscalSupports
{
    public class GetFiscalSupportByIdQuery : GetByIdQuery<FiscalSupportDTO>
    {
        public GetFiscalSupportByIdQuery(long id) : base(id) { }

        public override async Task<FiscalSupportDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var fiscalSupport = await unitOfWork.FiscalSupportRepository.GetByIdAsync(Id);
            return mapper.Map<FiscalSupportDTO>(fiscalSupport);
        }
    }
}
