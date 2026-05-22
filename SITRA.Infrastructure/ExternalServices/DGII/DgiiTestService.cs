using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services;
using SITRA.Core.Models.Dgii;
using SITRA.Core.DTOs.DGII;

namespace SITRA.Infrastructure.ExternalServices.DGII
{
    public class DgiiTestService : IDgiiTestService
    {
        private readonly IEnumerable<ITestCasesReader> _testCasesReaders;
        private readonly IEcfXmlGenerator _ecfXmlGenerator;
        private readonly IDgiiApiService _dgiiApiService;
        private readonly ISignatureService _signatureService;
        private readonly DgiiSettings _settings;
        private readonly IDgiiStorageService _storageService;

        public DgiiTestService(
            IEnumerable<ITestCasesReader> testCasesReaders,
            IEcfXmlGenerator ecfXmlGenerator,
            IDgiiApiService dgiiApiService,
            ISignatureService signatureService,
            IOptionsSnapshot<DgiiSettings> settings,
            IDgiiStorageService storageService)
        {
            _testCasesReaders = testCasesReaders;
            _ecfXmlGenerator = ecfXmlGenerator;
            _dgiiApiService = dgiiApiService;
            _signatureService = signatureService;
            _settings = settings.Get("DgiiTest");
            _storageService = storageService;
        }

        public async Task<DgiiTestSuiteResult> RunTestsAsync(string? ecfType = null)
        {
            return await RunTestSuiteAsync(ecfType, false, false);
        }

        public async Task<DgiiTestSuiteResult> RunRfceTestsAsync()
        {
            return await RunTestSuiteAsync(null, true, false);
        }

        public async Task<DgiiTestSuiteResult> RunAprobacionComercialTestsAsync()
        {
            return await RunTestSuiteAsync(null, false, true);
        }

        private async Task<DgiiTestSuiteResult> RunTestSuiteAsync(string? ecfType, bool forRfce, bool forAcecf)
        {
            var results = new List<DgiiTestResult>();

            if (!File.Exists(ResolvePath(_settings.TestFilePath)))
            {
                throw new FileNotFoundException($"Test dataset not found at: {ResolvePath(_settings.TestFilePath)}");
            }

            // Step 1: Authentication
            string seedXml = await _dgiiApiService.GetSemillaAsync();
            byte[] certBytes = await _storageService.ReadCertificateAsync();
            string signedSeed = _signatureService.SignAuthenticationSeed(seedXml, certBytes, _settings.CertificatePassword);
            string token = await _dgiiApiService.GetTokenAsync(signedSeed);

            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Failed to retrieve DGII authentication token.");
            }

            // Step 2: Read Test Cases
            var filePath = ResolvePath(forAcecf ? _settings.TestFilePath2 : _settings.TestFilePath);

