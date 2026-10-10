using AutoMapper;
using Mediator;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Commands.Invoices.Quotations;

namespace SITRA.Core.Mediator.Handlers.Invoices
{
    public class CreateQuotationWithItemCommandHandler : IRequestHandler<CreateQuotationWithItemCommand, CreateQuotationWithItemDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IMapper _mapper;

        public CreateQuotationWithItemCommandHandler(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
            _mapper = mapper;
        }

        public async ValueTask<CreateQuotationWithItemDTO> Handle(CreateQuotationWithItemCommand request, CancellationToken cancellationToken)
        {
            return await request.ExecuteAction(_unitOfWork, _outputCacheStore, _mapper, cancellationToken);
        }
    }
}
