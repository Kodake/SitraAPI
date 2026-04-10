using System;
using System.IO;
using System.Collections.Generic;
using ClosedXML.Excel;
using SITRA.Core.DTOs.DGII;

namespace SITRA.Infrastructure.Services.DGII
{
    public class TestCasesReaderService
    {
        public List<EcfDocument> ReadTestCases(Stream excelStream)
        {
            var documents = new List<EcfDocument>();

            using (var workbook = new XLWorkbook(excelStream))
            {
                // By default DGII might have different sheets or one sheet "E31", "E32", etc.
                // For simplicity, we read the first active worksheet
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed();

                bool isFirstRow = true;

                int rowIndex = 1;
                foreach (var row in rows)
                {
                    if (isFirstRow)
                    {
                        isFirstRow = false;
                        rowIndex++;
                        continue; // Skip header
                    }

                    // A generic mapping - we will adjust these indexes based on the actual Excel layout.
                    var ecfDocument = new EcfDocument();
                    
                    ecfDocument.Encabezado.Version = "1.0";
                    ecfDocument.Encabezado.IdDoc.TipoeCF = "31";
                    ecfDocument.Encabezado.IdDoc.ENCF = "E310000000001";
                    ecfDocument.TestCaseName = $"Fila {rowIndex} - Comprobante Tipo {ecfDocument.Encabezado.IdDoc.TipoeCF}";

                    ecfDocument.Encabezado.IdDoc.FechaVencimientoSecuencia = DateTime.Now.AddYears(1).ToString("dd-MM-yyyy");

                    ecfDocument.Encabezado.Emisor.RNCEmisor = "133556545";
                    ecfDocument.Encabezado.Emisor.RazonSocial = "Emisor de Pruebas SITRA";
                    ecfDocument.Encabezado.Emisor.FechaEmision = DateTime.Now.ToString("dd-MM-yyyy");
                    ecfDocument.Encabezado.Emisor.DireccionEmisor = "Av. Pruebas DGII, Distrito Nacional";
                    
                    ecfDocument.Encabezado.Comprador.RNCComprador = "101010101";
                    ecfDocument.Encabezado.Comprador.RazonSocialComprador = "Comprador de Pruebas DGII";
                    
                    if (decimal.TryParse(row.Cell(7).GetString(), out decimal total))
                    {
                        ecfDocument.Encabezado.Totales.MontoTotal = total;
                        ecfDocument.Encabezado.Totales.TotalITBIS = 0; // Default ITBIS para que mande 0 antes que MontoTotal
                    }

                    // Example Details Item
                    var item = new Item
                    {
                        NumeroLinea = 1,
                        NombreItem = "Servicios o Bienes",
                        Cantidad = 1,
                        PrecioUnitario = ecfDocument.Encabezado.Totales.MontoTotal,
                        MontoItem = ecfDocument.Encabezado.Totales.MontoTotal
                    };
                    ecfDocument.Detalles.Items.Add(item);

                    documents.Add(ecfDocument);
                    rowIndex++;
                }
            }

            return documents;
        }
    }
}
