using System.Globalization;
using System.Text;
using System.Xml.Linq;
using MediatR;
using Microsoft.Extensions.Options;
using SITRA.Core.DTOs.DGII;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Commands.Invoices.SaleInvoices;
using SITRA.Core.Models.Dgii;

namespace SITRA.Core.Mediator.Handlers.Invoices
{
    /// <summary>
    /// Handles the NCF annulment flow:
    ///   1. Loads the E34 fiscal support, increments its sequence → new eNCF.
    ///   2. Reads the original NCF XML from disk ({StoragePath}\{RNC}\{tipoEcf}\{RNC}{eNCF}.xml).
    ///   3. Mirrors Totales, Comprador, and Items exactly from the original document.
    ///   4. Injects &lt;InformacionReferencia&gt; (NCFModificado + FechaNCFModificado + CodigoModificacion=1).
    ///   5. Signs and sends to DGII.
    ///   6. Persists only the fiscal support sequence increment.
    /// </summary>
    public class AnnulNcfCommandHandler : IRequestHandler<AnnulNcfCommand, AnnulNcfDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDgiiApiService _dgiiApiService;
        private readonly ISignatureService _signatureService;
        private readonly IEcfXmlGenerator _ecfXmlGenerator;
        private readonly DgiiSettings _dgiiSettings;

        public AnnulNcfCommandHandler(
            IUnitOfWork unitOfWork,
            IDgiiApiService dgiiApiService,
            ISignatureService signatureService,
            IEcfXmlGenerator ecfXmlGenerator,
            IOptions<DgiiSettings> dgiiSettings)
        {
            _unitOfWork       = unitOfWork;
            _dgiiApiService   = dgiiApiService;
            _signatureService = signatureService;
            _ecfXmlGenerator  = ecfXmlGenerator;
            _dgiiSettings     = dgiiSettings.Value;
        }

