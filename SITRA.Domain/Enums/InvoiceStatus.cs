namespace SITRA.Core.Enums
{
    public enum InvoiceStatus
    {
        /// <summary>
        /// Factura válida / activa (completada con éxito)
        /// </summary>
        Active = 1,

        /// <summary>
        /// En proceso de validación o emisión
        /// </summary>
        Pending = 2,

        /// <summary>
        /// Rechazada por DGII o por validación fiscal
        /// </summary>
        Rejected = 3,

        /// <summary>
        /// Anulada / Cancelada por Nota de Crédito o anulación directa
        /// </summary>
        Cancelled = 4,

        /// <summary>
        /// Fallo interno durante la emisión
        /// </summary>
        Failed = 5
    }
}
