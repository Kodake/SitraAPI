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
        public async Task<IActionResult> ValidarSemillaAsync(IFormFile xml)
        {
            try
            {
                if (xml == null || xml.Length == 0)
                {
                    return BadRequest("xml file is required");
                }

                string xmlContent;
                using (var reader = new StreamReader(xml.OpenReadStream()))
                {
                    xmlContent = await reader.ReadToEndAsync();
                }

                // Obtener ruta del certificado base desde el proyecto
                string caCertPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "SITRA.Core", "Resources", "DGIIRootCertificateAuthority", "camaracomercio.crt");
                caCertPath = Path.GetFullPath(caCertPath);

                if (System.IO.File.Exists(caCertPath))
                {
                    bool isValidSignature = _signatureService.ValidateXmlSignature(xmlContent, caCertPath);

                    if (!isValidSignature)
                    {
                        return BadRequest("La firma XML es inválida o el certificado no fue emitido por la Autoridad Certificadora de la Cámara de Comercio.");
                    }
                }

                var result = await _dgiiApiService.GetTokenAsync(xmlContent);
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

        [HttpPost("/fe/recepcion/api/ecf")]
        public async Task<IActionResult> RecepcionEcfAsync()
        {
            try
            {
                string xmlContent;

                // Soporte dual: leer de multipart (Postman/Swagger) o del raw body (DGII Webhook)
                if (Request.HasFormContentType && Request.Form.Files.Count > 0)
                {
                    var xmlFile = Request.Form.Files[0];
                    using (var reader = new StreamReader(xmlFile.OpenReadStream()))
                    {
                        xmlContent = await reader.ReadToEndAsync();
                    }
                }
                else
                {
                    using (var reader = new StreamReader(Request.Body))
                    {
                        xmlContent = await reader.ReadToEndAsync();
                    }
                }

                if (string.IsNullOrWhiteSpace(xmlContent))
                {
                    return Content("<Error>XML content is empty</Error>", "text/xml");
                }

                string rncEmisor = "133556545"; // Debe ser de 9 u 11 dígitos
                string rncComprador = "133556545"; // Debe ser de 9 u 11 dígitos
                string eNcf = "E310000000001";

                try
                {
                    var doc = System.Xml.Linq.XDocument.Parse(xmlContent);
                    
                    // Buscar elementos ignorando el Namespace (para evitar fallos por prefijos)
                    var emisorNode = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "RNCEmisor");
                    var compradorNode = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "RNCComprador");
                    var encfNode = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "eNCF");

                    if (emisorNode != null && !string.IsNullOrWhiteSpace(emisorNode.Value)) rncEmisor = emisorNode.Value;
                    if (compradorNode != null && !string.IsNullOrWhiteSpace(compradorNode.Value)) rncComprador = compradorNode.Value;
                    if (encfNode != null && !string.IsNullOrWhiteSpace(encfNode.Value)) eNcf = encfNode.Value;
                }
                catch { } // Si falla el parseo, usamos los valores por defecto

                string fechaHora = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                // Construir el ARECF (Acuse de Recibo) SIN el namespace, ya que el XSD de DGII no lo requiere para ARECF
                string arecfXml = $@"<ARECF>
  <DetalleAcusedeRecibo>
    <Version>1.0</Version>
    <RNCEmisor>{rncComprador}</RNCEmisor>
    <RNCComprador>{rncEmisor}</RNCComprador>
    <eNCF>{eNcf}</eNCF>
    <Estado>0</Estado>
    <FechaHoraAcuseRecibo>{fechaHora}</FechaHoraAcuseRecibo>
  </DetalleAcusedeRecibo>
</ARECF>";

                // Firmar el Acuse de Recibo con tu certificado
                string signedArecf = _signatureService.SignXml(arecfXml);

                // Asegurar que la declaración XML esté presente al principio (OuterXml a veces la quita)
                if (!signedArecf.StartsWith("<?xml"))
                {
                    signedArecf = $@"<?xml version=""1.0"" encoding=""utf-8""?>" + "\n" + signedArecf;
                }

                // Retornar el XML firmado
                return Content(signedArecf, "text/xml");
            }
            catch (System.Exception ex)
            {
                // En caso de error, devolver siempre XML para que la DGII no falle con "Data at root level is invalid"
                string errorXml = $"<Error>{ex.Message}</Error>";
                return Content(errorXml, "text/xml");
            }
        }
    }
}
