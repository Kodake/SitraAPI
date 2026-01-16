using AutoMapper;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries
{
    public abstract class GetByIdQuery<TDto>(long id) : IQuery<TDto>
    {
        public long Id { get; set; } = id;

        public abstract Task<TDto> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken
        );
    }
}
