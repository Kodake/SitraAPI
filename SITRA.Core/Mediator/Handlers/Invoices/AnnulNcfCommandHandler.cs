using System.Globalization;
using System.Text;
using System.Xml.Linq;
using AutoMapper;
using Mediator;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using SITRA.Core.DTOs.DGII;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Commands.Invoices.SaleInvoices;
using SITRA.Core.Models.Dgii;
using SITRA.Core.Interfaces.Services;
using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Interfaces.Services.Messaging;

namespace SITRA.Core.Mediator.Handlers.Invoices
{
    /// <summary>
    /// Handler for AnnulNcfCommand, providing external API execution context for the MediatR pipeline.
    /// Integrates physical disk file parsing and outbound DGII signing/transmitting dependencies which are kept out of the pure command.
    /// </summary>
    public class AnnulNcfCommandHandler : IRequestHandler<AnnulNcfCommand, AnnulNcfDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IMapper _mapper;
        private readonly IDgiiApiService _dgiiApiService;
        private readonly ISignatureService _signatureService;
        private readonly IEcfXmlGenerator _ecfXmlGenerator;
        private readonly DgiiSettings _dgiiSettings;
        private readonly IDgiiStorageService _dgiiStorageService;

        public AnnulNcfCommandHandler(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            IDgiiApiService dgiiApiService,
            ISignatureService signatureService,
            IEcfXmlGenerator ecfXmlGenerator,
            IOptions<DgiiSettings> dgiiSettings,
            IDgiiStorageService dgiiStorageService)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
            _mapper = mapper;
            _dgiiApiService = dgiiApiService;
            _signatureService = signatureService;
            _ecfXmlGenerator = ecfXmlGenerator;
            _dgiiSettings = dgiiSettings.Value;
            _dgiiStorageService = dgiiStorageService;
        }

        public async ValueTask<AnnulNcfDTO> Handle(AnnulNcfCommand request, CancellationToken cancellationToken)
        {
            var (certBytes, certPassword) = await _unitOfWork.UserRoleService.GetTenantCertificateAsync(cancellationToken);

            // Define the external delegation pipeline that interacts with infrastructure (DGII services + Disk IO)
            request.DgiiProcess = async (annulInvoice) =>
            {
                string trackId = string.Empty;
                string status = "Enviado";
                string securityCode = "000000";
                string rawResponse = string.Empty;

                try
                {
                    // ── 1. Locate and read the existing NCF artifact from storage ──
                    if (string.IsNullOrEmpty(annulInvoice.ModifiedNcfNumber) || annulInvoice.ModifiedNcfNumber.Length < 3)
                        throw new InvalidOperationException($"Reference ModifiedNcfNumber is invalid: '{annulInvoice.ModifiedNcfNumber}'");

                    string originalTipoEcf = annulInvoice.ModifiedNcfNumber.Substring(1, 2);
                    string fileName = $"{annulInvoice.ModifiedNcfNumber}.xml";

                    string? xmlContent = await _dgiiStorageService.ReadDocumentXmlAsync(_dgiiSettings.DefaultRNC, originalTipoEcf, fileName);

                    if (string.IsNullOrEmpty(xmlContent))
                        throw new FileNotFoundException($"Reference XML document not found in storage: {fileName}");

                    var origXml = XDocument.Parse(xmlContent);

                    // Parsing Helpers
                    decimal Num(string name)
                    {
                        var raw = origXml.Descendants(name).FirstOrDefault()?.Value?.Trim();
                        return !string.IsNullOrEmpty(raw) && decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0m;
                    }
                    int? NumInt(string name)
                    {
                        var raw = origXml.Descendants(name).FirstOrDefault()?.Value?.Trim();
                        return !string.IsNullOrEmpty(raw) && int.TryParse(raw, out var i) ? i : null;
                    }
                    string? Str(string name) => origXml.Descendants(name).FirstOrDefault()?.Value?.Trim();

                    // Extract and prep standard variables for XML injection
                    decimal montoTotal = Num("MontoTotal");
                    decimal gravTotal  = Num("MontoGravadoTotal");
                    decimal grav1      = Num("MontoGravadoI1");
                    decimal grav2      = Num("MontoGravadoI2");
                    decimal grav3      = Num("MontoGravadoI3");
                    decimal exento     = Num("MontoExento");
                    
                    int? itbis1 = NumInt("ITBIS1");
                    int? itbis2 = NumInt("ITBIS2");
                    int? itbis3 = NumInt("ITBIS3");

                    string? rncBuyer    = Str("RNCComprador");
                    string? idExtranjer = Str("IdentificadorExtranjero");
                    string buyerName    = Str("RazonSocialComprador") ?? "CONSUMIDOR FINAL";
                    string origIssueDt  = Str("FechaEmision") ?? DateTimeOffset.Now.ToString("dd-MM-yyyy");

                    // Map items directly copying exactly what went up previously
                    var originalItems = origXml.Descendants("Item")
                        .Select((el, i) => new Item
                        {
                            NumeroLinea = i + 1,
                            IndicadorFacturacion = int.TryParse(el.Element("IndicadorFacturacion")?.Value, out var ind) ? ind : 4,
                            NombreItem = el.Element("NombreItem")?.Value ?? "Item",
                            IndicadorBienoServicio = int.TryParse(el.Element("IndicadorBienoServicio")?.Value, out var bs) ? bs : 1,
                            Cantidad = decimal.TryParse(el.Element("CantidadItem")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var q) ? q : 1m,
                            PrecioUnitario = decimal.TryParse(el.Element("PrecioUnitarioItem")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var p) ? p : 0m,
                            MontoItem = decimal.TryParse(el.Element("MontoItem")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var m) ? m : 0m
                        }).ToList();

                    if (originalItems.Count == 0)
                    {
                        originalItems.Add(new Item
                        {
                            NumeroLinea = 1, IndicadorFacturacion = 4,
                            NombreItem = $"Reversion de Cargo {annulInvoice.ModifiedNcfNumber}",
                            IndicadorBienoServicio = 2, Cantidad = 1m,
                            PrecioUnitario = montoTotal, MontoItem = montoTotal
                        });
                    }

                    // ── 2. Construct formal E34 DGII DTO Schema ────────────────────────────
                    var ecfDocument = new EcfDocument
                    {
                        Encabezado = new Encabezado
                        {
                            IdDoc = new IdDoc
                            {
                                TipoeCF = "34",
                                ENCF = annulInvoice.FiscalSupportNumber ?? string.Empty,
                                IndicadorNotaCredito = 0,
                                IndicadorMontoGral = "0",
                                TipoIngresos = (originalTipoEcf == "41" || originalTipoEcf == "43" || originalTipoEcf == "47") ? null : "01",
                                TipoPago = "1"
                            },
                            Emisor = new Emisor
                            {
                                RNCEmisor = _dgiiSettings.DefaultRNC,
                                RazonSocial = _dgiiSettings.DefaultRazonSocial,
                                DireccionEmisor = _dgiiSettings.DefaultDireccion,
                                FechaEmision = DateTimeOffset.Now.ToString("dd-MM-yyyy")
                            },
                            Comprador = new Comprador
                            {
                                RNCComprador = !string.IsNullOrEmpty(rncBuyer) ? rncBuyer : null,
                                IdentificadorExtranjero = !string.IsNullOrEmpty(idExtranjer) ? idExtranjer : null,
                                RazonSocialComprador = buyerName
                            },
                            Totales = new Totales
                            {
                                CurrentTipoeCF = "34",
                                MontoTotal = montoTotal,
                                MontoGravadoTotal = gravTotal > 0 ? gravTotal : null,
                                MontoGravadoI1 = grav1 > 0 ? grav1 : null,
                                MontoGravadoI2 = grav2 > 0 ? grav2 : null,
                                MontoGravadoI3 = grav3 > 0 ? grav3 : null,
                                MontoExento = exento > 0 ? exento : null,
                                ITBIS1 = itbis1, ITBIS2 = itbis2, ITBIS3 = itbis3,
                                TotalITBIS = Num("TotalITBIS"),
                                TotalITBIS1 = itbis1.HasValue ? Num("TotalITBIS1") : null,
                                TotalITBIS2 = itbis2.HasValue ? Num("TotalITBIS2") : null,
                                TotalITBIS3 = itbis3.HasValue ? Num("TotalITBIS3") : null
                            }
                        },
                        Detalles = new Detalles { Items = originalItems },
                        InformacionReferencia = new InformacionReferencia
                        {
                            NCFModificado = annulInvoice.ModifiedNcfNumber ?? string.Empty,
                            FechaNCFModificado = origIssueDt,
                            CodigoModificacion = 1 // Anula el NCF modificado
                        }
                    };

                    // ── 3. Execute DGII Authentication and Submission flow ────────────────
                    string seedXml = await _dgiiApiService.GetSemillaAsync();
                    string signedSeed = _signatureService.SignAuthenticationSeed(seedXml, certBytes, certPassword);
                    string token = await _dgiiApiService.GetTokenAsync(signedSeed);

                    if (!string.IsNullOrEmpty(token))
                    {
                        string xmlSinFirma = _ecfXmlGenerator.GenerateEcfXml(ecfDocument);
                        string xmlFirmado = _signatureService.SignXml(xmlSinFirma, certBytes, certPassword);
                        securityCode = GetSecurityCodeFromSignedXml(xmlFirmado);

                        annulInvoice.XmlUrl = await _dgiiStorageService.SaveDocumentXmlAsync(xmlFirmado, _dgiiSettings.DefaultRNC, "34", $"{ecfDocument.Encabezado.IdDoc.ENCF}.xml");

                        rawResponse = await _dgiiApiService.SendEcfAsync(xmlFirmado, token);

                        try
                        {
                            var jsonDoc = System.Text.Json.JsonDocument.Parse(rawResponse);
                            if (jsonDoc.RootElement.TryGetProperty("trackId", out var tidEl))
                            {
                                trackId = tidEl.GetString() ?? string.Empty;
                                if (!string.IsNullOrEmpty(trackId) && Guid.TryParse(trackId, out _))
                                {
                                    await Task.Delay(4000); // Allow time for DGII to snapshot internal state
                                    var finalPoll = await _dgiiApiService.CheckStatusAsync(trackId, token);
                                    if (!string.IsNullOrEmpty(finalPoll))
                                    {
                                        rawResponse = finalPoll;
                                        jsonDoc = System.Text.Json.JsonDocument.Parse(finalPoll);
                                    }
                                }
                            }

                            if (jsonDoc.RootElement.TryGetProperty("estado", out var estadoEl))
                            {
                                status = estadoEl.GetString() ?? "Enviado";
                            }
                            else if (jsonDoc.RootElement.TryGetProperty("codigo", out var codeEl))
                            {
                                int c = codeEl.ValueKind == System.Text.Json.JsonValueKind.Number ? codeEl.GetInt32() : int.TryParse(codeEl.GetString(), out int pr) ? pr : -1;
                                if (c != 1 && c != 3 && c != 4 && c != 5) status = "Rechazado";
                            }
                        }
                        catch { /* treat parsed fail gracefully */ }
                    }
                }
                catch (Exception ex)
                {
                    status = "Error";
                    // Save the full stack trace into the database to pinpoint the exact crash line in production
                    rawResponse = ex.ToString();
                }

                return (trackId, status, securityCode, rawResponse);
            };

            // Delegate fully configured, execute standard command sequence orchestration
            return await request.ExecuteAction(_unitOfWork, _outputCacheStore, _mapper, cancellationToken);
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
