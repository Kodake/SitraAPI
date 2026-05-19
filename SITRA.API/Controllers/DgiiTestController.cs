using Microsoft.AspNetCore.Mvc;
using SITRA.Core.Interfaces;

namespace SITRA.API.Controllers
{
    [ApiController]
    [Route("api/dgiiTests")]
    public class DgiiTestController : ControllerBase
    {
        private readonly IDgiiTestService _dgiiTestService;
        private readonly IDgiiApiService _dgiiApiService;
        private readonly ISignatureService _signatureService;

        public DgiiTestController(IDgiiTestService dgiiTestService, IDgiiApiService dgiiApiService, ISignatureService signatureService)
        {
            _dgiiTestService = dgiiTestService;
            _dgiiApiService = dgiiApiService;
            _signatureService = signatureService;
        }

        [HttpPost("run-tests")]
        public async Task<IActionResult> RunDgiiTests([FromQuery] string? ecfType = null)
        {
            try
            {
                var result = await _dgiiTestService.RunTestsAsync(ecfType);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message,
                    Stack = ex.StackTrace
                });
            }
        }

        [HttpPost("rfce/run-tests")]
        public async Task<IActionResult> RunRfceTests()
        {
            try
            {
                var result = await _dgiiTestService.RunRfceTestsAsync();
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message,
                    Stack = ex.StackTrace
                });
            }
        }

        [HttpPost("acecf/run-tests")]
        public async Task<IActionResult> RunAcecfTests()
        {
            try
            {
                var result = await _dgiiTestService.RunAprobacionComercialTestsAsync();
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message,
                    Stack = ex.StackTrace
                });
            }
        }

        [HttpGet("/fe/autenticacion/api/semilla")]
        public async Task<IActionResult> GetSemillaAsync()
        {
            try
            {
                var result = await _dgiiApiService.GetSemillaAsync();
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message,
                    Stack = ex.StackTrace
                });
            }
        }

        [HttpPost("/fe/autenticacion/api/validacioncertificado")]
        public async Task<IActionResult> ValidarSemillaAsync([FromBody] ValidarSemillaRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.SeedXmlSigned))
                {
                    return BadRequest("SeedXmlSigned is required");
                }

                // Obtener ruta del certificado base desde el proyecto
                string caCertPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "SITRA.Core", "Resources", "DGIIRootCertificateAuthority", "camaracomercio.crt");
                caCertPath = Path.GetFullPath(caCertPath);

                if (System.IO.File.Exists(caCertPath))
                {
                    bool isValidSignature = _signatureService.ValidateXmlSignature(request.SeedXmlSigned, caCertPath);

                    if (!isValidSignature)
                    {
                        return BadRequest("La firma XML es inválida o el certificado no fue emitido por la Autoridad Certificadora de la Cámara de Comercio.");
                    }
                }

                var result = await _dgiiApiService.GetTokenAsync(request.SeedXmlSigned);
                return Ok(new { token = result });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message,
                    Stack = ex.StackTrace
                });
            }
        }
    }

    public class ValidarSemillaRequest
    {
        public string SeedXmlSigned { get; set; } = string.Empty;
    }
}
