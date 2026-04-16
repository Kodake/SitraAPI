using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SITRA.Core.Interfaces;
using SITRA.Core.Models.Dgii;
using SITRA.Core.DTOs.DGII;

namespace SITRA.Infrastructure.Services.DGII
{
    public class DgiiTestService : IDgiiTestService
    {
        private readonly IEnumerable<ITestCasesReader> _testCasesReaders;
        private readonly IEcfXmlGenerator _ecfXmlGenerator;
        private readonly IDgiiApiService _dgiiApiService;
        private readonly ISignatureService _signatureService;
        private readonly DgiiSettings _settings;

        public DgiiTestService(
            IEnumerable<ITestCasesReader> testCasesReaders,
            IEcfXmlGenerator ecfXmlGenerator,
            IDgiiApiService dgiiApiService,
            ISignatureService signatureService,
            IOptions<DgiiSettings> settings)
        {
            _testCasesReaders = testCasesReaders;
            _ecfXmlGenerator = ecfXmlGenerator;
            _dgiiApiService = dgiiApiService;
            _signatureService = signatureService;
            _settings = settings.Value;
        }

        public async Task<DgiiTestSuiteResult> RunTestsAsync(string? ecfType = null)
        {
            return await RunTestSuiteAsync(ecfType, false);
        }

        public async Task<DgiiTestSuiteResult> RunRfceTestsAsync()
        {
            return await RunTestSuiteAsync(null, true);
        }

        private async Task<DgiiTestSuiteResult> RunTestSuiteAsync(string? ecfType, bool forRfce)
        {
            var results = new List<DgiiTestResult>();

            if (!File.Exists(_settings.TestFilePath))
            {
                throw new FileNotFoundException($"Test dataset not found at: {_settings.TestFilePath}");
            }

            // Step 1: Authentication
            string seedXml = await _dgiiApiService.GetSemillaAsync();
            string signedSeed = _signatureService.SignAuthenticationSeed(seedXml, _settings.CertificatePath, _settings.CertificatePassword);
            string token = await _dgiiApiService.GetTokenAsync(signedSeed);

            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Failed to retrieve DGII authentication token.");
            }

            // Step 2: Read Test Cases
            using var stream = File.OpenRead(_settings.TestFilePath);
            var extension = Path.GetExtension(_settings.TestFilePath).ToLower();
            
            ITestCasesReader reader = extension switch
            {
                ".json" => _testCasesReaders.OfType<JsonTestCasesReaderService>().FirstOrDefault() ?? throw new Exception("JSON Reader not registered"),
                ".xlsx" => _testCasesReaders.OfType<TestCasesReaderService>().FirstOrDefault() ?? throw new Exception("Excel Reader not registered"),
                _ => throw new NotSupportedException($"Unsupported file extension: {extension}")
            };

            var documents = reader.ReadTestCases(stream);

            if (forRfce)
            {
                documents = documents.FindAll(d => d.IsRfce);
            }
            else if (!string.IsNullOrEmpty(ecfType))
            {
                documents = documents.FindAll(d => d.Encabezado.IdDoc.TipoeCF == ecfType);
            }

            // Step 3: Execute Tests
            foreach (var doc in documents)
            {
                var result = new DgiiTestResult { TestName = doc.TestCaseName };
                
                if (!forRfce && doc.IsRfce)
                {
                    result.Status = "Skipped";
                    result.Error = "Test case belongs to RFCE Summary (B2C) channel. Skipped to prevent 'Invalid Channel' rejection.";
                    results.Add(result);
                    continue;
                }

                try
                {
                    string xmlFirmado = string.Empty;

                    if (forRfce)
                    {
                        var rfceDoc = MapToRfce(doc);
                        string xmlSinFirma = _ecfXmlGenerator.GenerateRfceXml(rfceDoc);
                        xmlFirmado = _signatureService.SignXml(xmlSinFirma, _settings.CertificatePath, _settings.CertificatePassword);
                    }
                    else
                    {
                        string xmlSinFirma = _ecfXmlGenerator.GenerateEcfXml(doc);
                        xmlFirmado = _signatureService.SignXml(xmlSinFirma, _settings.CertificatePath, _settings.CertificatePassword);
                    }

                    string rawResponse = forRfce 
                        ? await _dgiiApiService.SendRfceAsync(xmlFirmado, token)
                        : await _dgiiApiService.SendEcfAsync(xmlFirmado, token);
                    
                    using (JsonDocument docJson = JsonDocument.Parse(rawResponse))
                    {
                        if (docJson.RootElement.TryGetProperty("trackId", out JsonElement trackIdElement))
                        {
                            result.TrackId = trackIdElement.GetString();
                        }
                        else
                        {
                            result.TrackId = rawResponse; // Fallback if property is missing
                        }
                    }
                    
                    result.Status = "Success";
                }
                catch (Exception ex)
                {
                    result.Status = "Failed";
                    result.Error = ex.Message;
                }
                results.Add(result);
            }

