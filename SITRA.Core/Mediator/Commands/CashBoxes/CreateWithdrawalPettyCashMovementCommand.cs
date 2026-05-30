using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Entities.CashBoxes;
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

namespace SITRA.Core.Mediator.Commands.CashBoxes
{
    public class CreateWithdrawalPettyCashMovementCommand : IRequest<CreateWithdrawalPettyCashMovementDTO>
    {
        public CreateWithdrawalPettyCashMovementDTO Dto { get; set; }
        public CreateWithdrawalPettyCashMovementCommand(CreateWithdrawalPettyCashMovementDTO dto)
        {
            Dto = dto;
        }
    }

    public class CreateWithdrawalPettyCashMovementCommandHandler : IRequestHandler<CreateWithdrawalPettyCashMovementCommand, CreateWithdrawalPettyCashMovementDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IMapper _mapper;
        private readonly IPettyCashService _pettyCashService;

        public CreateWithdrawalPettyCashMovementCommandHandler(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            IPettyCashService pettyCashService)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
            _mapper = mapper;
            _pettyCashService = pettyCashService;
        }

        public async Task<CreateWithdrawalPettyCashMovementDTO> Handle(CreateWithdrawalPettyCashMovementCommand request, CancellationToken cancellationToken)
        {
            // All validations are handled by FluentValidation
            // These calls are now safe because validation ensures they exist
            var user = await _unitOfWork.UserService.GetUserAsync();
            var pettyCashDB = await _unitOfWork.PettyCashRepository.GetOpenPettyCashByUserIdAsync(user!.Id);

            var movement = new PettyCashMovement
            {
                PettyCashId = pettyCashDB!.Id,
                Amount = request.Dto.WithdrawalAmount,
                Type = MovementType.CashWithdrawals,
                InvoiceId = null,
                Invoice = null,
            };

            await _unitOfWork.PettyCashMovementRepository.CreateAsync(movement);

            var totalWithdrawals = await _pettyCashService.GetTotalWithdrawalsAsync(pettyCashDB.Id, cancellationToken);

            pettyCashDB.ClosingAmount -= request.Dto.WithdrawalAmount;
            pettyCashDB.WithdrawalAmount = totalWithdrawals;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _outputCacheStore.EvictByTagAsync("pettyCashes-get", cancellationToken);
            await _outputCacheStore.EvictByTagAsync("pettyCashMovements-get", cancellationToken);

            return request.Dto;
        }
    }
}

