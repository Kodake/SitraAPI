using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Invoices.SaleInvoices
{
    /// <summary>
    /// DTO for creating an Electronic Credit Note (E34) or Debit Note (E33).
    /// Follows the same structure as CreateSaleInvoiceDTO but:
    ///   - ConditionType and Payments are optional (notes don't always require full payment).
    ///   - Adds ModifiedNcfNumber (the NCF being corrected/cancelled) and ModificationCode.
    /// </summary>
    public class CreateNoteDTO
    {
        public Guid Id { get; set; }
        public Guid PricingId { get; set; }
        public Guid CurrencyId { get; set; }
        public Guid WarehouseId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? BillerId { get; set; }
        public DocumentType? DocumentType { get; set; }
        public Guid? FiscalSupportId { get; set; }
        public double? Shipping { get; set; }
        public double? Discount { get; set; }

        /// <summary>
        /// Optional for notes — omit for simple annulment (ModificationCode 1).
        /// </summary>
        public InvoiceCondition? ConditionType { get; set; }

        public List<PaymentDTO> Payments { get; set; } = [];
        public double? AmountReturned { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public List<SaleInvoiceItemDTO> InvoiceItems { get; set; } = [];
        public double? MontoNoFacturable { get; set; }
        public double? SaldoAnterior { get; set; }
        public double? MontoAvancePago { get; set; }
        public double? MontoPeriodo { get; set; }
        public string? TipoIngresos { get; set; }
        public int? IndicadorEnvioDiferido { get; set; }
        public string? IndicadorMontoGral { get; set; }

        // ─── Credit / Debit Note Reference ───────────────────────────────────────
        /// <summary>
        /// The fiscal sequence number (eNCF) of the invoice being modified/cancelled.
        /// Maps to &lt;NCFModificado&gt; in the DGII InformacionReferencia section.
        /// </summary>
        public string? ModifiedNcfNumber { get; set; }

        /// <summary>
        /// Reason code for the modification:
        /// 1 = Anula el NCF modificado
        /// 2 = Corrige Texto del Comprobante Fiscal modificado
        /// 3 = Corrige montos del NCF modificado
        /// 4 = Reemplazo NCF emitido en contingencia
        /// 5 = Referencia Factura Consumo Electrónica
        /// </summary>
        public int? ModificationCode { get; set; }
    }
}
