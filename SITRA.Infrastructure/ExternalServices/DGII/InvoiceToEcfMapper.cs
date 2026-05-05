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
                        IndicadorMontoGral = (tipoEcf is "43" or "44" or "46" or "47") ? null : (invoice.IndicadorMontoGral ?? "0"),
                        IndicadorEnvioDiferido = (invoice.IndicadorEnvioDiferido == 1) ? 1 : (int?)null,
                        TipoIngresos = (invoice.TipoIngresos ?? "01"),
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
                        RazonSocialComprador = buyer?.Name ?? (tipoEcf == "45" ? "ENTIDAD GUBERNAMENTAL" : "CONSUMIDOR FINAL"),
                        DireccionComprador = buyer?.Address ?? "N/A",
                        MunicipioComprador = buyer?.Municipality,
                        ProvinciaComprador = buyer?.Province,
                        CorreoComprador = buyer?.Email
                    }
                }
            };

            if ((tipoEcf == "31" || tipoEcf == "41" || tipoEcf == "44" || tipoEcf == "45") && string.IsNullOrEmpty(buyer?.Identification))
                throw new Exception($"RNC Comprador is mandatory for e-CF type {tipoEcf}.");

            if (tipoEcf == "46" && string.IsNullOrEmpty(buyer?.Name))
                throw new Exception("Razon Social Comprador is mandatory for E46 (Exportaciones).");

            decimal montoGravadoI1 = 0, montoGravadoI2 = 0, montoGravadoI3 = 0, montoExento = 0;
            decimal montoNoFacturableTotal = 0;
            decimal totalITBIS1 = 0, totalITBIS2 = 0, totalITBIS3 = 0;
            decimal totalITBISRetenido = 0;
            decimal totalISRRetenido = 0;

            bool usaI1 = false, usaI2 = false, usaI3 = false;

            int lineNum = 1;

            foreach (var item in invoice.InvoiceItems)
            {
                var taxes = item.Item?.ItemTaxes ?? [];
                var (indicador, tasa) = GetIndicadorYtasa(taxes);

                if (tipoEcf == "43" || tipoEcf == "44") { indicador = 4; tasa = 0; }
                if (tipoEcf == "46") { indicador = 3; tasa = 0; }

                if (tipoEcf != "44" && tipoEcf != "45" && tipoEcf != "46" && indicador == 0)
                    indicador = 4;

                var qty = Round((decimal)item.Qty);
                var price = Round((decimal)item.BasePrice);
                var montoItem = Round(qty * price);

                switch (indicador)
                {
                    case 0:
                        montoNoFacturableTotal += montoItem;
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

                var montoItbisRetenido = tipoEcf == "41"
                    ? indicador switch
                    {
                        1 => Round(montoItem * (18m / 100m)),
                        2 => Round(montoItem * (16m / 100m)),
                        _ => 0m
                    }
                    : (decimal?)null;

                var montoIsrRetenido = GetMontoIsrRetenido(taxes, item.Item?.ItemType, montoItem, tipoEcf);

                if (tipoEcf == "41" && montoItbisRetenido.HasValue)
                    totalITBISRetenido += montoItbisRetenido.Value;

                if (tipoEcf == "41" && montoIsrRetenido.HasValue)
                    totalISRRetenido += montoIsrRetenido.Value;

                ecf.Detalles.Items.Add(new Core.DTOs.DGII.Item
                {
                    NumeroLinea = lineNum++,
                    IndicadorFacturacion = indicador,
                    Retencion = tipoEcf == "41"
                        ? new Retencion
                        {
                            IndicadorAgenteRetencionoPercepcion = 1,
                            MontoITBISRetenido = montoItbisRetenido,
                            MontoISRRetenido = montoIsrRetenido
                        }
                        : null,
                    NombreItem = item.Item?.Name ?? "Item Generico",
                    IndicadorBienoServicio = (int?)item.Item?.ItemType ?? 1,
                    Cantidad = qty,
                    PrecioUnitario = price,
                    MontoItem = montoItem
                });
            }

            var simpleTipos = new HashSet<string> { "43", "44", "47" };

            if (simpleTipos.Contains(tipoEcf))
            {
                ecf.Encabezado.Totales = new Totales
                {
                    CurrentTipoeCF = tipoEcf,
                    MontoExento = montoExento > 0 ? Round(montoExento) : null,
                    MontoTotal = Round(montoExento + montoNoFacturableTotal)
                };
            }
            else
            {
                ecf.Encabezado.Totales = new Totales
                {
                    CurrentTipoeCF = tipoEcf,

                    ITBIS1 = (tipoEcf != "46" && usaI1) ? 18 : null,
                    ITBIS2 = (tipoEcf != "46" && usaI2) ? 16 : null,
                    ITBIS3 = usaI3 ? 0 : null,

                    MontoGravadoTotal = Round(montoGravadoI1 + montoGravadoI2 + montoGravadoI3),

                    MontoGravadoI1 = (tipoEcf != "46" && usaI1) ? Round(montoGravadoI1) : null,
                    MontoGravadoI2 = (tipoEcf != "46" && usaI2) ? Round(montoGravadoI2) : null,
                    MontoGravadoI3 = usaI3 ? Round(montoGravadoI3) : null,

                    MontoExento = (tipoEcf == "46") ? null : (montoExento > 0 ? Round(montoExento) : null),

                    TotalITBIS = Round(totalITBIS1 + totalITBIS2 + totalITBIS3),
                    TotalITBIS1 = (tipoEcf != "46" && usaI1) ? Round(totalITBIS1) : null,
                    TotalITBIS2 = (tipoEcf != "46" && usaI2) ? Round(totalITBIS2) : null,
                    TotalITBIS3 = usaI3 ? Round(totalITBIS3) : null,

                    MontoTotal = Round(montoGravadoI1 + montoGravadoI2 + montoGravadoI3 + montoExento + totalITBIS1 + totalITBIS2 + totalITBIS3),

                    TotalITBISRetenido = tipoEcf == "41"
                        ? Round(totalITBISRetenido)  // siempre incluir para E41, aunque sea 0
                        : null,

                    TotalISRRetencion = tipoEcf == "41" && totalISRRetenido > 0
                        ? Round(totalISRRetenido)
                        : null,

                    MontoNoFacturable = tipoEcf == "45" && montoNoFacturableTotal > 0
                        ? Round(montoNoFacturableTotal)
                        : null,

                    SaldoAnterior = tipoEcf == "45" && invoice.SaldoAnterior > 0
                        ? (decimal)invoice.SaldoAnterior
                        : null,

                    MontoAvancePago = tipoEcf == "45" && invoice.MontoAvancePago > 0
                        ? (decimal)invoice.MontoAvancePago
                        : null,

                    MontoPeriodo = tipoEcf == "45" && invoice.MontoPeriodo > 0
                        ? (decimal)invoice.MontoPeriodo
                        : null,

                    ValorPagar = tipoEcf == "45"
                        ? Round(
                            (decimal)(invoice.SaldoAnterior ?? 0) +
                            (decimal)(invoice.MontoAvancePago ?? 0) +
                            (montoGravadoI1 + montoGravadoI2 + montoGravadoI3 + montoExento + totalITBIS1 + totalITBIS2 + totalITBIS3)
                        )
                        : null

                };
            }

            return ecf;
        }

        private decimal? GetMontoIsrRetenido(IEnumerable<ItemTaxes> taxes, ItemType? itemType, decimal montoItem, string tipoEcf)
        {
            if (tipoEcf != "41") return null;
            if (itemType != ItemType.Service) return null;

            var isrTax = taxes.FirstOrDefault(t => t.Tax.TaxType == TaxType.ISR);
            if (isrTax != null)
                return Round(montoItem * ((decimal)isrTax.Tax.Amount / 100m));

            return Round(montoItem * (10m / 100m));
        }

        private (int indicador, decimal tasa) GetIndicadorYtasa(IEnumerable<ItemTaxes> taxes)
        {
            if (taxes == null || !taxes.Any())
                return (4, 0);

            var noFacturable = taxes.FirstOrDefault(t => t.Tax.TaxType == TaxType.NO_FACTURABLE);
            if (noFacturable != null)
                return (0, 0);

            var itbis = taxes.FirstOrDefault(t => t.Tax.TaxType == TaxType.ITBIS);
            if (itbis != null)
            {
                var rate = (decimal)itbis.Tax.Amount;
                if (rate == 18) return (1, 18);
                if (rate == 16) return (2, 16);
                if (rate == 0) return (3, 0);
            }

            if (taxes.Any(t => t.Tax.TaxType == TaxType.EXENTO))
                return (4, 0);

            return (4, 0);
        }

        private decimal Round(decimal value)
            => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}