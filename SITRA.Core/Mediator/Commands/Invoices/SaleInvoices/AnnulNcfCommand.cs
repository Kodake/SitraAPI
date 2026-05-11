using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Invoices.SaleInvoices
{
    /// <summary>
    /// Command that orchestrates persisting an E34 Credit Note record in database as an annulment reference.
    /// It mimics core values from the original invoice, reserves the new fiscal sequence, and kicks off optional DGII transmission.
    /// </summary>
    public class AnnulNcfCommand(AnnulNcfDTO dto) : CreateCommand<Invoice, AnnulNcfDTO>(dto)
    {
        /// <summary>
        /// Delegate hooked up by the external handler to perform signed XML transmission without bloating this command with infra dependencies.
        /// </summary>
        public Func<Invoice, Task<(string trackId, string status, string securityCode, string rawResponse)>>? DgiiProcess { get; set; }

        public override async Task<AnnulNcfDTO> ExecuteAction(
            IUnitOfWork unitOfWork, 
            IOutputCacheStore outputCacheStore, 
            IMapper mapper, 
            CancellationToken cancellationToken)
        {
            // ── 1. Resolve original context ───────────────────────────────────────────
            var originalInvoice = await unitOfWork.SaleInvoiceRepository.GetByIdAsync(Dto.InvoiceId, cancellationToken);
            
            if (originalInvoice == null)
                throw new InvalidOperationException($"Original invoice {Dto.InvoiceId} not found.");

            if (string.IsNullOrWhiteSpace(originalInvoice.FiscalSupportNumber))
                throw new InvalidOperationException("The provided invoice has no associated fiscal support number (NCF) to annul.");

            Dto.ModifiedNcfNumber = originalInvoice.FiscalSupportNumber;

            // ── 2. Increment sequences & prep NCF ─────────────────────────────────────
            var fiscalSupport = await unitOfWork.FiscalSupportRepository.GetByIdAsync(Dto.FiscalSupportId);
            
            if (fiscalSupport == null)
                throw new InvalidOperationException($"Fiscal support {Dto.FiscalSupportId} not found.");

            if (fiscalSupport.FiscalSupportType != EcfType.E34)
                throw new InvalidOperationException("Provided fiscal support must be E34 type (Credit Note).");

            fiscalSupport.IncrementSequence();
            string creditNoteNcf = fiscalSupport.GenerateFiscalSupportNumber();
            Dto.CreditNoteNcf = creditNoteNcf;

            // ── 3. Define & Map new Annul Invoice Row ───────────────────────────────
            var annulInvoice = new Invoice
            {
                Id = Guid.NewGuid(),
                TenantId = originalInvoice.TenantId,
                InvoiceType = InvoiceType.Sale,
                Date = DateTimeOffset.Now,
                DueDate = DateTimeOffset.Now,
                PricingId = originalInvoice.PricingId,
                CurrencyId = originalInvoice.CurrencyId,
                WarehouseId = originalInvoice.WarehouseId,
                ConditionType = originalInvoice.ConditionType,
                FiscalSupportNumber = creditNoteNcf,
                ModifiedNcfNumber = originalInvoice.FiscalSupportNumber,
                ModificationCode = 1, // Anula NCF
                TotalAmount = originalInvoice.TotalAmount,
                TotalTaxes = originalInvoice.TotalTaxes,
                TotalPrice = originalInvoice.TotalPrice,
                TotalItems = originalInvoice.TotalItems,
                TotalQtyItems = originalInvoice.TotalQtyItems,
                Shipping = originalInvoice.Shipping,
                Discount = originalInvoice.Discount,
                Description = $"Nota de crédito generada para anular la factura {originalInvoice.FiscalSupportNumber}",
                Note = originalInvoice.Note,
                CreatedAt = DateTimeOffset.Now
            };

            // Connect derived references from original
            if (originalInvoice.InvoiceBuyer != null)
                annulInvoice.InvoiceBuyer = new InvoiceBuyer { ContactId = originalInvoice.InvoiceBuyer.ContactId };
            
            if (originalInvoice.InvoiceBiller != null)
                annulInvoice.InvoiceBiller = new InvoiceBiller { ContactId = originalInvoice.InvoiceBiller.ContactId };

            annulInvoice.InvoiceFiscalSupport = new InvoiceFiscalSupport { FiscalSupportId = fiscalSupport.Id };

            // Save base entities so we have IDs for any downstream tracking
            unitOfWork.FiscalSupportRepository.Update(fiscalSupport);
            await unitOfWork.SaleInvoiceRepository.CreateAsync(annulInvoice);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // ── 4. Execute Handover Workflow (DGII Communication) ─────────────────────
            if (DgiiProcess != null)
            {
                var (trackId, status, securityCode, rawResponse) = await DgiiProcess(annulInvoice);

                // Return response in DTO
                Dto.DgiiStatus = status;
                Dto.DgiiResponse = rawResponse;

                // Finalize entity writeback
                annulInvoice.DgiiStatus = status;
                annulInvoice.DgiiResponse = rawResponse;
                annulInvoice.DgiiTrackId = string.IsNullOrEmpty(trackId) ? null : trackId;
                annulInvoice.SecurityCode = securityCode;

                unitOfWork.SaleInvoiceRepository.Update(annulInvoice);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Clear standardized system caches
            await outputCacheStore.EvictByTagAsync("fiscal-supports-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("sale-invoices-get", cancellationToken);

            return Dto;
        }
    }
}
