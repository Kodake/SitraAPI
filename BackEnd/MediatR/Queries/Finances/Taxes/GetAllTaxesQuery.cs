using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Finances.Taxes;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Finances.Taxes
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
