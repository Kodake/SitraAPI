using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Finances.Taxes
{
    public class GetAllTaxesQuery : GetAllQuery<TaxListDTO>
    {
        public GetAllTaxesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<TaxListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var taxes = await unitOfWork.TaxRepository.GetAllAsync(Pagination);
            return mapper.Map<List<TaxListDTO>>(taxes);
        }
    }
}
