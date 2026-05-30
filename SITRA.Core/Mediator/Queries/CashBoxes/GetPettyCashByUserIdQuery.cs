using AutoMapper;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
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
    public class GetPettyCashByUserIdQuery : IRequest<PettyCashDTO?>
    {
    }

    public class GetPettyCashByUserIdQueryHandler : IRequestHandler<GetPettyCashByUserIdQuery, PettyCashDTO?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPettyCashService _pettyCashService;

        public GetPettyCashByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IPettyCashService pettyCashService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _pettyCashService = pettyCashService;
        }

        public async Task<PettyCashDTO?> Handle(GetPettyCashByUserIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserService.GetUserAsync();
            if (user == null)
                return null;

            var pettyCash = await _unitOfWork.PettyCashRepository.GetAllByUserIdAsync(user.Id);
            if (pettyCash == null)
                return null;

            var paymentMethods = await _pettyCashService.GetPaymentMethodSumsAsync(pettyCash.Id, cancellationToken);

            var totalWithdrawals = await _pettyCashService.GetTotalWithdrawalsAsync(pettyCash.Id, cancellationToken);

            var dto = _mapper.Map<PettyCashDTO>(pettyCash);

            dto.PaymentMethods = paymentMethods
                .Where(kvp => kvp.Key is PaymentMethod)
                .Select(kvp => new PaymentMethodAmountListDTO
                {
                    MethodType = (PaymentMethod)kvp.Key,
                    Amount = kvp.Value
                })
                .ToList();

            dto.CashBilled = paymentMethods.TryGetValue(PaymentMethod.Cash, out var cash)
                ? cash
                : 0;

            dto.TotalBilled = dto.PaymentMethods.Sum(p => p.Amount);

            dto.WithdrawalAmount = totalWithdrawals;

            return dto;
        }
    }
}

