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

            using (var workbook = new XLWorkbook(excelStream))
            {
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

            doc.Encabezado.IdDoc.IndicadorMontoGral = Get("IndicadorMontoGravado");

            doc.Encabezado.IdDoc.TipoIngresos = Get("TipoIngresos") ?? "01";
            doc.Encabezado.IdDoc.TipoPago = Get("TipoPago") ?? "1";

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

            // Comprador
            doc.Encabezado.Comprador.RNCComprador = Get("RNCComprador");
            doc.Encabezado.Comprador.RazonSocialComprador = Get("RazonSocialComprador");
            doc.Encabezado.Comprador.ContactoComprador = Get("ContactoComprador");
            doc.Encabezado.Comprador.CorreoComprador = Get("CorreoComprador");
            doc.Encabezado.Comprador.DireccionComprador = Get("DireccionComprador");
            doc.Encabezado.Comprador.MunicipioComprador = Get("MunicipioComprador");
            doc.Encabezado.Comprador.ProvinciaComprador = Get("ProvinciaComprador");
            doc.Encabezado.Comprador.FechaEntrega = Get("FechaEntrega");
            doc.Encabezado.Comprador.FechaOrdenCompra = Get("FechaOrdenCompra");
            doc.Encabezado.Comprador.NumeroOrdenCompra = Get("NumeroOrdenCompra");
            doc.Encabezado.Comprador.CodigoInternoComprador = Get("CodigoInternoComprador");

            // Totales
            doc.Encabezado.Totales.MontoGravadoTotal = GetDec("MontoGravadoTotal");
            doc.Encabezado.Totales.MontoGravadoI1 = GetDec("MontoGravadoI1");
            doc.Encabezado.Totales.MontoGravadoI2 = GetDec("MontoGravadoI2");
            doc.Encabezado.Totales.MontoExento = GetDec("MontoExento");
            doc.Encabezado.Totales.ITBIS1 = GetInt("ITBIS1");
            doc.Encabezado.Totales.ITBIS2 = GetInt("ITBIS2");
            doc.Encabezado.Totales.TotalITBIS = GetDec("TotalITBIS");
            doc.Encabezado.Totales.TotalITBIS1 = GetDec("TotalITBIS1");
            doc.Encabezado.Totales.TotalITBIS2 = GetDec("TotalITBIS2");
            doc.Encabezado.Totales.MontoTotal = GetDec("MontoTotal");
            doc.Encabezado.Totales.MontoNoFacturable = GetDec("MontoNoFacturable");
            doc.Encabezado.Totales.MontoPeriodo = GetDec("MontoPeriodo");
            doc.Encabezado.Totales.SaldoAnterior = GetDec("SaldoAnterior");
            doc.Encabezado.Totales.MontoAvancePago = GetDec("MontoAvancePago");
            doc.Encabezado.Totales.ValorPagar = GetDec("ValorPagar");

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
                    MontoSubTotal = GetDec($"MontoSubTotal[{i}]") ?? 0
                });
            }

            return doc;
        }
    }
}