            return new DgiiTestSuiteResult
            {
                Token = token,
                Message = forRfce ? "Pruebas RFCE completadas" : "Ronda de pruebas completada",
                Results = results
            };
        }

        private static RfceDocument MapToRfce(EcfDocument doc)
        {
            var rfce = new RfceDocument
            {
                Encabezado = new RfceEncabezado
                {
                    CodigoSeguridadeCF = doc.CodigoSeguridad ?? "000000",
                    Version = doc.Encabezado.Version,
                    IdDoc = new RfceIdDoc
                    {
                        TipoeCF = "32",
                        eNCF = doc.Encabezado.IdDoc.ENCF,
                        TipoIngresos = doc.Encabezado.IdDoc.TipoIngresos ?? "01",
                        TipoPago = doc.Encabezado.IdDoc.TipoPago ?? "1"
                    },
                    Emisor = new RfceEmisor
                    {
                        RNCEmisor = doc.Encabezado.Emisor.RNCEmisor,
                        RazonSocialEmisor = doc.Encabezado.Emisor.RazonSocial,
                        FechaEmision = doc.Encabezado.Emisor.FechaEmision
                    },
                    Comprador = new RfceComprador
                    {
                        RNCComprador = doc.Encabezado.Comprador.RNCComprador,
                        IdentificadorExtranjero = doc.Encabezado.Comprador.IdentificadorExtranjero,
                        RazonSocialComprador = doc.Encabezado.Comprador.RazonSocialComprador
                    },
                    Totales = new RfceTotales
                    {
                        MontoGravadoTotal = (decimal?)doc.Encabezado.Totales.MontoGravadoTotal,
                        MontoGravadoI1 = (decimal?)doc.Encabezado.Totales.MontoGravadoI1,
                        MontoGravadoI2 = (decimal?)doc.Encabezado.Totales.MontoGravadoI2,
                        MontoGravadoI3 = (decimal?)doc.Encabezado.Totales.MontoGravadoI3,
                        MontoExento = (decimal?)doc.Encabezado.Totales.MontoExento,
                        TotalITBIS = (decimal?)doc.Encabezado.Totales.TotalITBIS,
                        TotalITBIS1 = (decimal?)doc.Encabezado.Totales.TotalITBIS1,
                        TotalITBIS2 = (decimal?)doc.Encabezado.Totales.TotalITBIS2,
                        TotalITBIS3 = (decimal?)doc.Encabezado.Totales.TotalITBIS3,
                        MontoImpuestoAdicional = (decimal?)doc.Encabezado.Totales.MontoImpuestoAdicional,
                        MontoTotal = (decimal)doc.Encabezado.Totales.MontoTotal,
                        MontoNoFacturable = (decimal?)doc.Encabezado.Totales.MontoNoFacturable,
                        MontoPeriodo = (decimal?)doc.Encabezado.Totales.MontoPeriodo
                    }
                }
            };

            if (doc.Encabezado.IdDoc.TablaFormasPago != null)
            {
                rfce.Encabezado.IdDoc.TablaFormasPago = new RfceTablaFormasPago
                {
                    FormaDePago = doc.Encabezado.IdDoc.TablaFormasPago.Select(p => new RfceFormaDePago
                    {
                        FormaPago = p.FormaPago,
                        MontoPago = (decimal?)p.MontoPago
                    }).ToList()
                };
            }

            return rfce;
        }
    }
}
