using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SITRA.Infrastructure.Services.DGII;
using SITRA.Core.Interfaces;

namespace SITRA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DgiiTestController : ControllerBase
    {
        private readonly TestCasesReaderService _testCasesReaderService;
        private readonly EcfXmlGeneratorService _ecfXmlGeneratorService;
        private readonly IDgiiApiService _dgiiApiService;
        private readonly SignatureService _signatureService;

        public DgiiTestController(
            TestCasesReaderService testCasesReaderService,
            EcfXmlGeneratorService ecfXmlGeneratorService,
            IDgiiApiService dgiiApiService,
            IConfiguration configuration)
        {
            _testCasesReaderService = testCasesReaderService;
            _ecfXmlGeneratorService = ecfXmlGeneratorService;
            _dgiiApiService = dgiiApiService;

            // En un ambiente ideal, se inyecta por Tenedor/Tenant
            string certPath = "c:\\Users\\Kodake\\Desktop\\SITRA SRC\\13052984_identity.p12";
            string certPass = configuration["Dgii:CertPassword"] ?? "Silex450";

            _signatureService = new SignatureService(certPath, certPass);
        }

        [HttpPost("run-tests")]
        public async Task<IActionResult> RunDgiiTests()
        {
            string excelPath = @"C:\Users\Kodake\Downloads\133556545-09042026212654.xlsx";

            if (!System.IO.File.Exists(excelPath))
            {
                return BadRequest($"No se encuentra el archivo Excel en: {excelPath}");
            }

            try
            {
                // Paso 1: Obtener el Seed de DGII y firmarlo
                string seedXmlReal = await _dgiiApiService.GetSemillaAsync();
                string signedSeed = _signatureService.SignAuthenticationSeed(seedXmlReal);
                
                string token = await _dgiiApiService.GetTokenAsync(signedSeed);

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("No se pudo obtener el token de la DGII.");
                }

                // Paso 2: Leer los casos de prueba del Excel
                using var stream = System.IO.File.OpenRead(excelPath);
                var documents = _testCasesReaderService.ReadTestCases(stream);

                var testResults = new System.Collections.Generic.List<object>();

                // Paso 3: Iterar cada caso, generar XML, firmar y enviar
                foreach (var doc in documents)
                {
                    string xmlFirmado = string.Empty;
                    try
                    {
                        string xmlSinFirma = _ecfXmlGeneratorService.GenerateEcfXml(doc);
                        xmlFirmado = _signatureService.SignXml(xmlSinFirma);

                        string resultTrackId = await _dgiiApiService.SendEcfAsync(xmlFirmado, token);
                        
                        testResults.Add(new { 
                            Test = doc.TestCaseName, 
                            Status = "Success", 
                            TrackId = resultTrackId, 
                            Error = (string)null 
                        });
                    }
                    catch (Exception ex)
                    {
                        testResults.Add(new { 
                            Test = doc.TestCaseName, 
                            Status = "Failed", 
                            TrackId = (string)null, 
                            Error = ex.Message 
                        });
                    }
                }

                return Ok(new { Message = "Ronda de pruebas completada", Results = testResults });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message, Stack = ex.StackTrace });
            }
        }
    }
}
