using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Models.Dgii;
using SITRA.Infrastructure.Persistence;

namespace SITRA.API.Controllers
{
    [ApiController]
    [Route("api/dgiiTests")]
    public class DgiiTestController : ControllerBase
    {
        private readonly IDgiiTestService _dgiiTestService;
        private readonly IDgiiApiService _dgiiApiService;
        private readonly ISignatureService _signatureService;
        private readonly IDgiiStorageService _storageService;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DgiiTestController> _logger;
        private readonly DgiiSettings _dgiiSettings;

        public DgiiTestController(
            IDgiiTestService dgiiTestService,
            IDgiiApiService dgiiApiService,
            ISignatureService signatureService,
            IDgiiStorageService storageService,
            IDataProtectionProvider dataProtectionProvider,
            ApplicationDbContext dbContext,
            ILogger<DgiiTestController> logger,
            IOptions<DgiiSettings> dgiiOptions)
        {
            _dgiiTestService = dgiiTestService;
            _dgiiApiService = dgiiApiService;
            _signatureService = signatureService;
            _storageService = storageService;
            _dataProtectionProvider = dataProtectionProvider;
            _dbContext = dbContext;
            _logger = logger;
            _dgiiSettings = dgiiOptions.Value;
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

        private async Task<(byte[] CertBytes, string CertPassword, string Rnc)> GetCertificateCredentialsAsync(string? rncComprador = null)
        {
            string configuredPassword = !string.IsNullOrEmpty(_dgiiSettings.CertificatePassword) ? _dgiiSettings.CertificatePassword : string.Empty;

            User? user = null;
            if (!string.IsNullOrEmpty(rncComprador))
            {
                user = await _dbContext.Users.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.RucNumber == rncComprador && !string.IsNullOrEmpty(u.CertificateUrl));
            }

            if (user == null)
            {
                user = await _dbContext.Users.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.UserName == "QA2@GMAIL.COM" && !string.IsNullOrEmpty(u.CertificateUrl))
                    ?? await _dbContext.Users.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => !string.IsNullOrEmpty(u.CertificateUrl));
            }

