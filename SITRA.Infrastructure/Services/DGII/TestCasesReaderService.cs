using System;
using System.IO;
using System.Collections.Generic;
using ClosedXML.Excel;
using SITRA.Core.Interfaces;
using SITRA.Core.DTOs.DGII;
using System.Linq;

namespace SITRA.Infrastructure.Services.DGII
{
    public class TestCasesReaderService : ITestCasesReader
    {
        public List<EcfDocument> ReadTestCases(Stream excelStream)
        {
            var documents = new List<EcfDocument>();
            var rfceEncfs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var rfceFiscalSupports = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var rfceCodigoSeguridades = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (var workbook = new XLWorkbook(excelStream))
            {
                // Check for RFCE sheet to flag documents that should be skipped
                if (workbook.TryGetWorksheet("RFCE", out var rfceSheet))
                {
                    var rfceRange = rfceSheet.RangeUsed();
                    var rfceHeaderRow = rfceRange.Row(1);
                    int encfColIndex = -1;
                    int fiscalSupportColIndex = -1;
                    int codigoSeguridadColIndex = -1;

                    for (int i = 1; i <= rfceRange.ColumnCount(); i++)
                    {
                        var header = rfceHeaderRow.Cell(i).GetString();
                        if (string.Equals(header, "ENCF", StringComparison.OrdinalIgnoreCase))
                        {
                            encfColIndex = i;
                        }
                        else if (string.Equals(header, "FiscalSupport", StringComparison.OrdinalIgnoreCase))
                        {
                            fiscalSupportColIndex = i;
                        }
                        else if (string.Equals(header, "CodigoSeguridad", StringComparison.OrdinalIgnoreCase))
                        {
                            codigoSeguridadColIndex = i;
                        }
                    }

                    if (encfColIndex != -1)
                    {
                        foreach (var row in rfceRange.RowsUsed().Skip(1))
                        {
                            string encf = row.Cell(encfColIndex).GetString()?.Trim();
                            if (!string.IsNullOrEmpty(encf))
                            {
                                rfceEncfs.Add(encf);

                                if (fiscalSupportColIndex != -1)
                                {
                                    string fs = row.Cell(fiscalSupportColIndex).GetString()?.Trim();
                                    if (!string.IsNullOrEmpty(fs))
                                    {
                                        rfceFiscalSupports[encf] = fs;
                                    }
                                }

                                if (codigoSeguridadColIndex != -1)
                                {
                                    string cs = row.Cell(codigoSeguridadColIndex).GetString()?.Trim();
                                    if (!string.IsNullOrEmpty(cs))
                                    {
                                        rfceCodigoSeguridades[encf] = cs;
                                    }
                                }
                            }
                        }
                    }
                }

                var worksheet = workbook.Worksheet(1);
                var range = worksheet.RangeUsed();
                var headerRow = range.Row(1);

                // Build a dynamic index to be resilient to column shifts
                var headerIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int i = 1; i <= range.ColumnCount(); i++)
                {
                    string header = headerRow.Cell(i).GetString();
                    if (!string.IsNullOrEmpty(header) && !headerIndex.ContainsKey(header))
                    {
                        headerIndex[header] = i;
                    }
                }

                var rows = range.RowsUsed().Skip(1);
                foreach (var row in rows)
                {
                    var doc = MapRowToDocument(row, headerIndex);
                    if (doc != null && !string.IsNullOrEmpty(doc.Encabezado.IdDoc.ENCF))
                    {
                        if (rfceEncfs.Contains(doc.Encabezado.IdDoc.ENCF))
                        {
                            doc.IsRfce = true;
                            if (rfceFiscalSupports.TryGetValue(doc.Encabezado.IdDoc.ENCF, out var fs))
                            {
                                doc.Encabezado.IdDoc.TipoIngresos = fs;
                            }
                            if (rfceCodigoSeguridades.TryGetValue(doc.Encabezado.IdDoc.ENCF, out var cs))
                            {
                                doc.CodigoSeguridad = cs;
                            }
                        }
                        documents.Add(doc);
                    }
                }
            }

            return documents;
        }

