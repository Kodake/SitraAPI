using AutoMapper;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Interfaces.Services;
using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Interfaces.Services.Messaging;
using MediatR;

namespace SITRA.Core.Mediator.Queries.CashBoxes
{
    public class GetAllPaymentMethodSummaryByIdQuery(Guid pettyCashId) : IRequest<List<PaymentMethodAmountListDTO>>
    {
        public Guid PettyCashId { get; } = pettyCashId;
    }

    public class GetAllPaymentMethodSummaryByIdQueryHandler : IRequestHandler<GetAllPaymentMethodSummaryByIdQuery, List<PaymentMethodAmountListDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IPettyCashService _pettyCashService;

        public GetAllPaymentMethodSummaryByIdQueryHandler(IMapper mapper, IPettyCashService pettyCashService)
        {
            _mapper = mapper;
            _pettyCashService = pettyCashService;
        }

        public async Task<List<PaymentMethodAmountListDTO>> Handle(GetAllPaymentMethodSummaryByIdQuery request, CancellationToken cancellationToken)
        {
            var paymentMethodAmounts = await _pettyCashService.GetPaymentMethodAmountListAsync(request.PettyCashId, cancellationToken);
            return _mapper.Map<List<PaymentMethodAmountListDTO>>(paymentMethodAmounts);
        }
    }
}
