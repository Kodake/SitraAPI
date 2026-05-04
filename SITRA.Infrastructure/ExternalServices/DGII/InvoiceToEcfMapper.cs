using SITRA.Core.Entities.Invoices;
using SITRA.Core.Interfaces;
using SITRA.Core.DTOs.DGII;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;

namespace SITRA.Infrastructure.ExternalServices.DGII
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
                        // E43, E44, E47: XSD does not allow IndicadorMontoGral
                        IndicadorMontoGral = tipoEcf is "43" or "44" or "47" ? null : "0",
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
            decimal totalITBISRetenido = 0; // E41: sum of per-item MontoITBISRetenido
            decimal totalISRRetenido = 0;   // E41: sum of per-item MontoISRRetenido (services only)
            decimal montoNoFacturable = 0;  // E44: items with IndicadorFacturacion=0

            // Rastreamos qué tasas se usaron para asignar ITBIS1/ITBIS2/ITBIS3
            bool usaI1 = false, usaI2 = false, usaI3 = false;

            int lineNum = 1;

            foreach (var item in invoice.InvoiceItems)
            {
                var taxes = item.Item?.ItemTaxes ?? [];
                var (indicador, tasa) = GetIndicadorYtasa(taxes);

                // E43 and E44 (in practice): DGII only accepts IndicadorFacturacion=4 (Exento).
                if (tipoEcf == "43" || tipoEcf == "44") { indicador = 4; tasa = 0; }

                // Safety: indicador=0 (No Facturable) is only valid for E44.
                // For all other types, fall back to Exento to avoid invalid XML.
                if (tipoEcf != "44" && indicador == 0) { indicador = 4; }

                var qty = Round((decimal)item.Qty);
                var price = Round((decimal)item.BasePrice);
                var montoItem = Round(qty * price); // base neta (precio sin ITBIS × cantidad)

                switch (indicador)
                {
                    case 0: // No Facturable (E44 only)
                        montoNoFacturable += montoItem;
                        break;
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

                // E41: ITBIS retention per line
                // IndicadorFacturacion 1=18%, 2=16%, 3=0%, 4=Exento
                var montoItbisRetenido = tipoEcf == "41"
                    ? indicador switch
                    {
                        1 => Round(montoItem * (18m / 100m)),
                        2 => Round(montoItem * (16m / 100m)),
                        _ => 0m  // ITBIS3 (0%) and Exento → 0
                    }
                    : (decimal?)null;

                // E41: ISR retention per line — only for services (ItemType.Service)
                // Uses explicit ISR tax rate if configured, otherwise defaults to 10%
                var montoIsrRetenido = GetMontoIsrRetenido(taxes, item.Item?.ItemType, montoItem, tipoEcf);

                if (tipoEcf == "41" && montoItbisRetenido.HasValue)
                    totalITBISRetenido += montoItbisRetenido.Value;

                if (tipoEcf == "41" && montoIsrRetenido.HasValue)
                    totalISRRetenido += montoIsrRetenido.Value;

                ecf.Detalles.Items.Add(new Core.DTOs.DGII.Item
                {
                    NumeroLinea = lineNum++,
                    IndicadorFacturacion = indicador,
                    // E41 requires <Retencion> (minOccurs=1) with MontoITBISRetenido (error 260)
                    // MontoISRRetenido is included only for services
                    Retencion = tipoEcf == "41"
                        ? new Retencion
                        {
                            IndicadorAgenteRetencionoPercepcion = 1,
                            MontoITBISRetenido = montoItbisRetenido,
                            MontoISRRetenido = montoIsrRetenido  // null for goods, 10% for services
                        }
                        : null,
                    NombreItem = item.Item?.Name ?? "Item Generico",
                    IndicadorBienoServicio = (int?)item.Item?.ItemType ?? 1,
                    Cantidad = qty,
                    PrecioUnitario = price,
                    MontoItem = montoItem
                });
            }

            // E43, E44 and E47 XSD restrict Totales to: MontoExento (opt) + MontoTotal (req) only.
            // Sending MontoGravadoTotal or any ITBIS field causes error code 3.
            var simpleTipos = new HashSet<string> { "43", "44", "47" };
            if (simpleTipos.Contains(tipoEcf))
            {
                ecf.Encabezado.Totales = new Totales
                {
                    // MontoExento: only truly exempt items (IndicadorFacturacion=4)
                    MontoExento = montoExento > 0 ? Round(montoExento) : null,

                    // MontoTotal = MontoExento + MontoNoFacturable (no ITBIS ever charged)
                    MontoTotal = Round(montoExento + montoNoFacturable)
                };
            }
            else
            {
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
                        montoExento + totalITBIS1 + totalITBIS2 + totalITBIS3),

                    // E41: total ITBIS retained must match sum of per-line MontoITBISRetenido
                    TotalITBISRetenido = tipoEcf == "41" && totalITBISRetenido > 0
                        ? Round(totalITBISRetenido)
                        : null,

                    // E41: total ISR retained must match sum of per-line MontoISRRetenido (services only)
                    TotalISRRetencion = tipoEcf == "41" && totalISRRetenido > 0
                        ? Round(totalISRRetenido)
                        : null
                };
            }

            return ecf;
        }

        // =========================
        // HELPER ISR
        // =========================
        private decimal? GetMontoIsrRetenido(IEnumerable<ItemTaxes> taxes, ItemType? itemType, decimal montoItem, string tipoEcf)
        {
            // ISR retention only applies in E41
            if (tipoEcf != "41") return null;

            // ISR only applies to services
            if (itemType != ItemType.Service) return null;

            // Check if the item has an explicit ISR tax configured
            var isrTax = taxes.FirstOrDefault(t => t.Tax.TaxType == TaxType.ISR);
            if (isrTax != null)
                return Round(montoItem * ((decimal)isrTax.Tax.Amount / 100m));

            // Fallback: if it's a service in E41 with no ISR tax configured,
            // apply the default 10% ISR rate as required by DGII
            return Round(montoItem * (10m / 100m));
        }

        // =========================
        // HELPER INTELIGENTE
        // =========================
        private (int indicador, decimal tasa) GetIndicadorYtasa(IEnumerable<ItemTaxes> taxes)
        {
            if (taxes == null || !taxes.Any())
                return (4, 0);

            // Explicit NO_FACTURABLE tax type takes priority
            var noFacturableByType = taxes.FirstOrDefault(t => t.Tax.TaxType == TaxType.NO_FACTURABLE);
            if (noFacturableByType != null)
                return (0, 0);

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

            if (taxes.Any(t => t.Tax.TaxType == TaxType.EXENTO))
                return (4, 0);

            return (4, 0);
        }

        private decimal Round(decimal value)
            => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}