        private EcfDocument MapRowToDocument(IXLRangeRow row, Dictionary<string, int> headerIndex)
        {
            var doc = new EcfDocument();

            string Get(string header)
            {
                if (!headerIndex.TryGetValue(header, out int idx)) return null;
                var val = row.Cell(idx).GetString()?.Trim();
                return (string.IsNullOrEmpty(val) || val == "#e") ? null : val;
            }

            decimal? GetDec(string header) => decimal.TryParse(Get(header), out decimal d) ? d : (decimal?)null;
            int? GetInt(string header) => int.TryParse(Get(header), out int i) ? i : (int?)null;
            int GetIntOrZero(string header) => int.TryParse(Get(header), out int i) ? i : 0;

            doc.TestCaseName = Get("CasoPrueba");

            // IdDoc
            doc.Encabezado.Version = Get("Version") ?? "1.0";
            doc.Encabezado.IdDoc.TipoeCF = Get("TipoeCF") ?? "31";
            doc.Encabezado.IdDoc.ENCF = Get("ENCF");
            doc.Encabezado.IdDoc.FechaVencimientoSecuencia = Get("FechaVencimientoSecuencia");
            doc.Encabezado.IdDoc.IndicadorNotaCredito = GetInt("IndicadorNotaCredito");

            doc.Encabezado.IdDoc.IndicadorMontoGral = Get("IndicadorMontoGravado");

            string eCfType = doc.Encabezado.IdDoc.TipoeCF;
            
            // TipoIngresos default logic (01 for most types except 34, 41, 43, 47 and only if not provided)
            doc.Encabezado.IdDoc.TipoIngresos = Get("TipoIngresos") ?? (eCfType != "34" && eCfType != "41" && eCfType != "43" && eCfType != "47" ? "01" : null);

            // TipoPago default logic (1 for most types except 41, 43)
            doc.Encabezado.IdDoc.TipoPago = Get("TipoPago") ?? (eCfType != "41" && eCfType != "43" ? "1" : null);
            doc.Encabezado.IdDoc.FechaLimitePago = Get("FechaLimitePago");
            doc.Encabezado.IdDoc.TerminoPago = Get("TerminoPago");
            doc.Encabezado.IdDoc.TipoCuentaPago = Get("TipoCuentaPago");
            doc.Encabezado.IdDoc.NumeroCuentaPago = Get("NumeroCuentaPago");
            doc.Encabezado.IdDoc.BancoPago = Get("BancoPago");
            doc.Encabezado.IdDoc.TotalPaginas = GetInt("TotalPaginas");

            // Emisor
            doc.Encabezado.Emisor.RNCEmisor = Get("RNCEmisor");
            doc.Encabezado.Emisor.RazonSocial = Get("RazonSocialEmisor");
            doc.Encabezado.Emisor.NombreComercial = Get("NombreComercial");
            doc.Encabezado.Emisor.Sucursal = Get("Sucursal");
            doc.Encabezado.Emisor.DireccionEmisor = Get("DireccionEmisor");
            doc.Encabezado.Emisor.Municipio = Get("Municipio");
            doc.Encabezado.Emisor.Provincia = Get("Provincia");
            doc.Encabezado.Emisor.CorreoEmisor = Get("CorreoEmisor");
            doc.Encabezado.Emisor.WebSite = Get("WebSite");
            doc.Encabezado.Emisor.ActividadEconomica = Get("ActividadEconomica");
            doc.Encabezado.Emisor.CodigoVendedor = Get("CodigoVendedor");
            doc.Encabezado.Emisor.NumeroFacturaInterna = Get("NumeroFacturaInterna");
            doc.Encabezado.Emisor.NumeroPedidoInterno = Get("NumeroPedidoInterno");
            doc.Encabezado.Emisor.ZonaVenta = Get("ZonaVenta");
            doc.Encabezado.Emisor.RutaVenta = Get("RutaVenta");
            doc.Encabezado.Emisor.InformacionAdicionalEmisor = Get("InformacionAdicionalEmisor");
            doc.Encabezado.Emisor.FechaEmision = Get("FechaEmision");

            for (int i = 1; i <= 3; i++)
            {
                string? phone = Get($"TelefonoEmisor[{i}]") ?? (i == 1 ? Get("TelefonoEmisor") : null);
                if (!string.IsNullOrEmpty(phone))
                {
                    if (doc.Encabezado.Emisor.TablaTelefonoEmisor == null) doc.Encabezado.Emisor.TablaTelefonoEmisor = new List<string>();
                    doc.Encabezado.Emisor.TablaTelefonoEmisor.Add(phone);
                }
            }

            // Comprador
            doc.Encabezado.Comprador.RNCComprador = Get("RNCComprador");
            doc.Encabezado.Comprador.IdentificadorExtranjero = Get("IdentificadorExtranjero");
            doc.Encabezado.Comprador.RazonSocialComprador = Get("RazonSocialComprador");
            doc.Encabezado.Comprador.ContactoComprador = Get("ContactoComprador");
            doc.Encabezado.Comprador.CorreoComprador = Get("CorreoComprador");
            doc.Encabezado.Comprador.DireccionComprador = Get("DireccionComprador");
            doc.Encabezado.Comprador.MunicipioComprador = Get("MunicipioComprador");
            doc.Encabezado.Comprador.ProvinciaComprador = Get("ProvinciaComprador");
            doc.Encabezado.Comprador.PaisComprador = Get("PaisComprador");
            doc.Encabezado.Comprador.FechaEntrega = Get("FechaEntrega");
            doc.Encabezado.Comprador.ContactoEntrega = Get("ContactoEntrega");
            doc.Encabezado.Comprador.DireccionEntrega = Get("DireccionEntrega");
            doc.Encabezado.Comprador.TelefonoAdicional = Get("TelefonoAdicional");
            doc.Encabezado.Comprador.FechaOrdenCompra = Get("FechaOrdenCompra");
            doc.Encabezado.Comprador.NumeroOrdenCompra = Get("NumeroOrdenCompra");
            doc.Encabezado.Comprador.CodigoInternoComprador = Get("CodigoInternoComprador");
            doc.Encabezado.Comprador.ResponsablePago = Get("ResponsablePago");
            doc.Encabezado.Comprador.InformacionAdicionalComprador = Get("InformacionAdicionalComprador");

            // Totales
            doc.Encabezado.Totales.MontoGravadoTotal = GetDec("MontoGravadoTotal");
            doc.Encabezado.Totales.MontoGravadoI1 = GetDec("MontoGravadoI1");
            doc.Encabezado.Totales.MontoGravadoI2 = GetDec("MontoGravadoI2");
            doc.Encabezado.Totales.MontoGravadoI3 = GetDec("MontoGravadoI3");
            doc.Encabezado.Totales.MontoExento = GetDec("MontoExento");
            doc.Encabezado.Totales.ITBIS1 = GetInt("ITBIS1");
            doc.Encabezado.Totales.ITBIS2 = GetInt("ITBIS2");
            doc.Encabezado.Totales.ITBIS3 = GetInt("ITBIS3");
            doc.Encabezado.Totales.TotalITBIS = GetDec("TotalITBIS");
            doc.Encabezado.Totales.TotalITBIS1 = GetDec("TotalITBIS1");
            doc.Encabezado.Totales.TotalITBIS2 = GetDec("TotalITBIS2");
            doc.Encabezado.Totales.TotalITBIS3 = GetDec("TotalITBIS3");
            doc.Encabezado.Totales.MontoTotal = GetDec("MontoTotal");
            doc.Encabezado.Totales.MontoNoFacturable = GetDec("MontoNoFacturable");
            doc.Encabezado.Totales.MontoPeriodo = GetDec("MontoPeriodo");
            doc.Encabezado.Totales.SaldoAnterior = GetDec("SaldoAnterior");
            doc.Encabezado.Totales.MontoAvancePago = GetDec("MontoAvancePago");
            doc.Encabezado.Totales.ValorPagar = GetDec("ValorPagar");
            doc.Encabezado.Totales.TotalITBISRetenido = GetDec("TotalITBISRetenido");
            doc.Encabezado.Totales.TotalISRRetencion = GetDec("TotalISRRetencion");
            doc.Encabezado.Totales.TotalITBISPercepcion = GetDec("TotalITBISPercepcion");
            doc.Encabezado.Totales.TotalISRPercepcion = GetDec("TotalISRPercepcion");

            // Items - iterate while we find line numbers
            for (int i = 1; i <= 200; i++)
            {
                string lineKey = $"NumeroLinea[{i}]";
                if (!headerIndex.ContainsKey(lineKey)) break;
                
                string lineVal = Get(lineKey);
                if (string.IsNullOrEmpty(lineVal) || !int.TryParse(lineVal, out int numeroLinea)) break;

                var item = new Item
                {
                    NumeroLinea = numeroLinea,
                    IndicadorFacturacion = GetIntOrZero($"IndicadorFacturacion[{i}]"),
                    NombreItem = Get($"NombreItem[{i}]"),
                    IndicadorBienoServicio = GetIntOrZero($"IndicadorBienoServicio[{i}]"),
                    Cantidad = GetDec($"CantidadItem[{i}]") ?? 0,
                    UnidadMedida = Get($"UnidadMedida[{i}]"),
                    FechaElaboracion = Get($"FechaElaboracion[{i}]"),
                    FechaVencimientoItem = Get($"FechaVencimientoItem[{i}]"),
                    PrecioUnitario = GetDec($"PrecioUnitarioItem[{i}]") ?? 0,
                    DescuentoMonto = GetDec($"DescuentoMonto[{i}]"),
                    RecargoMonto = GetDec($"RecargoMonto[{i}]"),
                    MontoItem = GetDec($"MontoItem[{i}]") ?? 0
                };

                // Item Codes
                for (int j = 1; j <= 5; j++)
                {
                    string tipo = Get($"TipoCodigo[{i}][{j}]");
                    if (string.IsNullOrEmpty(tipo)) break;
                    if (item.TablaCodigosItem == null) item.TablaCodigosItem = new List<CodigoItem>();
                    item.TablaCodigosItem.Add(new CodigoItem { TipoCodigo = tipo, Codigo = Get($"CodigoItem[{i}][{j}]") ?? "" });
                }

                // Sub Descuentos
                for (int j = 1; j <= 5; j++)
                {
                    string tipo = Get($"TipoSubDescuento[{i}][{j}]");
                    if (string.IsNullOrEmpty(tipo)) break;
                    if (item.TablaSubDescuento == null) item.TablaSubDescuento = new List<SubDescuento>();
                    item.TablaSubDescuento.Add(new SubDescuento
                    {
                        TipoSubDescuento = tipo,
                        SubDescuentoPorcentaje = GetDec($"SubDescuentoPorcentaje[{i}][{j}]"),
                        MontoSubDescuento = GetDec($"MontoSubDescuento[{i}][{j}]")
                    });
                }

                // Sub Recargos
                for (int j = 1; j <= 5; j++)
                {
                    string tipo = Get($"TipoSubRecargo[{i}][{j}]");
                    if (string.IsNullOrEmpty(tipo)) break;
                    if (item.TablaSubRecargo == null) item.TablaSubRecargo = new List<SubRecargo>();
                    item.TablaSubRecargo.Add(new SubRecargo
                    {
                        TipoSubRecargo = tipo,
                        SubRecargoPorcentaje = GetDec($"SubRecargoPorcentaje[{i}][{j}]"),
                        MontoSubRecargo = GetDec($"MontoSubRecargo[{i}][{j}]")
                    });
                }

                item.DescripcionItem = Get($"DescripcionItem[{i}]");

                // Item level retention
                int? iarp = GetInt($"IndicadorAgenteRetencionoPercepcion[{i}]");
                if (iarp.HasValue)
                {
                    item.Retencion = new Retencion
                    {
                        IndicadorAgenteRetencionoPercepcion = iarp.Value,
                        MontoITBISRetenido = GetDec($"MontoITBISRetenido[{i}]"),
                        MontoISRRetenido = GetDec($"MontoISRRetenido[{i}]")
                    };
                }
                else if (doc.Encabezado.IdDoc.TipoeCF == "41")
                {
                    // For E41, Retencion is mandatory. If not present in excel, we must at least provide an empty one with a default indicator
                    // But usually the excel SHOULD have it if it's a valid E41 test case.
                    // The error says: "invalid child element 'NombreItem'. List of possible elements expected: 'Retencion'"
                    // So we MUST set it for 41.
                    item.Retencion = new Retencion { IndicadorAgenteRetencionoPercepcion = 0 }; 
                }

                doc.Detalles.Items.Add(item);
            }

            // Global Adjustments
            for (int i = 1; i <= 20; i++)
            {
                string lineKey = $"TipoAjuste[{i}]";
                if (!headerIndex.ContainsKey(lineKey)) break;

                string typeVal = Get(lineKey);
                if (string.IsNullOrEmpty(typeVal)) break;

                if (doc.DescuentosORecargos == null) doc.DescuentosORecargos = new DescuentosORecargos();
                doc.DescuentosORecargos.Items.Add(new DescuentoORecargo
                {
                    NumeroLinea = i,
                    TipoAjuste = typeVal,
                    DescripcionDescuentooRecargo = Get($"DescripcionDescuentooRecargo[{i}]"),
                    TipoValor = Get($"TipoValor[{i}]"),
                    ValorDescuentooRecargo = GetDec($"ValorDescuentooRecargo[{i}]"),
                    MontoDescuentooRecargo = GetDec($"MontoDescuentooRecargo[{i}]") ?? 0,
                    MontoDescuentooRecargoOtraMoneda = GetDec($"MontoDescuentooRecargoOtraMoneda[{i}]"),
                    IndicadorFacturacion = GetInt($"IndicadorFacturacionDescuentooRecargo[{i}]")
                });
            }

            // Subtotals
            for (int i = 1; i <= 20; i++)
            {
                string lineKey = $"NumeroSubTotal[{i}]";
                if (!headerIndex.ContainsKey(lineKey)) break;

                string val = Get(lineKey);
                if (string.IsNullOrEmpty(val) || !int.TryParse(val, out int nst)) break;

                if (doc.Subtotales == null) doc.Subtotales = new Subtotales();
                doc.Subtotales.Items.Add(new Subtotal
                {
                    NumeroSubTotal = nst,
                    DescripcionSubtotal = Get($"DescripcionSubtotal[{i}]"),
                    Orden = GetIntOrZero($"Orden[{i}]"),
                    MontoSubTotal = GetDec($"MontoSubTotal[{i}]") ?? 0,
                    SubTotalMontoGravadoTotal = GetDec($"SubTotalMontoGravadoTotal[{i}]"),
                    SubTotalMontoGravadoI1 = GetDec($"SubTotalMontoGravadoI1[{i}]"),
                    SubTotalMontoGravadoI2 = GetDec($"SubTotalMontoGravadoI2[{i}]"),
                    SubTotalMontoGravadoI3 = GetDec($"SubTotalMontoGravadoI3[{i}]"),
                    SubTotaITBIS = GetDec($"SubTotaITBIS[{i}]"),
                    SubTotaITBIS1 = GetDec($"SubTotaITBIS1[{i}]"),
                    SubTotaITBIS2 = GetDec($"SubTotaITBIS2[{i}]"),
                    SubTotaITBIS3 = GetDec($"SubTotaITBIS3[{i}]"),
                    SubTotalImpuestoAdicional = GetDec($"SubTotalImpuestoAdicional[{i}]"),
                    SubTotalExento = GetDec($"SubTotalExento[{i}]"),
                    Lineas = GetInt($"Lineas[{i}]")
                });
            }

            // Information Referencia
            string? ncfModificado = Get("NCFModificado");
            if (!string.IsNullOrEmpty(ncfModificado))
            {
                doc.InformacionReferencia = new InformacionReferencia
                {
                    NCFModificado = ncfModificado,
                    RNCOtroContribuyente = Get("RNCOtroContribuyente"),
                    FechaNCFModificado = Get("FechaNCFModificado"),
                    CodigoModificacion = GetInt("CodigoModificacion"),
                    RazonModificacion = Get("RazonModificacion")
                };
            }

            // Forms of Payment
            for (int i = 1; i <= 10; i++)
            {
                string? fpago = Get($"FormaPago[{i}]");
                if (string.IsNullOrEmpty(fpago)) break;

                if (doc.Encabezado.IdDoc.TablaFormasPago == null) doc.Encabezado.IdDoc.TablaFormasPago = new List<FormaDePago>();
                doc.Encabezado.IdDoc.TablaFormasPago.Add(new FormaDePago
                {
                    FormaPago = fpago,
                    MontoPago = GetDec($"MontoPago[{i}]") ?? 0
                });
            }

            return doc;
        }
    }
}
