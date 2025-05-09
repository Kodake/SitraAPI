using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.AdministrativeTools.FiscalSupports;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.AdministrativeTools.FiscalSupports
{
    public class GetAllFiscalSupportsQuery : GetAllQuery<FiscalSupportListDTO>
    {
        public GetAllFiscalSupportsQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<FiscalSupportListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var fiscalSupports = await unitOfWork.FiscalSupportRepository.GetAllAsync(Pagination);
            return mapper.Map<List<FiscalSupportListDTO>>(fiscalSupports);
        }
    }
}
