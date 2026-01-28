using SITRA.Core.DTOs.Finances.Currencies;
using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.DTOs.Warehouses.Warehouses;

namespace SITRA.Core.DTOs.Wizard
{
    public class WizardDTO
    {
        public CreateCurrencyDTO Currency { get; set; } = null!;
        public CreateTaxDTO Tax { get; set; } = null!;
        public WizardProductDTO Product { get; set; } = null!;
        public CreateWarehouseDTO Warehouse { get; set; } = null!;
        public WizardStockDTO Stock { get; set; } = null!;
    }

    public class WizardProductDTO
    {
        public string Name { get; set; } = string.Empty;
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
        public List<Guid> TaxIds { get; set; } = null!;
    }

    public class WizardStockDTO
    {
        public double Qty { get; set; }
    }
}