        public async Task<AnnulNcfDTO> Handle(AnnulNcfCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            // ── 1. Load E34 fiscal support and generate the credit note eNCF ─────────
            var fiscalSupport = await _unitOfWork.FiscalSupportRepository.GetByIdAsync(dto.FiscalSupportId);

            if (fiscalSupport == null)
                throw new InvalidOperationException($"Fiscal support {dto.FiscalSupportId} not found.");

            if (fiscalSupport.FiscalSupportType != EcfType.E34)
                throw new InvalidOperationException("The provided fiscal support must be of type E34 (Nota de Crédito Electrónica).");

            fiscalSupport.IncrementSequence(); // throws if sequence exhausted
            string creditNoteNcf = fiscalSupport.GenerateFiscalSupportNumber();
            dto.CreditNoteNcf = creditNoteNcf;

            // Persist sequence increment immediately so it's reserved even if DGII fails
            _unitOfWork.FiscalSupportRepository.Update(fiscalSupport);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // ── 2. Locate and parse the original NCF XML from disk ───────────────────
            // Convention: {StoragePath}\{DefaultRNC}\{tipoEcf}\{DefaultRNC}{eNCF}.xml
            // tipoEcf extracted from NCF string: "E460000000604" → "46"
            if (dto.ModifiedNcfNumber.Length < 3)
                throw new InvalidOperationException($"ModifiedNcfNumber format invalid: '{dto.ModifiedNcfNumber}'");

            string originalTipoEcf = dto.ModifiedNcfNumber.Substring(1, 2);
            string xmlFilePath = Path.Combine(
                _dgiiSettings.StoragePath,
                _dgiiSettings.DefaultRNC,
                originalTipoEcf,
                $"{_dgiiSettings.DefaultRNC}{dto.ModifiedNcfNumber}.xml");

            if (!File.Exists(xmlFilePath))
                throw new FileNotFoundException(
                    $"Original NCF XML not found at: {xmlFilePath}. " +
                    "Ensure the original invoice was transmitted through this system.");

            var origXml = XDocument.Load(xmlFilePath);

            // ── Helper lambdas ────────────────────────────────────────────────────────
            decimal Num(string name)
            {
                var raw = origXml.Descendants(name).FirstOrDefault()?.Value?.Trim();
                return !string.IsNullOrEmpty(raw) &&
                       decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0m;
            }
            int? NumInt(string name)
            {
                var raw = origXml.Descendants(name).FirstOrDefault()?.Value?.Trim();
                return !string.IsNullOrEmpty(raw) && int.TryParse(raw, out var i) ? i : null;
            }
            string? Str(string name) => origXml.Descendants(name).FirstOrDefault()?.Value?.Trim();

            // ── 3. Extract Totales ────────────────────────────────────────────────────
            decimal montoTotal        = Num("MontoTotal");
            decimal montoGravadoTotal = Num("MontoGravadoTotal");
            decimal montoGravI1       = Num("MontoGravadoI1");
            decimal montoGravI2       = Num("MontoGravadoI2");
            decimal montoGravI3       = Num("MontoGravadoI3");
            decimal montoExento       = Num("MontoExento");
            decimal totalITBIS        = Num("TotalITBIS");
            decimal totalITBIS1       = Num("TotalITBIS1");
            decimal totalITBIS2       = Num("TotalITBIS2");
            decimal totalITBIS3       = Num("TotalITBIS3");
            int? itbis1Rate           = NumInt("ITBIS1");
            int? itbis2Rate           = NumInt("ITBIS2");
            int? itbis3Rate           = NumInt("ITBIS3");

            // ── 4. Extract Comprador ──────────────────────────────────────────────────
            string? rncComprador      = Str("RNCComprador");
            string? identExtranjero   = Str("IdentificadorExtranjero");
            string  razonComprador    = Str("RazonSocialComprador") ?? "CONSUMIDOR FINAL";
            string  fechaEmisionOrig  = Str("FechaEmision") ?? DateTimeOffset.Now.ToString("dd-MM-yyyy"); // rule 634: must match original

            // ── 5. Mirror Items exactly from the original XML ─────────────────────────
            var originalItems = origXml.Descendants("Item")
                .Select((el, i) => new Item
                {
                    NumeroLinea = i + 1,
                    IndicadorFacturacion = int.TryParse(el.Element("IndicadorFacturacion")?.Value, out var ind) ? ind : 4,
                    NombreItem = el.Element("NombreItem")?.Value ?? "Item",
                    IndicadorBienoServicio = int.TryParse(el.Element("IndicadorBienoServicio")?.Value, out var bs) ? bs : 1,
                    Cantidad = decimal.TryParse(el.Element("CantidadItem")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var qty) ? qty : 1m,
                    PrecioUnitario = decimal.TryParse(el.Element("PrecioUnitarioItem")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var pu) ? pu : 0m,
                    MontoItem = decimal.TryParse(el.Element("MontoItem")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var mi) ? mi : 0m,
                })
                .ToList();

            // Fallback item if the original had none (shouldn't happen)
            if (originalItems.Count == 0)
                originalItems.Add(new Item
                {
                    NumeroLinea = 1, IndicadorFacturacion = 4,
                    NombreItem = $"Anulación {dto.ModifiedNcfNumber}",
                    IndicadorBienoServicio = 2, Cantidad = 1m,
                    PrecioUnitario = montoTotal, MontoItem = montoTotal,
                });

            // ── 6. Build the E34 ECF document mirroring the original ──────────────────
            var ecfDocument = new EcfDocument
            {
                Encabezado = new Encabezado
                {
                    IdDoc = new IdDoc
                    {
                        TipoeCF              = "34",
                        ENCF                 = creditNoteNcf,
                        IndicadorNotaCredito = 0,   // 0 = NCF modified within 30 days
                        IndicadorMontoGral   = "0", // 0 = item prices exclude ITBIS (required by rule 176 when MontoGravadoTotal is present)
                        TipoIngresos         = "01",
                        TipoPago             = "1",
                    },
                    Emisor = new Emisor
                    {
                        RNCEmisor        = _dgiiSettings.DefaultRNC,
                        RazonSocial      = _dgiiSettings.DefaultRazonSocial,
                        DireccionEmisor  = _dgiiSettings.DefaultDireccion,
                        FechaEmision     = DateTimeOffset.Now.ToString("dd-MM-yyyy"),
                    },
                    Comprador = new Comprador
                    {
                        RNCComprador             = !string.IsNullOrEmpty(rncComprador) ? rncComprador : null,
                        IdentificadorExtranjero  = !string.IsNullOrEmpty(identExtranjero) ? identExtranjero : null,
                        RazonSocialComprador     = razonComprador,
                    },
                    Totales = new Totales
                    {
                        CurrentTipoeCF    = "34",
                        MontoGravadoTotal = montoGravadoTotal > 0 ? montoGravadoTotal : null,
                        MontoGravadoI1    = montoGravI1 > 0 ? montoGravI1 : null,
                        MontoGravadoI2    = montoGravI2 > 0 ? montoGravI2 : null,
                        MontoGravadoI3    = montoGravI3 > 0 ? montoGravI3 : null,
                        MontoExento       = montoExento > 0 ? montoExento : null,
                        ITBIS1            = itbis1Rate,
                        ITBIS2            = itbis2Rate,
                        ITBIS3            = itbis3Rate,
                        TotalITBIS        = totalITBIS,
                        // Rule 11031: TotalITBISN must be present whenever ITBISN rate is present, even at 0.00
                        TotalITBIS1       = itbis1Rate.HasValue ? totalITBIS1 : null,
                        TotalITBIS2       = itbis2Rate.HasValue ? totalITBIS2 : null,
                        TotalITBIS3       = itbis3Rate.HasValue ? totalITBIS3 : null,
                        MontoTotal        = montoTotal,
                    },
                },
                Detalles = new Detalles { Items = originalItems },
            };

            // ── 7. Inject InformacionReferencia AFTER DetallesItems ───────────────────
            // XSD sequence: NCFModificado → RNCOtroContribuyente? → FechaNCFModificado → CodigoModificacion
            ecfDocument.InformacionReferencia = new InformacionReferencia
            {
                NCFModificado      = dto.ModifiedNcfNumber,
                FechaNCFModificado = fechaEmisionOrig,  // DGII rule 634: must equal original invoice FechaEmision
                CodigoModificacion = 1, // Anula el NCF modificado
            };

            // ── 5. Sign and send to DGII ──────────────────────────────────────────────
            bool isAccepted   = true;
            string trackId    = string.Empty;
            string status     = "Enviado";
            string securityCode = "000000";
            string rawResponse  = string.Empty;

            try
            {
                string seedXml    = await _dgiiApiService.GetSemillaAsync();
                string signedSeed = _signatureService.SignAuthenticationSeed(seedXml);
                string token      = await _dgiiApiService.GetTokenAsync(signedSeed);

                if (!string.IsNullOrEmpty(token))
                {
                    string xmlSinFirma = _ecfXmlGenerator.GenerateEcfXml(ecfDocument);
                    string xmlFirmado  = _signatureService.SignXml(xmlSinFirma);

                    securityCode = GetSecurityCodeFromSignedXml(xmlFirmado);

                    SaveDocumentXml(xmlFirmado, creditNoteNcf, ecfDocument.Encabezado?.Emisor?.RNCEmisor ?? "", "34", _dgiiSettings.StoragePath);

                    rawResponse = await _dgiiApiService.SendEcfAsync(xmlFirmado, token);

                    try
                    {
                        var jsonDoc = System.Text.Json.JsonDocument.Parse(rawResponse);

                        if (jsonDoc.RootElement.TryGetProperty("trackId", out var tidEl))
                        {
                            trackId = tidEl.GetString() ?? string.Empty;

                            if (!string.IsNullOrEmpty(trackId) && Guid.TryParse(trackId, out _))
                            {
                                await Task.Delay(4000, cancellationToken);
                                var statusResponse = await _dgiiApiService.CheckStatusAsync(trackId, token);
                                if (!string.IsNullOrEmpty(statusResponse))
                                {
                                    rawResponse = statusResponse;
                                    jsonDoc = System.Text.Json.JsonDocument.Parse(statusResponse);
                                }
                            }
                        }

                        if (jsonDoc.RootElement.TryGetProperty("estado", out var estadoEl))
                        {
                            status = estadoEl.GetString() ?? "Enviado";
                            if (status.Contains("Rechazado", StringComparison.OrdinalIgnoreCase))
                                isAccepted = false;
                        }
                        else if (jsonDoc.RootElement.TryGetProperty("codigo", out var codigoEl))
                        {
                            int codigoInt = codigoEl.ValueKind == System.Text.Json.JsonValueKind.Number
                                ? codigoEl.GetInt32()
                                : int.TryParse(codigoEl.GetString(), out int c) ? c : -1;

                            if (codigoInt != 1 && codigoInt != 3 && codigoInt != 4 && codigoInt != 5)
                                isAccepted = false;
                        }
                    }
                    catch { /* Parsing failure — treat as sent but unverified */ }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DGII Annul Flow Error: {ex.Message}");
                isAccepted  = false;
                status      = "Error";
                rawResponse = ex.Message;
            }

            dto.DgiiStatus   = status;
            dto.DgiiResponse = rawResponse;

            return dto;
        }

        // ── Helpers ────────────────────────────────────────────────────────────────

        private static void SaveDocumentXml(string xml, string encf, string rnc, string docType, string? storagePath)
        {
            try
            {
                string baseDir = !string.IsNullOrEmpty(storagePath)
                    ? storagePath
                    : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DgiiDocuments");

                string saveDir = Path.Combine(baseDir, rnc, docType);
                if (!Directory.Exists(saveDir)) Directory.CreateDirectory(saveDir);

                File.WriteAllText(Path.Combine(saveDir, $"{rnc}{encf}.xml"), xml, new UTF8Encoding(false));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save annulment XML: {ex.Message}");
            }
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
                    sigValue = sigValue.Replace("\n", "").Replace("\r", "").Replace(" ", "");
                    return sigValue.Length >= 6 ? sigValue.Substring(0, 6) : sigValue;
                }
            }
            catch { }
            return "000000";
        }
    }
}
