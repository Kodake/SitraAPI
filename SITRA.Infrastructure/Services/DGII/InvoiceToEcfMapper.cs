using SITRA.Core.Entities.Invoices;
using SITRA.Core.Interfaces;
using SITRA.Core.DTOs.DGII;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;

namespace SITRA.Infrastructure.Services.DGII
{
    public class InvoiceToEcfMapper : IInvoiceToEcfMapper
    {
        public EcfDocument MapToDgiiDocument(Invoice invoice)
        {
            if (string.IsNullOrEmpty(invoice.FiscalSupportNumber) || invoice.FiscalSupportNumber.Length < 3)
                throw new Exception("FiscalSupportNumber is invalid or missing.");

            string tipoEcf = invoice.FiscalSupportNumber.Substring(1, 2);

            return BuildEcfDocument(invoice, tipoEcf);
        }

        private EcfDocument BuildEcfDocument(Invoice invoice, string tipoEcf)
        {
            var biller = invoice.InvoiceBiller?.Biller;
            var buyer = invoice.InvoiceBuyer?.Buyer;

            var ecf = new EcfDocument
            {
                Encabezado = new Encabezado
                {
                    IdDoc = new IdDoc
                    {
                        TipoeCF = tipoEcf,
                        ENCF = invoice.FiscalSupportNumber!,
                        FechaVencimientoSecuencia = "31-12-2028",
                        IndicadorMontoGral = "0",
                        TipoIngresos = "01",
                        TipoPago = invoice.ConditionType == InvoiceCondition.Spot ? "1" : "2"
                    },
                    Emisor = new Emisor
                    {
                        RNCEmisor = biller?.Identification ?? "133556545",
                        RazonSocial = biller?.Name ?? "SISTRA SRL",
                        DireccionEmisor = biller?.Address ?? "N/A",
                        Municipio = biller?.Municipality,
                        Provincia = biller?.Province,
                        CorreoEmisor = biller?.Email,
                        FechaEmision = invoice.Date.ToString("dd-MM-yyyy")
                    },
                    Comprador = new Comprador
                    {
                        RNCComprador = buyer?.Identification,
                        RazonSocialComprador = buyer?.Name ?? "CONSUMIDOR FINAL",
                        DireccionComprador = buyer?.Address ?? "N/A",
                        MunicipioComprador = buyer?.Municipality,
                        ProvinciaComprador = buyer?.Province,
                        CorreoComprador = buyer?.Email
                    }
                }
            };

            // =========================
            // ACUMULADORES
            // =========================
            decimal montoGravadoI1 = 0, montoGravadoI2 = 0, montoGravadoI3 = 0, montoExento = 0;
            decimal totalITBIS1 = 0, totalITBIS2 = 0, totalITBIS3 = 0;

            // Rastreamos qué tasas se usaron para asignar ITBIS1/ITBIS2/ITBIS3
            bool usaI1 = false, usaI2 = false, usaI3 = false;

            int lineNum = 1;

            foreach (var item in invoice.InvoiceItems)
            {
                var taxes = item.Item?.ItemTaxes ?? [];
                var (indicador, tasa) = GetIndicadorYtasa(taxes);

                var qty = Round((decimal)item.Qty);
                var price = Round((decimal)item.BasePrice);
                var montoItem = Round(qty * price); // base neta (precio sin ITBIS × cantidad)

                switch (indicador)
                {
                    case 1:
                        montoGravadoI1 += montoItem;
                        totalITBIS1 += Round(montoItem * (tasa / 100m));
                        usaI1 = true;
                        break;
                    case 2:
                        montoGravadoI2 += montoItem;
                        totalITBIS2 += Round(montoItem * (tasa / 100m));
                        usaI2 = true;
                        break;
                    case 3:
                        montoGravadoI3 += montoItem;
                        usaI3 = true;
                        break;
                    case 4:
                        montoExento += montoItem;
                        break;
                }

                ecf.Detalles.Items.Add(new SITRA.Core.DTOs.DGII.Item
                {
                    NumeroLinea = lineNum++,
                    IndicadorFacturacion = indicador,
                    NombreItem = item.Item?.Name ?? "Item Generico",
                    IndicadorBienoServicio = (int?)item.Item?.ItemType ?? 1,
                    Cantidad = qty,
                    PrecioUnitario = price,
                    MontoItem = montoItem
                });
            }

            ecf.Encabezado.Totales = new Totales
            {
                // Tasas porcentuales (campos int en el DTO — indican qué tasa aplica)
                ITBIS1 = usaI1 ? 18 : null,
                ITBIS2 = usaI2 ? 16 : null,
                ITBIS3 = usaI3 ? 0 : null,

                MontoGravadoTotal = Round(montoGravadoI1 + montoGravadoI2 + montoGravadoI3),

                MontoGravadoI1 = montoGravadoI1 > 0 ? Round(montoGravadoI1) : null,
                MontoGravadoI2 = montoGravadoI2 > 0 ? Round(montoGravadoI2) : null,
                MontoGravadoI3 = montoGravadoI3 > 0 ? Round(montoGravadoI3) : null,

                MontoExento = montoExento > 0 ? Round(montoExento) : null,

                TotalITBIS = Round(totalITBIS1 + totalITBIS2 + totalITBIS3),
                TotalITBIS1 = totalITBIS1 > 0 ? Round(totalITBIS1) : null,
                TotalITBIS2 = totalITBIS2 > 0 ? Round(totalITBIS2) : null,
                TotalITBIS3 = totalITBIS3 > 0 ? Round(totalITBIS3) : null,

                MontoTotal = Round(
                    montoGravadoI1 + montoGravadoI2 + montoGravadoI3 +
                    montoExento + totalITBIS1 + totalITBIS2 + totalITBIS3
                )
            };

            return ecf;
        }

        // =========================
        // HELPER INTELIGENTE
        // =========================
        private (int indicador, decimal tasa) GetIndicadorYtasa(IEnumerable<ItemTaxes> taxes)
        {
            if (taxes == null || !taxes.Any())
                return (4, 0);

            var itbisByType = taxes.FirstOrDefault(t => t.Tax.TaxType == TaxType.ITBIS);

            if (itbisByType != null)
            {
                var rate = (decimal)itbisByType.Tax.Amount;
                if (rate == 18) return (1, 18);
                if (rate == 16) return (2, 16);
                if (rate == 0) return (3, 0);
            }

            var itbisByName = taxes.FirstOrDefault(t => t.Tax.Name.StartsWith("ITBIS"));

            if (itbisByName != null)
            {
                return itbisByName.Tax.Name switch
                {
                    "ITBIS1" => (1, 18),
                    "ITBIS2" => (2, 16),
                    "ITBIS3" => (3, 0),
                    _ => (4, 0)
                };
            }

            if (taxes.Any(t => t.Tax.Name == "Exento"))
                return (4, 0);

            return (4, 0);
        }

        private decimal Round(decimal value)
            => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}