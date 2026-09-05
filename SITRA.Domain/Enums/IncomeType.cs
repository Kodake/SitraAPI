using System.ComponentModel;

namespace SITRA.Core.Enums
{
    public enum IncomeType
    {
        [Description("Venta de Bienes")]
        saleOfGoods = 1,
        [Description("Prestación de Servicios")]
        provisionOfServices = 2,
        [Description("Arrendamiento de Bienes")]
        rentalOfGoods = 3,
        [Description("Venta de Valores")]
        saleOfSecurities = 4,
        [Description("Intereses")]
        interests = 5,
        [Description("Dividendos")]
        dividends = 6
    }
}
