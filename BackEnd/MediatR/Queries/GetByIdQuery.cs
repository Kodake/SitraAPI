using AutoMapper;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries
{
    public abstract class GetByIdQuery<TDto> : IQuery<TDto>
    {
        public int Id { get; set; }

        protected GetByIdQuery(int id)
        {
            Id = id;
        }

        public abstract Task<TDto> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken
        );
    }
}
