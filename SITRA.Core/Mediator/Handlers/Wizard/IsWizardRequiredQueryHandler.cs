using AutoMapper;
using MediatR;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Queries.Wizard;

namespace SITRA.Core.Mediator.Handlers.Wizard
{
    public class IsWizardRequiredQueryHandler : IRequestHandler<IsWizardRequiredQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public IsWizardRequiredQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(IsWizardRequiredQuery request, CancellationToken cancellationToken)
        {
            return await request.GetData(_unitOfWork, _mapper, cancellationToken);
        }
    }
}
