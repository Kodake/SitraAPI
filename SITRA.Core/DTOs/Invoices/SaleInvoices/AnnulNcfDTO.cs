namespace SITRA.Core.DTOs.Invoices.SaleInvoices
{
    /// <summary>
    /// Minimal payload to annul an existing NCF via an E34 Credit Note.
    /// ModificationCode is always 1 (Anula el NCF modificado) — hardcoded in the handler.
    /// </summary>
    public class AnnulNcfDTO
    {
        /// <summary>
        /// The eNCF of the invoice to be annulled (e.g. "E310000000001").
        /// This value is placed in &lt;InformacionReferencia&gt;&lt;NCFModificado&gt; of the DGII XML.
        /// </summary>
        public string ModifiedNcfNumber { get; set; } = string.Empty;

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
