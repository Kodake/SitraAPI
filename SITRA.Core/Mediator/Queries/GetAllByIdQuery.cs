using AutoMapper;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries
{
    public abstract class GetAllByIdQuery<TDto> : IQuery<IEnumerable<TDto>>
    {
        public Guid Id { get; set; }

        protected GetAllByIdQuery(Guid id)
        {
            Id = id;
        }

        public abstract Task<IEnumerable<TDto>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken
        );
    }
}
