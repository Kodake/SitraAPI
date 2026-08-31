using Microsoft.Extensions.Options;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Interfaces;
using SITRA.Core.DTOs.DGII;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Models.Dgii;

namespace SITRA.Infrastructure.ExternalServices.DGII
{
    public class InvoiceToEcfMapper : IInvoiceToEcfMapper
    {
        private readonly DgiiSettings _settings;

        public InvoiceToEcfMapper(IOptions<DgiiSettings> options)
        {
            _settings = options.Value;
        }

        public EcfDocument MapToDgiiDocument(Invoice invoice)
        {
            if (string.IsNullOrEmpty(invoice.FiscalSupportNumber) || invoice.FiscalSupportNumber.Length < 3)
                throw new Exception("FiscalSupportNumber is invalid or missing.");

            string tipoEcf = invoice.FiscalSupportNumber.Substring(1, 2);

            return BuildEcfDocument(invoice, tipoEcf);
        }

        private EcfDocument BuildEcfDocument(Invoice invoice, string tipoEcf)
        {
            var biller = invoice.Biller;
            var buyer = invoice.Buyer;

            var ecf = new EcfDocument
            {
                Encabezado = new Encabezado
                {
                    IdDoc = new IdDoc
                    {
                        TipoeCF = tipoEcf,
                        ENCF = invoice.FiscalSupportNumber!,
                        FechaVencimientoSecuencia = _settings.DefaultFechaVencimientoSecuencia,
                        IndicadorMontoGral = (tipoEcf is "43" or "44" or "46" or "47") ? null : invoice.IndicadorMontoGral,
                        IndicadorEnvioDiferido = (invoice.IndicadorEnvioDiferido == 1) ? 1 : (int?)null,
                        TipoIngresos = ((tipoEcf == "33" || tipoEcf == "34") && !string.IsNullOrEmpty(invoice.ModifiedNcfNumber) &&
                                        (invoice.ModifiedNcfNumber.StartsWith("E41", StringComparison.OrdinalIgnoreCase) || 
                                         invoice.ModifiedNcfNumber.StartsWith("E43", StringComparison.OrdinalIgnoreCase) || 
                                         invoice.ModifiedNcfNumber.StartsWith("E47", StringComparison.OrdinalIgnoreCase)))
                                        ? null
                                        : (invoice.TipoIngresos ?? "01"),
                        TipoPago = invoice.ConditionType == InvoiceCondition.Spot ? "1" : "2"
                    },
                    Emisor = new Emisor
                    {
                        RNCEmisor = biller?.Identification ?? _settings.DefaultRNC,
                        RazonSocial = biller?.Name ?? _settings.DefaultRazonSocial,
                        DireccionEmisor = biller?.Address ?? _settings.DefaultDireccion,
                        Municipio = biller?.Municipality,
                        Provincia = biller?.Province,
                        CorreoEmisor = biller?.Email,
                        FechaEmision = invoice.Date.ToString("dd-MM-yyyy")
                    },
                    Comprador = new Comprador
                    {
                        RNCComprador = (tipoEcf == "46" || tipoEcf == "47") ? null : buyer?.Identification,
                        IdentificadorExtranjero = (tipoEcf == "46" || tipoEcf == "47") ? buyer?.Identification : null,
                        RazonSocialComprador = buyer?.Name ?? (tipoEcf == "45" ? "ENTIDAD GUBERNAMENTAL" : "CONSUMIDOR FINAL"),
                        DireccionComprador = tipoEcf == "47" ? null : (buyer?.Address ?? "N/A"),
                        MunicipioComprador = (tipoEcf == "46" || tipoEcf == "47") ? null : buyer?.Municipality,
                        ProvinciaComprador = (tipoEcf == "46" || tipoEcf == "47") ? null : buyer?.Province,
                        CorreoComprador = (tipoEcf == "46" || tipoEcf == "47") ? null : buyer?.Email
                    }
                }
            };

            if ((tipoEcf == "31" || tipoEcf == "41" || tipoEcf == "44" || tipoEcf == "45") && string.IsNullOrEmpty(buyer?.Identification))
                throw new Exception($"RNC Comprador is mandatory for e-CF type {tipoEcf}.");

            if ((tipoEcf == "46" || tipoEcf == "47") && string.IsNullOrEmpty(buyer?.Identification))
                throw new Exception($"Identificador Extranjero is mandatory for e-CF type {tipoEcf}.");

            if ((tipoEcf == "46" || tipoEcf == "47") && string.IsNullOrEmpty(buyer?.Name))
                throw new Exception($"Razon Social Comprador is mandatory for e-CF type {tipoEcf}.");

            // =========================
            // ACUMULADORES
            // =========================
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

                if (tipoEcf == "43" || tipoEcf == "44" || tipoEcf == "47") { indicador = 4; tasa = 0; }
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

                // Retención ITBIS: solo indicadores 1 y 2
                var montoItbisRetenido = (tipoEcf == "41" || tipoEcf == "47")
                    ? indicador switch
                    {
                        1 => Round(montoItem * (18m / 100m)),
                        2 => Round(montoItem * (16m / 100m)),
                        3 => 0m,
                        _ => (decimal?)null
                    }
                    : (decimal?)null;

                // Retención ISR: servicios en e41 (10%) y e47 (27%)
                var montoIsrRetenido = GetMontoIsrRetenido(taxes, item.Item?.ItemType, montoItem, tipoEcf);

                // Acumular retenciones
                if (tipoEcf == "41" && montoItbisRetenido.HasValue)
                    totalITBISRetenido += montoItbisRetenido.Value;

                if ((tipoEcf == "41" || tipoEcf == "47") && montoIsrRetenido.HasValue)
                    totalISRRetenido += montoIsrRetenido.Value;

                bool tieneAlgoQueRetener = montoItbisRetenido.HasValue || montoIsrRetenido.HasValue;

                ecf.Detalles.Items.Add(new Core.DTOs.DGII.Item
                {
                    NumeroLinea = lineNum++,
                    IndicadorFacturacion = indicador,

                    // e41: Retencion obligatoria en TODAS las líneas aunque esté vacía
                    // e47: solo cuando hay algo que retener, e ISR sí se reporta por línea
                    Retencion = tipoEcf == "41"
                        ? new Retencion
                        {
                            IndicadorAgenteRetencionoPercepcion = 1,
                            MontoITBISRetenido = montoItbisRetenido,
                            MontoISRRetenido = montoIsrRetenido
                        }
                        : (tipoEcf == "47" && tieneAlgoQueRetener
                            ? new Retencion
                            {
                                IndicadorAgenteRetencionoPercepcion = 1,
                                MontoITBISRetenido = montoItbisRetenido,
                                MontoISRRetenido = montoIsrRetenido
                            }
                            : null),

                    NombreItem = item.Item?.Name ?? "Item Generico",
                    IndicadorBienoServicio = (int?)item.Item?.ItemType ?? 1,
                    Cantidad = qty,
                    PrecioUnitario = price,
                    MontoItem = montoItem
                });
            }

