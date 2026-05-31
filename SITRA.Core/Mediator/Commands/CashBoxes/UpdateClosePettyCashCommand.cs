using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using QuestPDF.Infrastructure;
using SITRA.Core.Documents;
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
using MediatR;
using System.IO;

namespace SITRA.Core.Mediator.Commands.CashBoxes
{
    public class UpdateClosePettyCashCommand : IRequest<(byte[] FileBytes, string FileName)?>
    {
        public Guid Id { get; set; }
        public CreateClosePettyCashDTO Dto { get; set; }

        public UpdateClosePettyCashCommand(Guid id, CreateClosePettyCashDTO dto)
        {
            Id = id;
            Dto = dto;
        }
    }

    public class UpdateClosePettyCashCommandHandler : IRequestHandler<UpdateClosePettyCashCommand, (byte[] FileBytes, string FileName)?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IMapper _mapper;
        private readonly IPettyCashService _pettyCashService;

        public UpdateClosePettyCashCommandHandler(
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

        public async Task<(byte[] FileBytes, string FileName)?> Handle(
            UpdateClosePettyCashCommand request,
            CancellationToken cancellationToken)
        {
            // All validations are handled by FluentValidation before this action is executed
            var user = await _unitOfWork.UserService.GetUserAsync();

            // ?? Get the current open petty cash
            var pettyCashDB = await _unitOfWork.PettyCashRepository.GetOpenPettyCashByUserIdAsync(user!.Id);

            // ?? Update fields
            pettyCashDB.Comments = request.Dto.Comments;
            pettyCashDB.ClosingDate = DateTimeOffset.UtcNow;
            pettyCashDB.Status = PettyCashStatus.Closed;

            // ?? Compute movement totals
            var paymentMethods = await _pettyCashService
                .GetPaymentMethodSumsAsync(pettyCashDB.Id, cancellationToken);
            var totalWithdrawals = await _pettyCashService.GetTotalWithdrawalsAsync(pettyCashDB.Id, cancellationToken);

            // ?? Map to DTO
            var pettyCashDto = _mapper.Map<PettyCashDTO>(pettyCashDB);

            pettyCashDto.PaymentMethods = paymentMethods
                .Where(kvp => kvp.Key is PaymentMethod)
                .Select(kvp => new PaymentMethodAmountListDTO
                {
                    MethodType = (PaymentMethod)kvp.Key,
                    Amount = kvp.Value
                })
                .ToList();

            pettyCashDto.CashBilled = paymentMethods.TryGetValue(PaymentMethod.Cash, out var cash)
                ? cash
                : 0;

            pettyCashDto.TotalBilled = pettyCashDto.PaymentMethods.Sum(p => p.Amount);
            pettyCashDto.WithdrawalAmount = totalWithdrawals;
            pettyCashDto.ClosingAmount =
                pettyCashDto.OpeningAmount + pettyCashDto.CashBilled - pettyCashDto.WithdrawalAmount;

            pettyCashDB.ClosingAmount = pettyCashDto.ClosingAmount;

            // ?? Save updates
            _unitOfWork.PettyCashRepository.Update(pettyCashDB);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _outputCacheStore.EvictByTagAsync("pettyCashes-get", cancellationToken);

            // ?? Generate PDF and return it
            var pdfDoc = new ClosePettyCashDocument(pettyCashDto);
            var fileName = $"ClosePettyCash_{DateTimeOffset.UtcNow:yyyyMMdd_HHmmss}.pdf";

            var fileBytes = pdfDoc.GenerateToBytes();

            return (fileBytes, fileName);
        }
    }
}