            using var stream = File.OpenRead(filePath);
            var extension = Path.GetExtension(filePath).ToLower();

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
            else if (forAcecf)
            {
                documents = documents.FindAll(d => d.IsAcecf);
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
                    bool useRfceChannel = forRfce;
                    bool useAcecfChannel = forAcecf;

                    if (forAcecf)
                    {
                        var acecfDoc = MapToAcecf(doc);
                        string xmlSinFirma = _ecfXmlGenerator.GenerateAcecfXml(acecfDoc);
                        xmlFirmado = _signatureService.SignXml(xmlSinFirma, certBytes, _settings.CertificatePassword);

                        await SaveDocumentXmlAsync(xmlFirmado, doc.Encabezado.IdDoc.ENCF, doc.Encabezado.Emisor.RNCEmisor, "ACECF");
                    }
                    else if (forRfce)
                    {
                        // 1. Generate and sign the base ECF (Type 32)
                        string baseEcfXml = _ecfXmlGenerator.GenerateEcfXml(doc);
                        string signedBaseEcf = _signatureService.SignXml(baseEcfXml, certBytes, _settings.CertificatePassword);

                        // 2. Save the base e-CF in the "32" folder (always archive individual invoices)
                        await SaveDocumentXmlAsync(signedBaseEcf, doc.Encabezado.IdDoc.ENCF, doc.Encabezado.Emisor.RNCEmisor, "32");

                        // 3. Extract the security code and generate RFCE
                        string securityCode = GetSecurityCodeFromSignedXml(signedBaseEcf);
                        var rfceDoc = MapToRfce(doc);
                        rfceDoc.Encabezado.CodigoSeguridadeCF = securityCode;

                        string xmlSinFirma = _ecfXmlGenerator.GenerateRfceXml(rfceDoc);
                        xmlFirmado = _signatureService.SignXml(xmlSinFirma, certBytes, _settings.CertificatePassword);

                        // Save the RFCE summary
                        await SaveDocumentXmlAsync(xmlFirmado, doc.Encabezado.IdDoc.ENCF, doc.Encabezado.Emisor.RNCEmisor, "RFCE");
                    }
                    else if (doc.Encabezado.IdDoc.TipoeCF == "32" && doc.Encabezado.Totales.MontoTotal < 250000)
                    {
                        // Automatic redirect for B2C < 250k: Save individual E32, then generate and send RFCE instead
                        string baseEcfXml = _ecfXmlGenerator.GenerateEcfXml(doc);
                        string signedBaseEcf = _signatureService.SignXml(baseEcfXml, certBytes, _settings.CertificatePassword);
                        await SaveDocumentXmlAsync(signedBaseEcf, doc.Encabezado.IdDoc.ENCF, doc.Encabezado.Emisor.RNCEmisor, "32");

                        string securityCode = GetSecurityCodeFromSignedXml(signedBaseEcf);
                        var rfceDoc = MapToRfce(doc);
                        rfceDoc.Encabezado.CodigoSeguridadeCF = securityCode;

                        string rfceXmlSinFirma = _ecfXmlGenerator.GenerateRfceXml(rfceDoc);
                        xmlFirmado = _signatureService.SignXml(rfceXmlSinFirma, certBytes, _settings.CertificatePassword);

                        // Save and prepare for RFCE sending
                        await SaveDocumentXmlAsync(xmlFirmado, doc.Encabezado.IdDoc.ENCF, doc.Encabezado.Emisor.RNCEmisor, "RFCE");
                        useRfceChannel = true;
                    }
                    else
                    {
                        // Standard individual invoice generation
                        string xmlSinFirma = _ecfXmlGenerator.GenerateEcfXml(doc);
                        xmlFirmado = _signatureService.SignXml(xmlSinFirma, certBytes, _settings.CertificatePassword);

                        await SaveDocumentXmlAsync(xmlFirmado, doc.Encabezado.IdDoc.ENCF, doc.Encabezado.Emisor.RNCEmisor, doc.Encabezado.IdDoc.TipoeCF);
                    }

                    string rawResponse = useAcecfChannel
                        ? await _dgiiApiService.SendAprobacionComercialAsync(xmlFirmado, token)
                        : useRfceChannel
                            ? await _dgiiApiService.SendRfceAsync(xmlFirmado, token)
                            : await _dgiiApiService.SendEcfAsync(xmlFirmado, token);

                    using (JsonDocument docJson = JsonDocument.Parse(rawResponse))
                    {
                        if (docJson.RootElement.TryGetProperty("trackId", out JsonElement trackIdElement))
                        {
                            result.TrackId = trackIdElement.GetString();
                        }
                        else if (docJson.RootElement.TryGetProperty("estado", out JsonElement estadoElement))
                        {
                            string estado = estadoElement.GetString() ?? string.Empty;
                            string encf = docJson.RootElement.TryGetProperty("encf", out JsonElement encfElement)
                                ? encfElement.GetString() ?? string.Empty
                                : string.Empty;

                            result.TrackId = !string.IsNullOrEmpty(encf) ? $"{estado} ({encf})" : estado;
                        }
                        else
                        {
                            result.TrackId = rawResponse;
                        }
                    }

                    result.Status = "Success";
                }
                catch (Exception ex)
                {
                    result.Status = "Failed";
                    string exMessage = ex.Message;

                    try
                    {
                        int jsonStart = exMessage.IndexOf('{');
                        int jsonEnd = exMessage.LastIndexOf('}');

                        if (jsonStart != -1 && jsonEnd != -1 && jsonEnd > jsonStart)
                        {
                            string jsonString = exMessage.Substring(jsonStart, jsonEnd - jsonStart + 1);
                            using (JsonDocument docJson = JsonDocument.Parse(jsonString))
                            {
                                var root = docJson.RootElement;
                                string estado = root.TryGetProperty("estado", out var e) ? e.GetString() ?? "" : "";

                                List<string> errorMessages = new List<string>();
                                if (root.TryGetProperty("mensajes", out var m) && m.ValueKind == JsonValueKind.Array)
                                {
                                    foreach (var msg in m.EnumerateArray())
                                    {
                                        if (msg.TryGetProperty("valor", out var v))
                                        {
                                            errorMessages.Add(v.GetString() ?? "");
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(estado))
                                {
                                    result.Status = $"Failed ({estado})";
                                }

                                result.Error = errorMessages.Count > 0
                                    ? string.Join("; ", errorMessages)
                                    : exMessage;
                            }
                        }
                        else
                        {
                            result.Error = exMessage;
                        }
                    }
                    catch
                    {
                        result.Error = exMessage;
                    }
                }
                results.Add(result);
            }

            return new DgiiTestSuiteResult
            {
                Token = token,
                Message = forRfce ? "Pruebas RFCE completadas" : forAcecf ? "Pruebas ACECF completadas" : "Ronda de pruebas completada",
                Results = results
            };
        }