            // =========================
            // TOTALES
            // =========================
            var simpleTipos = new HashSet<string> { "43", "44", "47" };

            if (simpleTipos.Contains(tipoEcf))
            {
                ecf.Encabezado.Totales = new Totales
                {
                    CurrentTipoeCF = tipoEcf,
                    MontoExento = montoExento > 0 ? Round(montoExento) : null,
                    MontoTotal = Round(montoExento + montoNoFacturableTotal),

                    // e47 reporta retenciones en totales solo si hay algo retenido
                    TotalITBISRetenido = tipoEcf == "47" && totalITBISRetenido > 0
                        ? Round(totalITBISRetenido)
                        : null,
                    TotalISRRetencion = tipoEcf == "47" && totalISRRetenido > 0
                        ? Round(totalISRRetenido)
                        : null
                };
            }
            else
            {
                bool tieneMontoGravado = usaI1 || usaI2 || usaI3;

                if (string.IsNullOrEmpty(ecf.Encabezado.IdDoc.IndicadorMontoGral) && (tipoEcf is not "43" and not "44" and not "46" and not "47"))
                {
                    // Our line items serialize their BasePrice (tax-excluded), which formally requires "0" per DGII schema definitions.
                    ecf.Encabezado.IdDoc.IndicadorMontoGral = tieneMontoGravado
                        ? "0"
                        : null; 
                }

                ecf.Encabezado.Totales = new Totales
                {
                    CurrentTipoeCF = tipoEcf,

                    ITBIS1 = (tipoEcf != "46" && usaI1) ? 18 : null,
                    ITBIS2 = (tipoEcf != "46" && usaI2) ? 16 : null,
                    ITBIS3 = usaI3 ? 0 : null,

                    MontoGravadoTotal = tieneMontoGravado ? Round(montoGravadoI1 + montoGravadoI2 + montoGravadoI3) : null,

                    MontoGravadoI1 = (tipoEcf != "46" && usaI1) ? Round(montoGravadoI1) : null,
                    MontoGravadoI2 = (tipoEcf != "46" && usaI2) ? Round(montoGravadoI2) : null,
                    MontoGravadoI3 = usaI3 ? Round(montoGravadoI3) : null,

                    MontoExento = (tipoEcf == "46") ? null : (montoExento > 0 ? Round(montoExento) : null),

                    TotalITBIS = tieneMontoGravado ? Round(totalITBIS1 + totalITBIS2 + totalITBIS3) : null,
                    TotalITBIS1 = (tipoEcf != "46" && usaI1) ? Round(totalITBIS1) : null,
                    TotalITBIS2 = (tipoEcf != "46" && usaI2) ? Round(totalITBIS2) : null,
                    TotalITBIS3 = usaI3 ? Round(totalITBIS3) : null,

                    MontoTotal = Round(
                        montoGravadoI1 + montoGravadoI2 + montoGravadoI3 +
                        montoExento + totalITBIS1 + totalITBIS2 + totalITBIS3
                    ),

                    // e41: TotalITBISRetenido siempre presente aunque sea 0
                    TotalITBISRetenido = tipoEcf == "41"
                        ? Round(totalITBISRetenido)
                        : null,

                    // e41: TotalISRRetencion solo si hay retención
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
                            (montoGravadoI1 + montoGravadoI2 + montoGravadoI3 +
                             montoExento + totalITBIS1 + totalITBIS2 + totalITBIS3)
                        )
                        : null
                };
            }

