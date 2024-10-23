using BackEnd.Entities.AdministrativeTools;
using BackEnd.Entities.Contacts;
using BackEnd.Entities.Products;
using BackEnd.Enums;

namespace BackEnd.Entities.Invoices
{
    public sealed class SaleInvoice
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public Pricing Pricing { get; set; } = null!;
        public int BillerId { get; set; }
        public Contact Biller { get; set; } = null!;
        public int BuyerId { get; set; }
        public Contact Buyer { get; set; } = null!;
        public string FiscalSupportNumber
        {
            get
            {
                // 1. Obtenemos el nombre del tipo fiscal
                string fiscalSupportName = Enum.GetName(typeof(FiscalSupportType), FiscalSupport.FiscalSupportType) ?? "Unknown";

                // 2. Aseguramos que el símbolo tenga al menos un carácter válido
                char symbolChar = FiscalSupport.Symbol.Length > 0 ? FiscalSupport.Symbol[0] : '?';

                // 3. Repetimos el símbolo tantas veces como indique SpaceNumber
                string repeatedSymbols = new string(symbolChar, FiscalSupport.SpaceNumber);

                // 4. Convertimos InitialSequence a string y lo ajustamos a la longitud de SpaceNumber
                string sequence = FiscalSupport.InitialSequence.ToString().PadLeft(FiscalSupport.SpaceNumber, '0');

                // 5. Reemplazamos los últimos n símbolos por la secuencia completa
                string finalSymbols = repeatedSymbols[..^FiscalSupport.SpaceNumber] + sequence;

                // 6. Retornamos el resultado final concatenado
                return $"{fiscalSupportName} {finalSymbols}";
            }
        }
        public double TotalQtyItems
        {
            get
            {
                return SaleInvoiceItems.Sum(x => x.ItemId);
            }
        }
        public double TotalItems
        {
            get
            {
                return SaleInvoiceItems.Sum(x => x.ItemId);
            }
        }
        public double TotalAmount
        {
            get
            {
                return SaleInvoiceItems.Sum(x => x.TotalPrice);
            }
        }
        public double TotalTaxes
        {
            get
            {
                return SaleInvoiceItems.Sum(x => x.ItemTaxes.Sum(it => it.Tax.Amount * x.Qty));
            }
        }
        public bool IsPaid { get; set; } = false;
        public InvoiceStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public int FiscalSupportId { get; set; }
        public FiscalSupport FiscalSupport { get; set; } = null!;
        public int DeadlineId { get; set; }
        public Deadline Deadline { get; set; } = null!;
        public ICollection<SaleInvoiceItem> SaleInvoiceItems { get; set; } = [];
    }
}
