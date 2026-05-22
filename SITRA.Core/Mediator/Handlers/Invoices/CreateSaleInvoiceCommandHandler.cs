using System.Text;
using System.Xml.Linq;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using SITRA.Core.DTOs.DGII;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Commands.Invoices.SaleInvoices;
using SITRA.Core.Models.Dgii;
using SITRA.Core.Interfaces.Services;

namespace SITRA.Core.Mediator.Handlers.Invoices
{
    public class CreateSaleInvoiceCommandHandler : IRequestHandler<CreateSaleInvoiceCommand, CreateSaleInvoiceDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IMapper _mapper;
        private readonly IDgiiApiService _dgiiApiService;
        private readonly ISignatureService _signatureService;
        private readonly IEcfXmlGenerator _ecfXmlGenerator;
        private readonly IInvoiceToEcfMapper _mapperToEcf;
        private readonly IDgiiStorageService _dgiiStorageService;
        private readonly DgiiSettings _dgiiSettings;

        public CreateSaleInvoiceCommandHandler(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            IDgiiApiService dgiiApiService,
            ISignatureService signatureService,
            IEcfXmlGenerator ecfXmlGenerator,
            IInvoiceToEcfMapper mapperToEcf,
            IDgiiStorageService dgiiStorageService,
            IOptions<DgiiSettings> dgiiSettings)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
            _mapper = mapper;
            _dgiiApiService = dgiiApiService;
            _signatureService = signatureService;
            _ecfXmlGenerator = ecfXmlGenerator;
            _mapperToEcf = mapperToEcf;
            _dgiiStorageService = dgiiStorageService;
            _dgiiSettings = dgiiSettings.Value;
        }

        public async Task<CreateSaleInvoiceDTO> Handle(CreateSaleInvoiceCommand request, CancellationToken cancellationToken)
        {
            var (certBytes, certPassword) = await _unitOfWork.UserRoleService.GetTenantCertificateAsync(cancellationToken);

            request.DgiiProcess = async (invoice) =>
            {
                bool isAccepted = true;
                string trackId = "";
                string status = "Enviado";
                string securityCode = "000000";
                string rawResponse = "";

                try
                {
                    var ecfDocument = _mapperToEcf.MapToDgiiDocument(invoice);
                    
                    if (!string.IsNullOrEmpty(invoice.ModifiedNcfNumber))
                    {
                        string refDate = DateTimeOffset.Now.ToString("dd-MM-yyyy");
                        var candidates = await _unitOfWork.SaleInvoiceRepository.GetAllAsync(i => i.FiscalSupportNumber == invoice.ModifiedNcfNumber);
                        var originalDoc = candidates.FirstOrDefault();
                        if (originalDoc != null)
                        {
                            refDate = originalDoc.Date.ToString("dd-MM-yyyy");
                        }

                        ecfDocument.InformacionReferencia = new InformacionReferencia
                        {
                            NCFModificado = invoice.ModifiedNcfNumber,
                            CodigoModificacion = invoice.ModificationCode ?? 3,
                            FechaNCFModificado = refDate
                        };
                    }

                    string seedXml = await _dgiiApiService.GetSemillaAsync();
                    string signedSeed = _signatureService.SignAuthenticationSeed(seedXml, certBytes, certPassword);
                    string token = await _dgiiApiService.GetTokenAsync(signedSeed);

                    if (!string.IsNullOrEmpty(token))
                    {
                        string xmlFirmado = string.Empty;
                        bool useRfceChannel = false;

                        bool isType32 = ecfDocument.Encabezado?.IdDoc?.TipoeCF == "32";
                        bool isAmountLessThan250k = ecfDocument.Encabezado?.Totales?.MontoTotal < 250000;

                        if (isType32 && isAmountLessThan250k)
                        {
                            string baseEcfXml = _ecfXmlGenerator.GenerateEcfXml(ecfDocument);
                            string signedBaseEcf = _signatureService.SignXml(baseEcfXml, certBytes, certPassword);
                            
                            await _dgiiStorageService.SaveDocumentXmlAsync(signedBaseEcf, ecfDocument.Encabezado.Emisor.RNCEmisor, "32", ecfDocument.Encabezado.IdDoc.ENCF + ".xml");

                            securityCode = GetSecurityCodeFromSignedXml(signedBaseEcf);
                            var rfceDoc = MapToRfce(ecfDocument, securityCode);

                            string rfceXmlSinFirma = _ecfXmlGenerator.GenerateRfceXml(rfceDoc);
                            xmlFirmado = _signatureService.SignXml(rfceXmlSinFirma, certBytes, certPassword);

                            invoice.XmlUrl = await _dgiiStorageService.SaveDocumentXmlAsync(xmlFirmado, ecfDocument.Encabezado.Emisor.RNCEmisor, "RFCE", ecfDocument.Encabezado.IdDoc.ENCF + "_RFCE.xml");
                            useRfceChannel = true;
                        }
                        else
                        {
                            string xmlSinFirma = _ecfXmlGenerator.GenerateEcfXml(ecfDocument);
                            xmlFirmado = _signatureService.SignXml(xmlSinFirma, certBytes, certPassword);
                            securityCode = GetSecurityCodeFromSignedXml(xmlFirmado);

                            var docType = ecfDocument.Encabezado?.IdDoc?.TipoeCF ?? "UNKNOWN";
                            invoice.XmlUrl = await _dgiiStorageService.SaveDocumentXmlAsync(xmlFirmado, ecfDocument.Encabezado?.Emisor?.RNCEmisor ?? "", docType, (ecfDocument.Encabezado?.IdDoc?.ENCF ?? "") + ".xml");
                        }

                        rawResponse = useRfceChannel
                            ? await _dgiiApiService.SendRfceAsync(xmlFirmado, token)
                            : await _dgiiApiService.SendEcfAsync(xmlFirmado, token);

                        trackId = string.Empty;

                        try
                        {
                            var jsonDoc = System.Text.Json.JsonDocument.Parse(rawResponse);
                            if (jsonDoc.RootElement.TryGetProperty("trackId", out var trackIdElement))
                            {
                                trackId = trackIdElement.GetString() ?? string.Empty;

                                if (!string.IsNullOrEmpty(trackId) && Guid.TryParse(trackId, out _))
                                {
                                    try
                                    {
                                        await Task.Delay(4000); // Wait for DGII processing
                                        var statusResponse = await _dgiiApiService.CheckStatusAsync(trackId, token);
                                        if (!string.IsNullOrEmpty(statusResponse))
                                        {
                                            rawResponse = statusResponse; // Save the detailed response
                                            jsonDoc = System.Text.Json.JsonDocument.Parse(statusResponse);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Error checking trackId status: {ex.Message}");
                                    }
                                }
                            }

                            if (jsonDoc.RootElement.TryGetProperty("estado", out var estadoElement))
                            {
                                status = estadoElement.GetString() ?? "Enviado";
                                if (status.Contains("Rechazado", StringComparison.OrdinalIgnoreCase))
                                    isAccepted = false;
                            }
                            else if (jsonDoc.RootElement.TryGetProperty("codigo", out var codigoElement))
                            {
                                int codigoInt = -1;
                                if (codigoElement.ValueKind == System.Text.Json.JsonValueKind.Number)
                                    codigoInt = codigoElement.GetInt32();
                                else if (codigoElement.ValueKind == System.Text.Json.JsonValueKind.String)
                                    int.TryParse(codigoElement.GetString(), out codigoInt);

                                if (codigoInt != 1 && codigoInt != 3 && codigoInt != 4 && codigoInt != 5)
                                    isAccepted = false;
                            }
                        }
                        catch
                        {
                            // If parsing fails, we assume it went through but could not be parsed
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"DGII Flow Error: {ex.Message}");
                    isAccepted = false;
                    status = "Error";
                    rawResponse = ex.Message;
                }

                return (isAccepted, trackId, status, securityCode, rawResponse);
            };

            var dto = await request.ExecuteAction(_unitOfWork, _outputCacheStore, _mapper, cancellationToken);
            return dto;
        }

        private static void SaveDocumentXml(string xml, string encf, string rnc, string docType, string? storagePath)
        {
            try
            {
                string baseDir = !string.IsNullOrEmpty(storagePath)
                    ? storagePath
                    : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DgiiDocuments");

                string saveDir = Path.Combine(baseDir, rnc, docType);

                if (!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }

                string fileName = docType switch
                {
                    "RFCE" => $"{rnc}{encf}_RFCE.xml",
                    "ACECF" => $"{GetRncCompradorFromXml(xml)}{encf}_ACECF.xml",
                    _ => $"{rnc}{encf}.xml"
                };

                string filePath = Path.Combine(saveDir, fileName);
                File.WriteAllText(filePath, xml, new UTF8Encoding(false));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save DGII XML: {ex.Message}");
            }
        }

        private static string GetRncCompradorFromXml(string xml)
        {
            try
            {
                var docX = XDocument.Parse(xml);
                return docX.Descendants("RNCComprador").FirstOrDefault()?.Value ?? string.Empty;
            }
            catch { return string.Empty; }
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

        private static RfceDocument MapToRfce(EcfDocument doc, string securityCode)
        {
            var rfce = new RfceDocument
            {
                Encabezado = new RfceEncabezado
                {
                    CodigoSeguridadeCF = securityCode,
                    Version = doc.Encabezado!.Version,
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
    }
}