            return ecf;
        }

        // =========================
        // HELPERS
        // =========================

        private decimal? GetMontoIsrRetenido(IEnumerable<ItemTaxes> taxes, ItemType? itemType, decimal montoItem, string tipoEcf)
        {
            if (tipoEcf != "41" && tipoEcf != "47") return null;

            // e47: ISR 27% sobre servicios exentos para no-residentes
            if (tipoEcf == "47")
            {
                if (itemType != ItemType.Service) return null;
                var isrTax47 = taxes.FirstOrDefault(t => t.Tax.TaxType == TaxType.ISR);
                return isrTax47 != null
                    ? Round(montoItem * ((decimal)isrTax47.Tax.Amount / 100m))
                    : Round(montoItem * (27m / 100m));
            }

            // e41: ISR 10% solo sobre servicios
            if (itemType != ItemType.Service) return null;
            var isrTax = taxes.FirstOrDefault(t => t.Tax.TaxType == TaxType.ISR);
            return isrTax != null
                ? Round(montoItem * ((decimal)isrTax.Tax.Amount / 100m))
                : Round(montoItem * (10m / 100m));
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
                if (rate == 0) return (4, 0);
            }

            if (taxes.Any(t => t.Tax.TaxType == TaxType.EXENTO))
                return (4, 0);

            return (4, 0);
        }

        private decimal Round(decimal value)
            => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}