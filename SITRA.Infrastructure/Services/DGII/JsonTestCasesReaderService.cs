using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using SITRA.Core.Interfaces;
using SITRA.Core.DTOs.DGII;

namespace SITRA.Infrastructure.Services.DGII
{
    public class JsonTestCasesReaderService : ITestCasesReader
    {
        public List<EcfDocument> ReadTestCases(Stream jsonStream)
        {
            return ReadTestCasesFromJson(jsonStream);
        }

        public List<EcfDocument> ReadTestCasesFromJson(Stream jsonStream)
        {
            var documents = new List<EcfDocument>();

            using (var reader = new StreamReader(jsonStream))
            {
                string json = reader.ReadToEnd();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var rawList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json, options);

                if (rawList == null) return documents;

                foreach (var rawDoc in rawList)
                {
                    var ecfDocument = new EcfDocument();
                    
                    // Basic Mapping
                    ecfDocument.TestCaseName = GetString(rawDoc, "CasoPrueba");
                    ecfDocument.Encabezado.Version = GetString(rawDoc, "Version") ?? "1.0";
                    ecfDocument.Encabezado.IdDoc.TipoeCF = GetString(rawDoc, "TipoeCF");
                    ecfDocument.Encabezado.IdDoc.ENCF = GetString(rawDoc, "ENCF");
                    ecfDocument.Encabezado.IdDoc.FechaVencimientoSecuencia = GetString(rawDoc, "FechaVencimientoSecuencia");
                    if (int.TryParse(GetString(rawDoc, "IndicadorNotaCredito"), out int inc))
                        ecfDocument.Encabezado.IdDoc.IndicadorNotaCredito = inc;
                    ecfDocument.Encabezado.IdDoc.IndicadorMontoGral = GetString(rawDoc, "IndicadorMontoGravado");
                    ecfDocument.Encabezado.IdDoc.TipoIngresos = GetString(rawDoc, "TipoIngresos");
                    string ecType = ecfDocument.Encabezado.IdDoc.TipoeCF;
                    
                    // Unified default logic (apply to all except 31, 33, 34, 41, 43, and 47)
                    if (string.IsNullOrEmpty(ecfDocument.Encabezado.IdDoc.TipoIngresos) && ecType != "34" && ecType != "41" && ecType != "43" && ecType != "47")
                        ecfDocument.Encabezado.IdDoc.TipoIngresos = "01";

                    ecfDocument.Encabezado.IdDoc.TipoPago = GetString(rawDoc, "TipoPago") ?? (ecType != "41" && ecType != "43" ? "1" : null);
                    if (int.TryParse(GetString(rawDoc, "TotalPaginas"), out int tp))
                        ecfDocument.Encabezado.IdDoc.TotalPaginas = tp;
                    
                    // Emisor Mapping
                    ecfDocument.Encabezado.Emisor.RNCEmisor = GetString(rawDoc, "RNCEmisor");
                    ecfDocument.Encabezado.Emisor.RazonSocial = GetString(rawDoc, "RazonSocialEmisor");
                    ecfDocument.Encabezado.Emisor.NombreComercial = GetString(rawDoc, "NombreComercial");
                    ecfDocument.Encabezado.Emisor.Sucursal = GetString(rawDoc, "Sucursal");
                    ecfDocument.Encabezado.Emisor.DireccionEmisor = GetString(rawDoc, "DireccionEmisor");
                    ecfDocument.Encabezado.Emisor.Municipio = GetString(rawDoc, "Municipio");
                    ecfDocument.Encabezado.Emisor.Provincia = GetString(rawDoc, "Provincia");
                    ecfDocument.Encabezado.Emisor.CorreoEmisor = GetString(rawDoc, "CorreoEmisor");
                    ecfDocument.Encabezado.Emisor.WebSite = GetString(rawDoc, "WebSite");
                    ecfDocument.Encabezado.Emisor.ActividadEconomica = GetString(rawDoc, "ActividadEconomica");
                    ecfDocument.Encabezado.Emisor.CodigoVendedor = GetString(rawDoc, "CodigoVendedor");
                    ecfDocument.Encabezado.Emisor.NumeroFacturaInterna = GetString(rawDoc, "NumeroFacturaInterna");
                    ecfDocument.Encabezado.Emisor.NumeroPedidoInterno = GetString(rawDoc, "NumeroPedidoInterno");
                    ecfDocument.Encabezado.Emisor.ZonaVenta = GetString(rawDoc, "ZonaVenta");
                    ecfDocument.Encabezado.Emisor.RutaVenta = GetString(rawDoc, "RutaVenta");
                    ecfDocument.Encabezado.Emisor.InformacionAdicionalEmisor = GetString(rawDoc, "InformacionAdicionalEmisor");
                    ecfDocument.Encabezado.Emisor.FechaEmision = GetString(rawDoc, "FechaEmision");
                    
                    // Comprador Mapping
                    ecfDocument.Encabezado.Comprador.RNCComprador = GetString(rawDoc, "RNCComprador");
                    ecfDocument.Encabezado.Comprador.IdentificadorExtranjero = GetString(rawDoc, "IdentificadorExtranjero");
                    ecfDocument.Encabezado.Comprador.RazonSocialComprador = GetString(rawDoc, "RazonSocialComprador");
                    ecfDocument.Encabezado.Comprador.ContactoComprador = GetString(rawDoc, "ContactoComprador");
                    ecfDocument.Encabezado.Comprador.CorreoComprador = GetString(rawDoc, "CorreoComprador") ?? GetString(rawDoc, "Correo");
                    ecfDocument.Encabezado.Comprador.DireccionComprador = GetString(rawDoc, "DireccionComprador");
                    ecfDocument.Encabezado.Comprador.MunicipioComprador = GetString(rawDoc, "MunicipioComprador");
                    ecfDocument.Encabezado.Comprador.ProvinciaComprador = GetString(rawDoc, "ProvinciaComprador");
                    ecfDocument.Encabezado.Comprador.PaisComprador = GetString(rawDoc, "PaisComprador");
                    ecfDocument.Encabezado.Comprador.FechaEntrega = GetString(rawDoc, "FechaEntrega");
                    ecfDocument.Encabezado.Comprador.ContactoEntrega = GetString(rawDoc, "ContactoEntrega");
                    ecfDocument.Encabezado.Comprador.DireccionEntrega = GetString(rawDoc, "DireccionEntrega");
                    ecfDocument.Encabezado.Comprador.TelefonoAdicional = GetString(rawDoc, "TelefonoAdicional");
                    ecfDocument.Encabezado.Comprador.FechaOrdenCompra = GetString(rawDoc, "FechaOrdenCompra");
                    ecfDocument.Encabezado.Comprador.NumeroOrdenCompra = GetString(rawDoc, "NumeroOrdenCompra");
                    ecfDocument.Encabezado.Comprador.CodigoInternoComprador = GetString(rawDoc, "CodigoInternoComprador");
                    ecfDocument.Encabezado.Comprador.ResponsablePago = GetString(rawDoc, "ResponsablePago");
                    ecfDocument.Encabezado.Comprador.InformacionAdicionalComprador = GetString(rawDoc, "InformacionAdicionalComprador");
                    
                    if (decimal.TryParse(GetString(rawDoc, "MontoTotal"), out decimal total))
                        ecfDocument.Encabezado.Totales.MontoTotal = total;

                    if (decimal.TryParse(GetString(rawDoc, "MontoNoFacturable"), out decimal mnf))
                        ecfDocument.Encabezado.Totales.MontoNoFacturable = mnf;

                    if (decimal.TryParse(GetString(rawDoc, "MontoPeriodo"), out decimal mp))
                        ecfDocument.Encabezado.Totales.MontoPeriodo = mp;

                    if (decimal.TryParse(GetString(rawDoc, "SaldoAnterior"), out decimal sa))
                        ecfDocument.Encabezado.Totales.SaldoAnterior = sa;

                    if (decimal.TryParse(GetString(rawDoc, "MontoAvancePago"), out decimal map))
                        ecfDocument.Encabezado.Totales.MontoAvancePago = map;

                    if (decimal.TryParse(GetString(rawDoc, "ValorPagar"), out decimal vp))
                        ecfDocument.Encabezado.Totales.ValorPagar = vp;
                    
                    if (decimal.TryParse(GetString(rawDoc, "TotalITBIS"), out decimal itbis))
                        ecfDocument.Encabezado.Totales.TotalITBIS = itbis;

                    if (decimal.TryParse(GetString(rawDoc, "MontoGravadoTotal"), out decimal mgt))
                        ecfDocument.Encabezado.Totales.MontoGravadoTotal = mgt;

                    if (decimal.TryParse(GetString(rawDoc, "MontoGravadoI1"), out decimal mg1))
                        ecfDocument.Encabezado.Totales.MontoGravadoI1 = mg1;

                    if (decimal.TryParse(GetString(rawDoc, "MontoGravadoI2"), out decimal mg2))
                        ecfDocument.Encabezado.Totales.MontoGravadoI2 = mg2;

                    if (decimal.TryParse(GetString(rawDoc, "MontoGravadoI3"), out decimal mg3))
                        ecfDocument.Encabezado.Totales.MontoGravadoI3 = mg3;

                    if (decimal.TryParse(GetString(rawDoc, "MontoExento"), out decimal mex))
                        ecfDocument.Encabezado.Totales.MontoExento = mex;

                    if (int.TryParse(GetString(rawDoc, "ITBIS1"), out int i1))
                        ecfDocument.Encabezado.Totales.ITBIS1 = i1;

                    if (int.TryParse(GetString(rawDoc, "ITBIS2"), out int i2))
                        ecfDocument.Encabezado.Totales.ITBIS2 = i2;

                    if (int.TryParse(GetString(rawDoc, "ITBIS3"), out int i3))
                        ecfDocument.Encabezado.Totales.ITBIS3 = i3;

                    if (decimal.TryParse(GetString(rawDoc, "TotalITBIS1"), out decimal ti1))
                        ecfDocument.Encabezado.Totales.TotalITBIS1 = ti1;

                    if (decimal.TryParse(GetString(rawDoc, "TotalITBIS2"), out decimal ti2))
                        ecfDocument.Encabezado.Totales.TotalITBIS2 = ti2;

                    if (decimal.TryParse(GetString(rawDoc, "TotalITBIS3"), out decimal ti3))
                        ecfDocument.Encabezado.Totales.TotalITBIS3 = ti3;

                    // Reference Mapping
                    string? ncfMod = GetString(rawDoc, "NCFModificado");
                    if (!string.IsNullOrEmpty(ncfMod))
                    {
                        ecfDocument.InformacionReferencia = new InformacionReferencia
                        {
                            NCFModificado = ncfMod,
                            RNCOtroContribuyente = GetString(rawDoc, "RNCOtroContribuyente"),
                            FechaNCFModificado = GetString(rawDoc, "FechaNCFModificado"),
                            RazonModificacion = GetString(rawDoc, "RazonModificacion")
                        };
                        if (int.TryParse(GetString(rawDoc, "CodigoModificacion"), out int cm))
                            ecfDocument.InformacionReferencia.CodigoModificacion = cm;
                    }

                    // Mapping Items, Subtotales, and Global adjustments
                    var itemsDict = new SortedDictionary<int, Item>();
                    var subtotalsDict = new SortedDictionary<int, Subtotal>();
                    var adjustmentsDict = new SortedDictionary<int, DescuentoORecargo>();

                    foreach (var kvp in rawDoc)
                    {
                        var match = Regex.Match(kvp.Key, @"(\w+)?\[(\d+)\]");
                        if (match.Success)
                        {
                            string fieldName = match.Groups[1].Value;
                            int index = int.Parse(match.Groups[2].Value);
                            string value = kvp.Value?.ToString() ?? "";

                            if (value == "#e") continue;

                            // Line Items
                            if (fieldName.Contains("Item") || fieldName == "IndicadorFacturacion" || fieldName == "UnidadMedida" || fieldName == "FechaElaboracion" || fieldName == "FechaVencimientoItem")
                            {
                                if (!itemsDict.ContainsKey(index)) itemsDict[index] = new Item { NumeroLinea = index };
                                var item = itemsDict[index];
                                switch (fieldName)
                                {
                                    case "NombreItem": item.NombreItem = value; break;
                                    case "PrecioUnitarioItem": if (decimal.TryParse(value, out decimal p)) item.PrecioUnitario = p; break;
                                    case "CantidadItem": if (decimal.TryParse(value, out decimal c)) item.Cantidad = c; break;
                                    case "MontoItem": if (decimal.TryParse(value, out decimal m)) item.MontoItem = m; break;
                                    case "DescuentoMonto": if (decimal.TryParse(value, out decimal d)) item.DescuentoMonto = d; break;
                                    case "RecargoMonto": if (decimal.TryParse(value, out decimal r)) item.RecargoMonto = r; break;
                                    case "IndicadorBienoServicio": if (int.TryParse(value, out int b)) item.IndicadorBienoServicio = b; break;
                                    case "IndicadorFacturacion": if (int.TryParse(value, out int f)) item.IndicadorFacturacion = f; break;
                                    case "UnidadMedida": item.UnidadMedida = value; break;
                                    case "FechaElaboracion": item.FechaElaboracion = value; break;
                                    case "FechaVencimientoItem": item.FechaVencimientoItem = value; break;
                                }
                            }
                            // Global Adjustments
                            else if (fieldName == "TipoAjuste" || fieldName == "MontoDescuentooRecargo" || fieldName == "DescripcionDescuentooRecargo" || fieldName == "TipoValor" || fieldName == "ValorDescuentooRecargo" || fieldName == "IndicadorFacturacionDescuentooRecargo")
                            {
                                if (!adjustmentsDict.ContainsKey(index)) adjustmentsDict[index] = new DescuentoORecargo { NumeroLinea = index };
                                var adj = adjustmentsDict[index];
                                switch (fieldName)
                                {
                                    case "TipoAjuste": adj.TipoAjuste = value; break;
                                    case "DescripcionDescuentooRecargo": adj.DescripcionDescuentooRecargo = value; break;
                                    case "TipoValor": adj.TipoValor = value; break;
                                    case "ValorDescuentooRecargo": if (decimal.TryParse(value, out decimal v)) adj.ValorDescuentooRecargo = v; break;
                                    case "MontoDescuentooRecargo": if (decimal.TryParse(value, out decimal m)) adj.MontoDescuentooRecargo = m; break;
                                    case "IndicadorFacturacionDescuentooRecargo": if (int.TryParse(value, out int f)) adj.IndicadorFacturacion = f; break;
                                }
                            }
                            // Subtotals
                            else if (fieldName.StartsWith("SubTotal") || fieldName == "NumeroSubTotal" || fieldName == "DescripcionSubtotal" || fieldName == "Orden")
                            {
                                if (!subtotalsDict.ContainsKey(index)) subtotalsDict[index] = new Subtotal { NumeroSubTotal = index };
                                var st = subtotalsDict[index];
                                switch (fieldName)
                                {
                                    case "NumeroSubTotal": if (int.TryParse(value, out int n)) st.NumeroSubTotal = n; break;
                                    case "DescripcionSubtotal": st.DescripcionSubtotal = value; break;
                                    case "Orden": if (int.TryParse(value, out int o)) st.Orden = o; break;
                                    case "MontoSubTotal": if (decimal.TryParse(value, out decimal m)) st.MontoSubTotal = m; break;
                                }
                            }
                        }
                    }

                    foreach (var item in itemsDict.Values) if (!string.IsNullOrEmpty(item.NombreItem)) ecfDocument.Detalles.Items.Add(item);
                    
                    if (adjustmentsDict.Any())
                    {
                        ecfDocument.DescuentosORecargos = new DescuentosORecargos();
                        ecfDocument.DescuentosORecargos.Items.AddRange(adjustmentsDict.Values);
                    }

                    if (subtotalsDict.Any())
                    {
                        ecfDocument.Subtotales = new Subtotales();
                        ecfDocument.Subtotales.Items.AddRange(subtotalsDict.Values);
                    }

                    documents.Add(ecfDocument);
                }
            }

            return documents;
        }

        private string? GetString(Dictionary<string, object> dict, string key)
        {
            if (dict.TryGetValue(key, out var value) && value != null)
            {
                string valStr = value.ToString()?.Trim() ?? "";
                return (valStr == "#e" || valStr == "") ? null : valStr;
            }
            return null;
        }
    }
}
