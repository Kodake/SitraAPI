namespace SITRA.Core.DTOs.Invoices.SaleInvoices
{
    /// <summary>
    /// Minimal payload to annul an existing NCF via an E34 Credit Note.
    /// ModificationCode is always 1 (Anula el NCF modificado) — hardcoded in the handler.
    /// </summary>
    public class AnnulNcfDTO
    {
        /// <summary>
        /// The invoice to fetch information from to be annulled.
        /// </summary>
        public Guid InvoiceId { get; set; }

        /// <summary>
        /// The eNCF of the invoice to be annulled (populated internally via InvoiceId).
        /// </summary>
        public string? ModifiedNcfNumber { get; set; }

        /// <summary>
        /// The E34 fiscal support sequence to draw from for the new credit note eNCF.
        /// </summary>
        public Guid FiscalSupportId { get; set; }

        /// <summary>
        /// The resulting credit note eNCF — populated by the handler and returned in the response.
        /// </summary>
        public string? CreditNoteNcf { get; set; }

        /// <summary>
        /// DGII processing status (Aceptado / Rechazado / Error).
        /// </summary>
        public string? DgiiStatus { get; set; }

        /// <summary>
        /// Raw DGII JSON response for debugging.
        /// </summary>
        public string? DgiiResponse { get; set; }
    }
}
