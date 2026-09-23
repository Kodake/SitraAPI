using System.Text.Json;
using Mediator;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Logging;
using SITRA.Core.DTOs.Invoices.LogInvoices;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services;
using SITRA.Core.Mediator.Commands.Invoices.LogInvoices;

namespace SITRA.Core.Mediator.Handlers.Invoices
{
    public class SyncDgiiStatusCommandHandler :
        IRequestHandler<SyncDgiiInvoiceStatusCommand, SyncDgiiStatusResultDTO>,
        IRequestHandler<SyncPendingDgiiInvoicesCommand, List<SyncDgiiStatusResultDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDgiiApiService _dgiiApiService;
        private readonly ISignatureService _signatureService;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly ILogger<SyncDgiiStatusCommandHandler> _logger;

        public SyncDgiiStatusCommandHandler(
            IUnitOfWork unitOfWork,
            IDgiiApiService dgiiApiService,
            ISignatureService signatureService,
            IOutputCacheStore outputCacheStore,
            ILogger<SyncDgiiStatusCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _dgiiApiService = dgiiApiService;
            _signatureService = signatureService;
            _outputCacheStore = outputCacheStore;
            _logger = logger;
        }

        public async ValueTask<SyncDgiiStatusResultDTO> Handle(SyncDgiiInvoiceStatusCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _unitOfWork.LogInvoiceRepository.GetInvoiceForSyncAsync(request.InvoiceId, cancellationToken);
            if (invoice == null)
            {
                return new SyncDgiiStatusResultDTO
                {
                    InvoiceId = request.InvoiceId,
                    Success = false,
                    NotFound = true,
                    Message = SITRA.Core.Helpers.Utilities.InvoiceNotFoundMessage
                };
            }

            if (string.IsNullOrWhiteSpace(invoice.DgiiTrackId))
            {
                return new SyncDgiiStatusResultDTO
                {
                    InvoiceId = invoice.Id,
                    FiscalSupportNumber = invoice.FiscalSupportNumber,
                    PreviousDgiiStatus = invoice.DgiiStatus,
                    NewDgiiStatus = invoice.DgiiStatus,
                    InvoiceStatus = invoice.Status,
                    Success = false,
                    Message = SITRA.Core.Helpers.Utilities.InvoiceMissingTrackIdMessage
                };
            }

            try
            {
                var token = await GetDgiiTokenForTenantAsync(invoice.TenantId, cancellationToken);
                var result = await SyncSingleInvoiceCoreAsync(invoice, token, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await InvalidateCachesAsync(cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al sincronizar factura {InvoiceId} con DGII", invoice.Id);
                return new SyncDgiiStatusResultDTO
                {
                    InvoiceId = invoice.Id,
                    FiscalSupportNumber = invoice.FiscalSupportNumber,
                    PreviousDgiiStatus = invoice.DgiiStatus,
                    NewDgiiStatus = invoice.DgiiStatus,
                    InvoiceStatus = invoice.Status,
                    Success = false,
                    Message = SITRA.Core.Helpers.Utilities.InvoiceSyncErrorMessage(ex.Message)
                };
            }
        }

        public async ValueTask<List<SyncDgiiStatusResultDTO>> Handle(SyncPendingDgiiInvoicesCommand request, CancellationToken cancellationToken)
        {
            var results = new List<SyncDgiiStatusResultDTO>();
            var pendingInvoices = await _unitOfWork.LogInvoiceRepository.GetPendingSyncInvoicesAsync(request.TenantId, cancellationToken);

            if (pendingInvoices.Count == 0)
            {
                _logger.LogInformation("No se encontraron facturas pendientes de sincronización con DGII.");
                return results;
            }

            _logger.LogInformation("Iniciando sincronización en lote para {Count} facturas pendientes.", pendingInvoices.Count);
            var tokenCache = new Dictionary<Guid, string>();
            bool anyUpdated = false;

            foreach (var invoice in pendingInvoices)
            {
                try
                {
                    if (!tokenCache.TryGetValue(invoice.TenantId, out var token))
                    {
                        token = await GetDgiiTokenForTenantAsync(invoice.TenantId, cancellationToken);
                        tokenCache[invoice.TenantId] = token;
                    }

                    var result = await SyncSingleInvoiceCoreAsync(invoice, token, cancellationToken);
                    results.Add(result);
                    if (result.Success)
                    {
                        anyUpdated = true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error sincronizando factura {Id} (e-NCF: {Ncf})", invoice.Id, invoice.FiscalSupportNumber);
                    results.Add(new SyncDgiiStatusResultDTO
                    {
                        InvoiceId = invoice.Id,
                        FiscalSupportNumber = invoice.FiscalSupportNumber,
                        PreviousDgiiStatus = invoice.DgiiStatus,
                        NewDgiiStatus = invoice.DgiiStatus,
                        InvoiceStatus = invoice.Status,
                        Success = false,
                        Message = ex.Message
                    });
                }
            }

            if (anyUpdated)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await InvalidateCachesAsync(cancellationToken);
            }

            return results;
        }

        private async Task<SyncDgiiStatusResultDTO> SyncSingleInvoiceCoreAsync(Invoice invoice, string token, CancellationToken cancellationToken)
        {
            var prevStatus = invoice.DgiiStatus;
            var prevInvoiceStatus = invoice.Status;

            var statusResponse = await _dgiiApiService.CheckStatusAsync(invoice.DgiiTrackId!, token);
            if (string.IsNullOrWhiteSpace(statusResponse))
            {
                return new SyncDgiiStatusResultDTO
                {
                    InvoiceId = invoice.Id,
                    FiscalSupportNumber = invoice.FiscalSupportNumber,
                    PreviousDgiiStatus = prevStatus,
                    NewDgiiStatus = prevStatus,
                    InvoiceStatus = invoice.Status,
                    Success = false,
                    Message = SITRA.Core.Helpers.Utilities.DgiiNoResponseMessage
                };
            }

            string newStatus = prevStatus;
            try
            {
                using var jsonDoc = JsonDocument.Parse(statusResponse);
                if (jsonDoc.RootElement.TryGetProperty("estado", out var estadoElem))
                {
                    newStatus = estadoElem.GetString() ?? prevStatus;
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Respuesta no JSON al consultar trackId {TrackId}", invoice.DgiiTrackId);
            }

            invoice.DgiiStatus = newStatus;
            invoice.DgiiResponse = statusResponse;
            invoice.UpdatedAt = DateTimeOffset.UtcNow;

            // Map DGII Status to SITRA InvoiceStatus
            if (newStatus.Contains("Aceptad", StringComparison.OrdinalIgnoreCase))
            {
                invoice.Status = InvoiceStatus.Active;
            }
            else if (newStatus.Contains("Rechazad", StringComparison.OrdinalIgnoreCase))
            {
                invoice.Status = InvoiceStatus.Rejected;
            }
            else if (newStatus.Contains("Anulad", StringComparison.OrdinalIgnoreCase))
            {
                invoice.Status = InvoiceStatus.Cancelled;
            }
            else if (newStatus.Contains("En Proceso", StringComparison.OrdinalIgnoreCase) ||
                     newStatus.Contains("Pendiente", StringComparison.OrdinalIgnoreCase))
            {
                invoice.Status = InvoiceStatus.Pending;
            }

            _unitOfWork.LogInvoiceRepository.Update(invoice);

            return new SyncDgiiStatusResultDTO
            {
                InvoiceId = invoice.Id,
                FiscalSupportNumber = invoice.FiscalSupportNumber,
                PreviousDgiiStatus = prevStatus,
                NewDgiiStatus = newStatus,
                DgiiResponse = statusResponse,
                InvoiceStatus = invoice.Status,
                Success = true,
                Message = SITRA.Core.Helpers.Utilities.InvoiceStatusUpdatedMessage(newStatus)
            };
        }

        private async Task<string> GetDgiiTokenForTenantAsync(Guid tenantId, CancellationToken cancellationToken)
        {
            var (certBytes, certPassword) = await _unitOfWork.UserRoleService.GetTenantCertificateByTenantIdAsync(tenantId, cancellationToken);
            string seedXml = await _dgiiApiService.GetSemillaAsync();
            string signedSeed = _signatureService.SignAuthenticationSeed(seedXml, certBytes, certPassword);
            return await _dgiiApiService.GetTokenAsync(signedSeed);
        }

        private async Task InvalidateCachesAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _outputCacheStore.EvictByTagAsync("logInvoices-get", cancellationToken);
                await _outputCacheStore.EvictByTagAsync("saleInvoices-get", cancellationToken);
                await _outputCacheStore.EvictByTagAsync("dashboard-get", cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al invalidar caché tras sincronización");
            }
        }
    }
}