            if (user != null && !string.IsNullOrEmpty(user.CertificateUrl))
            {
                string password = configuredPassword;
                if (string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(user.CertificatePassword))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector("CertificatePassword");
                        password = protector.Unprotect(user.CertificatePassword);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to unprotect password for user {User}. Using settings password.", user.UserName);
                    }
                }

                if (!string.IsNullOrEmpty(password))
                {
                    try
                    {
                        byte[] bytes = await _storageService.ReadCertificateFromUrlAsync(user.CertificateUrl);
                        string rnc = user.RucNumber ?? _dgiiSettings.DefaultRNC ?? "133002868";
                        _logger.LogInformation("Successfully loaded certificate for user {User} (RNC: {RNC}) from URL", user.UserName, rnc);
                        return (bytes, password, rnc);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to download certificate from URL. Trying local files.");
                    }
                }
            }

            // Fallback to local files
            string[] possiblePaths =
            [
                _dgiiSettings.CertificatePath,
                Path.Combine(AppContext.BaseDirectory, _dgiiSettings.CertificatePath),
                Path.Combine(AppContext.BaseDirectory, "Resources", "Certificates", "20260714-1752000-H80L156Y1.p12"),
                Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Certificates", "20260714-1752000-H80L156Y1.p12"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "SITRA.Application", "Resources", "Certificates", "20260714-1752000-H80L156Y1.p12")
            ];

            foreach (var path in possiblePaths)
            {
                if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                {
                    byte[] bytes = await System.IO.File.ReadAllBytesAsync(path);
                    string rnc = user?.RucNumber ?? _dgiiSettings.DefaultRNC ?? "133002868";
                    _logger.LogInformation("Loaded local certificate from {Path} with RNC {RNC}", path, rnc);
                    return (bytes, configuredPassword, rnc);
                }
            }

            throw new InvalidOperationException($"No digital certificate found for reception.");
        }

        [HttpPost("/fe/recepcion/api/ecf")]
        public async Task<IActionResult> RecepcionEcfAsync()
        {
            try
            {
                string xmlContent = string.Empty;

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
                    _logger.LogWarning("RecepcionEcfAsync: Received empty XML content.");
                    return BadRequest("XML content is empty");
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
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RecepcionEcfAsync: Error parsing incoming XML.");
                }

                var (certBytes, certPassword, ownerRnc) = await GetCertificateCredentialsAsync(rncComprador);

                if (string.IsNullOrEmpty(rncComprador))
                {
                    rncComprador = ownerRnc;
                }

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
                using (var stringWriter = new Utf8StringWriter())
                using (var xmlWriter = System.Xml.XmlWriter.Create(stringWriter, settings))
                {
                    serializer.Serialize(xmlWriter, arecfResponse, emptyNamespaces);
                    arecfXml = stringWriter.ToString();
                }

                string signedArecf = _signatureService.SignXml(arecfXml, certBytes, certPassword);

                if (signedArecf.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
                {
                    int endDecl = signedArecf.IndexOf("?>");
                    if (endDecl != -1)
                    {
                        signedArecf = signedArecf.Substring(endDecl + 2).TrimStart();
                    }
                }
                signedArecf = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + signedArecf;

                _logger.LogInformation("RecepcionEcfAsync: Successfully generated signed ARECF for eNCF: {eNCF}, RNCEmisor: {RNCEmisor}, RNCComprador: {RNCComprador}", eNcf, rncEmisor, rncComprador);

                return Content(signedArecf, "text/xml");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "RecepcionEcfAsync: Exception occurred.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("/fe/aprobacioncomercial/api/ecf")]
        public async Task<IActionResult> AprobacionComercialEcfAsync()
        {
            try
            {
                string xmlContent = string.Empty;

                if (Request.HasFormContentType)
                {
                    if (Request.Form.Files.Count > 0)
                    {
                        var xmlFile = Request.Form.Files[0];
                        using var reader = new StreamReader(xmlFile.OpenReadStream());
                        xmlContent = await reader.ReadToEndAsync();
                    }
                    else if (Request.Form.TryGetValue("xml", out var formXml))
                    {
                        xmlContent = formXml.ToString();
                    }
                }
                else
                {
                    using var reader = new StreamReader(Request.Body);
                    xmlContent = await reader.ReadToEndAsync();
                }

                if (string.IsNullOrWhiteSpace(xmlContent))
                {
                    _logger.LogWarning("AprobacionComercialEcfAsync: Received empty XML content.");
                    return BadRequest("XML content is empty");
                }

                _logger.LogInformation("AprobacionComercialEcfAsync: Received payload (length: {length}):\n{xmlContent}", xmlContent.Length, xmlContent);

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
                catch (Exception ex)
                {
                    _logger.LogError(ex, "AprobacionComercialEcfAsync: Error parsing incoming XML.");
                }

                var (certBytes, certPassword, ownerRnc) = await GetCertificateCredentialsAsync(rncComprador);

                if (string.IsNullOrEmpty(rncComprador))
                {
                    rncComprador = ownerRnc;
                }

                if (string.IsNullOrEmpty(rncEmisor))
                {
                    rncEmisor = ownerRnc;
                }

                if (decimal.TryParse(montoTotal, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsedMonto))
                {
                    montoTotal = parsedMonto.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    montoTotal = "0.00";
                }

                if (DateTime.TryParse(fechaEmision, out var parsedFecha))
                {
                    fechaEmision = parsedFecha.ToString("dd-MM-yyyy");
                }
                else if (string.IsNullOrEmpty(fechaEmision))
                {
                    fechaEmision = DateTime.Now.ToString("dd-MM-yyyy");
                }

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
                using (var stringWriter = new Utf8StringWriter())
                using (var xmlWriter = System.Xml.XmlWriter.Create(stringWriter, settings))
                {
                    serializer.Serialize(xmlWriter, acecfResponse, emptyNamespaces);
                    acecfXml = stringWriter.ToString();
                }

                string signedAcecf = _signatureService.SignXml(acecfXml, certBytes, certPassword);

                if (signedAcecf.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
                {
                    int endDecl = signedAcecf.IndexOf("?>");
                    if (endDecl != -1)
                    {
                        signedAcecf = signedAcecf.Substring(endDecl + 2).TrimStart();
                    }
                }
                signedAcecf = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + signedAcecf;

                _logger.LogInformation("AprobacionComercialEcfAsync: Successfully generated signed ACECF for eNCF: {eNCF}, RNCEmisor: {RNCEmisor}, RNCComprador: {RNCComprador}", eNcf, rncEmisor, rncComprador);

                return Content(signedAcecf, "text/xml");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "AprobacionComercialEcfAsync: Exception occurred.");
                return StatusCode(500, ex.Message);
            }
        }

        private sealed class Utf8StringWriter : StringWriter
        {
            public override System.Text.Encoding Encoding => new System.Text.UTF8Encoding(false);
        }
    }
}
