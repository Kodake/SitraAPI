using BackEnd.Repositories;
using BackEnd.DTOs;
using AutoMapper;

namespace BackEnd.MediatR.Queries
{
    public abstract class GetAllQuery<TDto> : IQuery<List<TDto>>
    {
        public PaginationDTO Pagination { get; set; }

        protected GetAllQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }

        public abstract Task<List<TDto>> GetData(
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            CancellationToken cancellationToken);
    }
}
