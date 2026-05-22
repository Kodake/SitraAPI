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
                return StatusCode(500, new { Error = ex.Message, Stack = ex.StackTrace });
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
                return StatusCode(500, new { Error = ex.Message, Stack = ex.StackTrace });
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
                return StatusCode(500, new { Error = ex.Message, Stack = ex.StackTrace });
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
                return StatusCode(500, new { Error = ex.Message, Stack = ex.StackTrace });
            }
        }

        [HttpPost("/fe/autenticacion/api/validacioncertificado")]
        public async Task<IActionResult> ValidarSemillaAsync(IFormFile xml)
        {
            try
            {
                if (xml == null || xml.Length == 0) return BadRequest("xml file is required");

                string xmlContent;
                using (var reader = new StreamReader(xml.OpenReadStream()))
                {
                    xmlContent = await reader.ReadToEndAsync();
                }

                string caCertPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "SITRA.Core", "Resources", "DGIIRootCertificateAuthority", "camaracomercio.crt");
                caCertPath = Path.GetFullPath(caCertPath);

                if (System.IO.File.Exists(caCertPath))
                {
                    bool isValidSignature = _signatureService.ValidateXmlSignature(xmlContent, caCertPath);
                    if (!isValidSignature) return BadRequest("La firma XML es inválida o el certificado no fue emitido por la Autoridad Certificadora de la Cámara de Comercio.");
                }

                var result = await _dgiiApiService.GetTokenAsync(xmlContent);
                return Ok(new { token = result });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message, Stack = ex.StackTrace });
            }
        }

        [HttpPost("/fe/recepcion/api/ecf")]
        public async Task<IActionResult> RecepcionEcfAsync()
        {
            try
            {
                string xmlContent;

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

                string rncEmisor = string.Empty;
                string rncComprador = string.Empty;
                string eNcf = string.Empty;

                try
                {
                    var doc = System.Xml.Linq.XDocument.Parse(xmlContent);
                    var emisorNode = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "RNCEmisor");
                    var compradorNode = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "RNCComprador");
                    var encfNode = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "eNCF");

                    if (emisorNode != null) rncEmisor = emisorNode.Value;
                    if (compradorNode != null) rncComprador = compradorNode.Value;
                    if (encfNode != null) eNcf = encfNode.Value;
                }
                catch { }

                var arecfResponse = new SITRA.Core.DTOs.DGII.ArecfDocumentTest
                {
                    Detalle = new SITRA.Core.DTOs.DGII.DetalleAcusedeRecibo
                    {
                        Version = "1.0",
                        RNCEmisor = rncEmisor,
                        RNCComprador = rncComprador,
                        ENCF = eNcf,
                        Estado = 0,
                        FechaHoraAcuseRecibo = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
                    }
                };

                var emptyNamespaces = new System.Xml.Serialization.XmlSerializerNamespaces(new[] { System.Xml.XmlQualifiedName.Empty });
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SITRA.Core.DTOs.DGII.ArecfDocumentTest));
                var settings = new System.Xml.XmlWriterSettings { OmitXmlDeclaration = false, Encoding = new System.Text.UTF8Encoding(false), Indent = true };

                string arecfXml;
                using (var stringWriter = new StringWriter())
                using (var xmlWriter = System.Xml.XmlWriter.Create(stringWriter, settings))
                {
                    serializer.Serialize(xmlWriter, arecfResponse, emptyNamespaces);
                    arecfXml = stringWriter.ToString();
                }

                string signedArecf = _signatureService.SignXml(arecfXml);

                if (!signedArecf.StartsWith("<?xml"))
                {
                    signedArecf = $@"<?xml version=""1.0"" encoding=""utf-8""?>" + "\n" + signedArecf;
                }

                return Content(signedArecf, "text/xml");
            }
            catch (System.Exception ex)
            {
                return Content($"<Error>{ex.Message}</Error>", "text/xml");
            }
        }

        [HttpPost("/fe/aprobacioncomercial/api/ecf")]
        public async Task<IActionResult> AprobacionComercialEcfAsync()
        {
            try
            {
                string xmlContent;

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

                string rncEmisor = string.Empty;
                string rncComprador = string.Empty;
                string eNcf = string.Empty;
                string fechaEmision = string.Empty;
                string montoTotal = string.Empty;

                try
                {
                    var doc = System.Xml.Linq.XDocument.Parse(xmlContent);
                    var emisorNode = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "RNCEmisor");
                    var compradorNode = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "RNCComprador");
                    var encfNode = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "eNCF");
                    var fechaEmisionNode = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "FechaEmision");
                    var montoTotalNode = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "MontoTotal");

                    if (emisorNode != null) rncEmisor = emisorNode.Value;
                    if (compradorNode != null) rncComprador = compradorNode.Value;
                    if (encfNode != null) eNcf = encfNode.Value;
                    if (fechaEmisionNode != null) fechaEmision = fechaEmisionNode.Value;
                    if (montoTotalNode != null) montoTotal = montoTotalNode.Value;
                }
                catch { }

                var acecfResponse = new SITRA.Core.DTOs.DGII.AcecfDocumentTest
                {
                    Detalle = new SITRA.Core.DTOs.DGII.DetalleAprobacionComercialTest
                    {
                        Version = "1.0",
                        RNCEmisor = rncEmisor,
                        ENCF = eNcf,
                        FechaEmision = fechaEmision,
                        MontoTotal = montoTotal,
                        RNCComprador = rncComprador,
                        Estado = 1, // 1 = Aceptado, 2 = Rechazado
                        FechaHoraAprobacionComercial = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
                    }
                };

                var emptyNamespaces = new System.Xml.Serialization.XmlSerializerNamespaces(new[] { System.Xml.XmlQualifiedName.Empty });
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SITRA.Core.DTOs.DGII.AcecfDocumentTest));
                var settings = new System.Xml.XmlWriterSettings { OmitXmlDeclaration = false, Encoding = new System.Text.UTF8Encoding(false), Indent = true };

                string acecfXml;
                using (var stringWriter = new StringWriter())
                using (var xmlWriter = System.Xml.XmlWriter.Create(stringWriter, settings))
                {
                    serializer.Serialize(xmlWriter, acecfResponse, emptyNamespaces);
                    acecfXml = stringWriter.ToString();
                }

                string signedAcecf = _signatureService.SignXml(acecfXml);

                if (!signedAcecf.StartsWith("<?xml"))
                {
                    signedAcecf = $@"<?xml version=""1.0"" encoding=""utf-8""?>" + "\n" + signedAcecf;
                }

                return Content(signedAcecf, "text/xml");
            }
            catch (System.Exception ex)
            {
                return Content($"<Error>{ex.Message}</Error>", "text/xml");
            }
        }
    }
}
