using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Wizard;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Commands.Wizard;

namespace SITRA.Core.Mediator.Handlers.Wizard
{
    public class CreateWizardCommandHandler : MediatR.IRequestHandler<CreateWizardCommand, WizardDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IMapper _mapper;

        public CreateWizardCommandHandler(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
            _mapper = mapper;
        }

        public async Task<WizardDTO> Handle(CreateWizardCommand request, CancellationToken cancellationToken)
        {
            return await request.ExecuteAction(_unitOfWork, _outputCacheStore, _mapper, cancellationToken);
        }
    }
}