        private static AcecfDocument MapToAcecf(EcfDocument doc)
        {
            return new AcecfDocument
            {
                Detalle = new DetalleAprobacionComercial
                {
                    Version = doc.Encabezado.Version,
                    RNCEmisor = doc.Encabezado.Emisor.RNCEmisor,
                    ENCF = doc.Encabezado.IdDoc.ENCF,
                    FechaEmision = doc.Encabezado.Emisor.FechaEmision,
                    MontoTotal = doc.Encabezado.Totales.MontoTotal?.ToString("F2") ?? "0.00",
                    RNCComprador = doc.Encabezado.Comprador.RNCComprador ?? string.Empty,
                    Estado = doc.AcecfEstado ?? "1",
                    DetalleMotivoRechazo = doc.AcecfMotivoRechazo,
                    FechaHoraAprobacionComercial = doc.AcecfFechaHora ?? DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
                }
            };
        }

        private async Task SaveDocumentXmlAsync(string xml, string encf, string rnc, string docType)
        {
            try
            {
                // Determine the file name based on doc type
                string fileName = docType switch
                {
                    "RFCE" => $"{rnc}{encf}_RFCE.xml",
                    "ACECF" => BuildAcecfFileName(xml, encf),
                    _ => $"{rnc}{encf}.xml"
                };

                await _storageService.SaveDocumentXmlAsync(xml, rnc, docType, fileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save DGII XML: {ex.Message}");
            }
        }

        private static string BuildAcecfFileName(string xml, string encf)
        {
            try
            {
                var docX = XDocument.Parse(xml);
                var rncComprador = docX.Descendants("RNCComprador").FirstOrDefault()?.Value;
                if (!string.IsNullOrEmpty(rncComprador))
                    return $"{rncComprador}{encf}_ACECF.xml";
            }
            catch { }
            return $"{encf}_ACECF.xml";
        }

        private static string GetSecurityCodeFromSignedXml(string signedXml)
        {
            try
            {
                var doc = XDocument.Parse(signedXml);
                XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";
                var sigValue = doc.Descendants(ds + "SignatureValue").FirstOrDefault()?.Value;

                if (!string.IsNullOrEmpty(sigValue))
                {
                    // Clean the base64 string
                    sigValue = sigValue.Replace("\n", "").Replace("\r", "").Replace(" ", "");
                    // Take first 6 characters as required by DGII
                    return sigValue.Length >= 6 ? sigValue.Substring(0, 6) : sigValue;
                }
            }
            catch { }

            return "000000";
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
                        MontoTotal = doc.Encabezado.Totales.MontoTotal ?? 0,
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
        private static string ResolvePath(string path) =>
            Path.IsPathRooted(path) ? path : Path.Combine(AppContext.BaseDirectory, path);
    }
}
