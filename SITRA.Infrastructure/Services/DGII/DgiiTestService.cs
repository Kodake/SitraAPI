using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SITRA.Core.Interfaces;
using SITRA.Core.Models.Dgii;

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

            if (!string.IsNullOrEmpty(ecfType))
            {
                documents = documents.FindAll(d => d.Encabezado.IdDoc.TipoeCF == ecfType);
            }

            // Step 3: Execute Tests
            foreach (var doc in documents)
            {
                var result = new DgiiTestResult { TestName = doc.TestCaseName };
                
                if (doc.IsRfce)
                {
                    result.Status = "Skipped";
                    result.Error = "Test case belongs to RFCE Summary (B2C) channel. Skipped to prevent 'Invalid Channel' rejection.";
                    results.Add(result);
                    continue;
                }

                try
                {
                    string xmlSinFirma = _ecfXmlGenerator.GenerateEcfXml(doc);
                    string xmlFirmado = _signatureService.SignXml(xmlSinFirma, _settings.CertificatePath, _settings.CertificatePassword);

                    string rawResponse = await _dgiiApiService.SendEcfAsync(xmlFirmado, token);
                    
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
                Message = "Ronda de pruebas completada",
                Results = results
            };
        }
    }
}
