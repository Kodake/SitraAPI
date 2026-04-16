using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace SITRA.Core.DTOs.DGII
{
    [XmlRoot(ElementName = "ECF")]
    public class EcfDocument
    {
        [XmlIgnore]
        public string TestCaseName { get; set; } = string.Empty;

        [XmlIgnore]
        public bool IsRfce { get; set; }

        [XmlIgnore]
        public string? CodigoSeguridad { get; set; }
        [XmlElement(ElementName = "Encabezado", Order = 1)]
        public Encabezado Encabezado { get; set; } = new();

        [XmlElement(ElementName = "DetallesItems", Order = 2)]
        public Detalles Detalles { get; set; } = new();

        [XmlElement(ElementName = "Subtotales", Order = 3)]
        public Subtotales? Subtotales { get; set; }

        [XmlElement(ElementName = "DescuentosORecargos", Order = 4)]
        public DescuentosORecargos? DescuentosORecargos { get; set; }

        [XmlElement(ElementName = "InformacionReferencia", Order = 5)]
        public InformacionReferencia? InformacionReferencia { get; set; }

        [XmlElement(ElementName = "FechaHoraFirma", Order = 6)]
        public string FechaHoraFirma { get; set; } = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

        [XmlAnyElement(Order = 7)]
        public XmlElement? Signature { get; set; }

        public bool ShouldSerializeDescuentosORecargos() => Encabezado.IdDoc.TipoeCF != "43" && DescuentosORecargos != null;
    }

    public class Encabezado
    {
        [XmlElement(ElementName = "Version", Order = 1)]
        public string Version { get; set; } = "1.0";

        [XmlElement(ElementName = "IdDoc", Order = 2)]
        public IdDoc IdDoc { get; set; } = new();

        [XmlElement(ElementName = "Emisor", Order = 3)]
        public Emisor Emisor { get; set; } = new();

        [XmlElement(ElementName = "Comprador", Order = 4)]
        public Comprador Comprador { get; set; } = new();

        [XmlElement(ElementName = "Totales", Order = 7)]
        public Totales Totales { get; set; } = new();

        [XmlElement(ElementName = "InformacionesAdicionales", Order = 5)]
        public InformacionesAdicionales? InformacionesAdicionales { get; set; }
        public bool ShouldSerializeInformacionesAdicionales() => InformacionesAdicionales != null;

        [XmlElement(ElementName = "Transporte", Order = 6)]
        public Transporte? Transporte { get; set; }
        public bool ShouldSerializeTransporte() => Transporte != null;

        public bool ShouldSerializeComprador() => IdDoc.TipoeCF != "43";
    }

    public class IdDoc
    {
        [XmlElement(ElementName = "TipoeCF", Order = 1)]
        public string TipoeCF { get; set; } = string.Empty;

        [XmlElement(ElementName = "eNCF", Order = 2)]
        public string ENCF { get; set; } = string.Empty;

        [XmlElement(ElementName = "FechaVencimientoSecuencia", Order = 3)]
        public string FechaVencimientoSecuencia { get; set; } = string.Empty;
        public bool ShouldSerializeFechaVencimientoSecuencia() => TipoeCF != "34";

        [XmlElement(ElementName = "IndicadorNotaCredito", Order = 4)]
        public int? IndicadorNotaCredito { get; set; }
        [XmlIgnore]
        public bool IndicadorNotaCreditoSpecified => IndicadorNotaCredito.HasValue;
        public bool ShouldSerializeIndicadorNotaCredito() => TipoeCF == "34";

        [XmlElement(ElementName = "IndicadorEnvioDiferido", Order = 5)]
        public int? IndicadorEnvioDiferido { get; set; }
        [XmlIgnore]
        public bool IndicadorEnvioDiferidoSpecified => IndicadorEnvioDiferido.HasValue;
        public bool ShouldSerializeIndicadorEnvioDiferido() => TipoeCF != "43";

        [XmlElement(ElementName = "IndicadorMontoGravado", Order = 6)]
        public string? IndicadorMontoGral { get; set; }
        public bool ShouldSerializeIndicadorMontoGral() => TipoeCF != "43";

        [XmlElement(ElementName = "IndicadorServicioTodoIncluido", Order = 7)]
        public int? IndicadorServicioTodoIncluido { get; set; }
        [XmlIgnore]
        public bool IndicadorServicioTodoIncluidoSpecified => IndicadorServicioTodoIncluido.HasValue;
        public bool ShouldSerializeIndicadorServicioTodoIncluido() => TipoeCF != "43";

        [XmlElement(ElementName = "TipoIngresos", Order = 8)]
        public string? TipoIngresos { get; set; }
        public bool ShouldSerializeTipoIngresos() => TipoeCF != "41" && TipoeCF != "43" && TipoeCF != "47";

        [XmlElement(ElementName = "TipoPago", Order = 9)]
        public string? TipoPago { get; set; }
        public bool ShouldSerializeTipoPago() => !string.IsNullOrEmpty(TipoPago);

        [XmlElement(ElementName = "FechaLimitePago", Order = 10)]
        public string? FechaLimitePago { get; set; }
        public bool ShouldSerializeFechaLimitePago() => TipoeCF != "43";

        [XmlElement(ElementName = "TerminoPago", Order = 11)]
        public string? TerminoPago { get; set; }
        public bool ShouldSerializeTerminoPago() => TipoeCF != "43";

        [XmlArray(ElementName = "TablaFormasPago", Order = 12)]
        [XmlArrayItem(ElementName = "FormaDePago")]
        public List<FormaDePago>? TablaFormasPago { get; set; }
        public bool ShouldSerializeTablaFormasPago() => TipoeCF != "43" && TablaFormasPago != null;

        [XmlElement(ElementName = "TipoCuentaPago", Order = 13)]
        public string? TipoCuentaPago { get; set; }
        public bool ShouldSerializeTipoCuentaPago() => TipoeCF != "43";

        [XmlElement(ElementName = "NumeroCuentaPago", Order = 14)]
        public string? NumeroCuentaPago { get; set; }
        public bool ShouldSerializeNumeroCuentaPago() => TipoeCF != "43";

        [XmlElement(ElementName = "BancoPago", Order = 15)]
        public string? BancoPago { get; set; }
        public bool ShouldSerializeBancoPago() => TipoeCF != "43";

        [XmlElement(ElementName = "FechaDesde", Order = 16)]
        public string? FechaDesde { get; set; }
        public bool ShouldSerializeFechaDesde() => TipoeCF != "43";

        [XmlElement(ElementName = "FechaHasta", Order = 17)]
        public string? FechaHasta { get; set; }
        public bool ShouldSerializeFechaHasta() => TipoeCF != "43";

        [XmlElement(ElementName = "TotalPaginas", Order = 18)]
        public int? TotalPaginas { get; set; }
        [XmlIgnore]
        public bool TotalPaginasSpecified => TotalPaginas.HasValue;
    }

    public class FormaDePago
    {
        [XmlElement(ElementName = "FormaPago", Order = 1)]
        public string FormaPago { get; set; } = string.Empty;

        [XmlElement(ElementName = "MontoPago", Order = 2)]
        public decimal MontoPago { get; set; }
    }

    public class InformacionReferencia
    {
        [XmlElement(ElementName = "NCFModificado", Order = 1)]
        public string? NCFModificado { get; set; }

        [XmlElement(ElementName = "RNCOtroContribuyente", Order = 2)]
        public string? RNCOtroContribuyente { get; set; }

        [XmlElement(ElementName = "FechaNCFModificado", Order = 3)]
        public string? FechaNCFModificado { get; set; }

        [XmlElement(ElementName = "CodigoModificacion", Order = 4)]
        public int? CodigoModificacion { get; set; }
        [XmlIgnore]
        public bool CodigoModificacionSpecified => CodigoModificacion.HasValue;

        [XmlElement(ElementName = "RazonModificacion", Order = 5)]
        public string? RazonModificacion { get; set; }
    }

    public class Emisor
    {
        [XmlElement(ElementName = "RNCEmisor", Order = 1)]
        public string RNCEmisor { get; set; } = string.Empty;

        [XmlElement(ElementName = "RazonSocialEmisor", Order = 2)]
        public string RazonSocial { get; set; } = string.Empty;

        [XmlElement(ElementName = "NombreComercial", Order = 3)]
        public string? NombreComercial { get; set; }

        [XmlElement(ElementName = "Sucursal", Order = 4)]
        public string? Sucursal { get; set; }

        [XmlElement(ElementName = "DireccionEmisor", Order = 5)]
        public string DireccionEmisor { get; set; } = string.Empty;

        [XmlElement(ElementName = "Municipio", Order = 6)]
        public string? Municipio { get; set; }

        [XmlElement(ElementName = "Provincia", Order = 7)]
        public string? Provincia { get; set; }

        [XmlArray(ElementName = "TablaTelefonoEmisor", Order = 8)]
        [XmlArrayItem(ElementName = "TelefonoEmisor")]
        public List<string>? TablaTelefonoEmisor { get; set; }

        [XmlElement(ElementName = "CorreoEmisor", Order = 9)]
        public string? CorreoEmisor { get; set; }

        [XmlElement(ElementName = "WebSite", Order = 10)]
        public string? WebSite { get; set; }

        [XmlElement(ElementName = "ActividadEconomica", Order = 11)]
        public string? ActividadEconomica { get; set; }

        [XmlElement(ElementName = "CodigoVendedor", Order = 12)]
        public string? CodigoVendedor { get; set; }

        [XmlElement(ElementName = "NumeroFacturaInterna", Order = 13)]
        public string? NumeroFacturaInterna { get; set; }

        [XmlElement(ElementName = "NumeroPedidoInterno", Order = 14)]
        public string? NumeroPedidoInterno { get; set; }

        [XmlElement(ElementName = "ZonaVenta", Order = 15)]
        public string? ZonaVenta { get; set; }

        [XmlElement(ElementName = "RutaVenta", Order = 16)]
        public string? RutaVenta { get; set; }

        [XmlElement(ElementName = "InformacionAdicionalEmisor", Order = 17)]
        public string? InformacionAdicionalEmisor { get; set; }

        [XmlElement(ElementName = "FechaEmision", Order = 18)]
        public string FechaEmision { get; set; } = string.Empty;
    }

    public class Comprador
    {
        [XmlElement(ElementName = "RNCComprador", Order = 1)]
        public string? RNCComprador { get; set; }
        public bool ShouldSerializeRNCComprador() => !string.IsNullOrEmpty(RNCComprador);

        [XmlElement(ElementName = "IdentificadorExtranjero", Order = 2)]
        public string? IdentificadorExtranjero { get; set; }

        [XmlElement(ElementName = "RazonSocialComprador", Order = 3)]
        public string RazonSocialComprador { get; set; } = string.Empty;

        [XmlElement(ElementName = "ContactoComprador", Order = 4)]
        public string? ContactoComprador { get; set; }

        [XmlElement(ElementName = "CorreoComprador", Order = 5)]
        public string? CorreoComprador { get; set; }

        [XmlElement(ElementName = "DireccionComprador", Order = 6)]
        public string? DireccionComprador { get; set; }

        [XmlElement(ElementName = "MunicipioComprador", Order = 7)]
        public string? MunicipioComprador { get; set; }

        [XmlElement(ElementName = "ProvinciaComprador", Order = 8)]
        public string? ProvinciaComprador { get; set; }

        [XmlElement(ElementName = "PaisComprador", Order = 9)]
        public string? PaisComprador { get; set; }

        [XmlElement(ElementName = "FechaEntrega", Order = 10)]
        public string? FechaEntrega { get; set; }

        [XmlElement(ElementName = "ContactoEntrega", Order = 11)]
        public string? ContactoEntrega { get; set; }

        [XmlElement(ElementName = "DireccionEntrega", Order = 12)]
        public string? DireccionEntrega { get; set; }

        [XmlElement(ElementName = "TelefonoAdicional", Order = 13)]
        public string? TelefonoAdicional { get; set; }

        [XmlElement(ElementName = "FechaOrdenCompra", Order = 14)]
        public string? FechaOrdenCompra { get; set; }

        [XmlElement(ElementName = "NumeroOrdenCompra", Order = 15)]
        public string? NumeroOrdenCompra { get; set; }

        [XmlElement(ElementName = "CodigoInternoComprador", Order = 16)]
        public string? CodigoInternoComprador { get; set; }

        [XmlElement(ElementName = "ResponsablePago", Order = 17)]
        public string? ResponsablePago { get; set; }

        [XmlElement(ElementName = "InformacionAdicionalComprador", Order = 18)]
        public string? InformacionAdicionalComprador { get; set; }
    }

    public class InformacionesAdicionales
    {
        [XmlElement(ElementName = "FechaEmbarque", Order = 1)]
        public string? FechaEmbarque { get; set; }

        [XmlElement(ElementName = "NumeroEmbarque", Order = 2)]
        public string? NumeroEmbarque { get; set; }

        [XmlElement(ElementName = "NumeroContenedor", Order = 3)]
        public string? NumeroContenedor { get; set; }

        [XmlElement(ElementName = "NumeroReferencia", Order = 4)]
        public string? NumeroReferencia { get; set; }

        [XmlElement(ElementName = "NombrePuertoEmbarque", Order = 5)]
        public string? NombrePuertoEmbarque { get; set; }

        [XmlElement(ElementName = "CondicionesEntrega", Order = 6)]
        public string? CondicionesEntrega { get; set; }

        [XmlElement(ElementName = "TotalFob", Order = 7)]
        public decimal? TotalFob { get; set; }
        [XmlIgnore]
        public bool TotalFobSpecified => TotalFob.HasValue;

        [XmlElement(ElementName = "Seguro", Order = 8)]
        public decimal? Seguro { get; set; }
        [XmlIgnore]
        public bool SeguroSpecified => Seguro.HasValue;

        [XmlElement(ElementName = "Flete", Order = 9)]
        public decimal? Flete { get; set; }
        [XmlIgnore]
        public bool FleteSpecified => Flete.HasValue;

        [XmlElement(ElementName = "OtrosGastos", Order = 10)]
        public decimal? OtrosGastos { get; set; }
        [XmlIgnore]
        public bool OtrosGastosSpecified => OtrosGastos.HasValue;

        [XmlElement(ElementName = "TotalCif", Order = 11)]
        public decimal? TotalCif { get; set; }
        [XmlIgnore]
        public bool TotalCifSpecified => TotalCif.HasValue;

        [XmlElement(ElementName = "RegimenAduanero", Order = 12)]
        public string? RegimenAduanero { get; set; }

        [XmlElement(ElementName = "NombrePuertoSalida", Order = 13)]
        public string? NombrePuertoSalida { get; set; }

        [XmlElement(ElementName = "NombrePuertoDesembarque", Order = 14)]
        public string? NombrePuertoDesembarque { get; set; }

        [XmlElement(ElementName = "PesoBruto", Order = 15)]
        public decimal? PesoBruto { get; set; }
        [XmlIgnore]
        public bool PesoBrutoSpecified => PesoBruto.HasValue;

        [XmlElement(ElementName = "PesoNeto", Order = 16)]
        public decimal? PesoNeto { get; set; }
        [XmlIgnore]
        public bool PesoNetoSpecified => PesoNeto.HasValue;

        [XmlElement(ElementName = "UnidadPesoBruto", Order = 17)]
        public string? UnidadPesoBruto { get; set; }

        [XmlElement(ElementName = "UnidadPesoNeto", Order = 18)]
        public string? UnidadPesoNeto { get; set; }

        [XmlElement(ElementName = "CantidadBulto", Order = 19)]
        public decimal? CantidadBulto { get; set; }
        [XmlIgnore]
        public bool CantidadBultoSpecified => CantidadBulto.HasValue;

        [XmlElement(ElementName = "UnidadBulto", Order = 20)]
        public string? UnidadBulto { get; set; }

        [XmlElement(ElementName = "VolumenBulto", Order = 21)]
        public decimal? VolumenBulto { get; set; }
        [XmlIgnore]
        public bool VolumenBultoSpecified => VolumenBulto.HasValue;

        [XmlElement(ElementName = "UnidadVolumen", Order = 22)]
        public string? UnidadVolumen { get; set; }
    }

    public class Transporte
    {
        [XmlElement(ElementName = "ViaTransporte", Order = 1)]
        public string? ViaTransporte { get; set; }

        [XmlElement(ElementName = "PaisOrigen", Order = 2)]
        public string? PaisOrigen { get; set; }

        [XmlElement(ElementName = "DireccionDestino", Order = 3)]
        public string? DireccionDestino { get; set; }

        [XmlElement(ElementName = "PaisDestino", Order = 4)]
        public string? PaisDestino { get; set; }

        [XmlElement(ElementName = "RNCIdentificacionCompaniaTransportista", Order = 5)]
        public string? RNCIdentificacionCompaniaTransportista { get; set; }

        [XmlElement(ElementName = "NombreCompaniaTransportista", Order = 6)]
        public string? NombreCompaniaTransportista { get; set; }

        [XmlElement(ElementName = "NumeroViaje", Order = 7)]
        public string? NumeroViaje { get; set; }

        [XmlElement(ElementName = "Conductor", Order = 8)]
        public string? Conductor { get; set; }

        [XmlElement(ElementName = "DocumentoTransporte", Order = 9)]
        public string? DocumentoTransporte { get; set; }

        [XmlElement(ElementName = "Ficha", Order = 10)]
        public string? Ficha { get; set; }

        [XmlElement(ElementName = "Placa", Order = 11)]
        public string? Placa { get; set; }

        [XmlElement(ElementName = "RutaTransporte", Order = 12)]
        public string? RutaTransporte { get; set; }

        [XmlElement(ElementName = "ZonaTransporte", Order = 13)]
        public string? ZonaTransporte { get; set; }

        [XmlElement(ElementName = "NumeroAlbaran", Order = 14)]
        public string? NumeroAlbaran { get; set; }
    }

    public class Totales
    {
        [XmlElement(ElementName = "MontoGravadoTotal", Order = 1)]
        public decimal? MontoGravadoTotal { get; set; }
        [XmlIgnore]
        public bool MontoGravadoTotalSpecified => MontoGravadoTotal.HasValue;

        [XmlElement(ElementName = "MontoGravadoI1", Order = 2)]
        public decimal? MontoGravadoI1 { get; set; }
        [XmlIgnore]
        public bool MontoGravadoI1Specified => MontoGravadoI1.HasValue;

        [XmlElement(ElementName = "MontoGravadoI2", Order = 3)]
        public decimal? MontoGravadoI2 { get; set; }
        [XmlIgnore]
        public bool MontoGravadoI2Specified => MontoGravadoI2.HasValue;

        [XmlElement(ElementName = "MontoGravadoI3", Order = 4)]
        public decimal? MontoGravadoI3 { get; set; }
        [XmlIgnore]
        public bool MontoGravadoI3Specified => MontoGravadoI3.HasValue;

        [XmlElement(ElementName = "MontoExento", Order = 5)]
        public decimal? MontoExento { get; set; }
        [XmlIgnore]
        public bool MontoExentoSpecified => MontoExento.HasValue;

        [XmlElement(ElementName = "ITBIS1", Order = 6)]
        public int? ITBIS1 { get; set; }
        [XmlIgnore]
        public bool ITBIS1Specified => ITBIS1.HasValue;

        [XmlElement(ElementName = "ITBIS2", Order = 7)]
        public int? ITBIS2 { get; set; }
        [XmlIgnore]
        public bool ITBIS2Specified => ITBIS2.HasValue;

        [XmlElement(ElementName = "ITBIS3", Order = 8)]
        public int? ITBIS3 { get; set; }
        [XmlIgnore]
        public bool ITBIS3Specified => ITBIS3.HasValue;

        [XmlElement(ElementName = "TotalITBIS", Order = 9)]
        public decimal? TotalITBIS { get; set; }
        [XmlIgnore]
        public bool TotalITBISSpecified => TotalITBIS.HasValue;

        [XmlElement(ElementName = "TotalITBIS1", Order = 10)]
        public decimal? TotalITBIS1 { get; set; }
        [XmlIgnore]
        public bool TotalITBIS1Specified => TotalITBIS1.HasValue;

        [XmlElement(ElementName = "TotalITBIS2", Order = 11)]
        public decimal? TotalITBIS2 { get; set; }
        [XmlIgnore]
        public bool TotalITBIS2Specified => TotalITBIS2.HasValue;

        [XmlElement(ElementName = "TotalITBIS3", Order = 12)]
        public decimal? TotalITBIS3 { get; set; }
        [XmlIgnore]
        public bool TotalITBIS3Specified => TotalITBIS3.HasValue;

        [XmlElement(ElementName = "MontoImpuestoAdicional", Order = 13)]
        public decimal? MontoImpuestoAdicional { get; set; }
        [XmlIgnore]
        public bool MontoImpuestoAdicionalSpecified => MontoImpuestoAdicional.HasValue;

        [XmlElement(ElementName = "MontoTotal", Order = 15)]
        public decimal? MontoTotal { get; set; }
        [XmlIgnore]
        public bool MontoTotalSpecified => MontoTotal.HasValue;

        [XmlElement(ElementName = "MontoNoFacturable", Order = 16)]
        public decimal? MontoNoFacturable { get; set; }
        [XmlIgnore]
        public bool MontoNoFacturableSpecified => MontoNoFacturable.HasValue;

        [XmlElement(ElementName = "MontoPeriodo", Order = 17)]
        public decimal? MontoPeriodo { get; set; }
        [XmlIgnore]
        public bool MontoPeriodoSpecified => MontoPeriodo.HasValue;

        [XmlElement(ElementName = "SaldoAnterior", Order = 18)]
        public decimal? SaldoAnterior { get; set; }
        [XmlIgnore]
        public bool SaldoAnteriorSpecified => SaldoAnterior.HasValue;

        [XmlElement(ElementName = "MontoAvancePago", Order = 19)]
        public decimal? MontoAvancePago { get; set; }
        [XmlIgnore]
        public bool MontoAvancePagoSpecified => MontoAvancePago.HasValue;

        [XmlElement(ElementName = "ValorPagar", Order = 20)]
        public decimal? ValorPagar { get; set; }
        [XmlIgnore]
        public bool ValorPagarSpecified => ValorPagar.HasValue;

        [XmlElement(ElementName = "TotalITBISRetenido", Order = 21)]
        public decimal? TotalITBISRetenido { get; set; }
        [XmlIgnore]
        public bool TotalITBISRetenidoSpecified => TotalITBISRetenido.HasValue;

        [XmlElement(ElementName = "TotalISRRetencion", Order = 22)]
        public decimal? TotalISRRetencion { get; set; }
        [XmlIgnore]
        public bool TotalISRRetencionSpecified => TotalISRRetencion.HasValue;

        [XmlElement(ElementName = "TotalITBISPercepcion", Order = 23)]
        public decimal? TotalITBISPercepcion { get; set; }
        [XmlIgnore]
        public bool TotalITBISPercepcionSpecified => TotalITBISPercepcion.HasValue;

        [XmlElement(ElementName = "TotalISRPercepcion", Order = 24)]
        public decimal? TotalISRPercepcion { get; set; }
        [XmlIgnore]
        public bool TotalISRPercepcionSpecified => TotalISRPercepcion.HasValue;
    }

    public class Detalles
    {
        [XmlElement(ElementName = "Item", Order = 1)]
        public List<Item> Items { get; set; } = new();
    }

    public class Item
    {
        [XmlElement(ElementName = "NumeroLinea", Order = 1)]
        public int NumeroLinea { get; set; }

        [XmlArray(ElementName = "TablaCodigosItem", Order = 2)]
        [XmlArrayItem(ElementName = "CodigosItem")]
        public List<CodigoItem>? TablaCodigosItem { get; set; }

        [XmlElement(ElementName = "IndicadorFacturacion", Order = 3)]
        public int IndicadorFacturacion { get; set; } = 1;

        [XmlElement(ElementName = "Retencion", Order = 4)]
        public Retencion? Retencion { get; set; }
        public bool ShouldSerializeRetencion() => Retencion != null; // Item class doesn't have IdDoc reference easily, but if it's null it's omitted. We'll ensure it's null in mapper for 43.

        [XmlElement(ElementName = "NombreItem", Order = 5)]
        public string NombreItem { get; set; } = string.Empty;

        [XmlElement(ElementName = "IndicadorBienoServicio", Order = 6)]
        public int IndicadorBienoServicio { get; set; } = 1;

        [XmlElement(ElementName = "DescripcionItem", Order = 7)]
        public string? DescripcionItem { get; set; }

        [XmlElement(ElementName = "CantidadItem", Order = 8)]
        public decimal Cantidad { get; set; }

        [XmlElement(ElementName = "UnidadMedida", Order = 9)]
        public string? UnidadMedida { get; set; }

        [XmlElement(ElementName = "FechaElaboracion", Order = 10)]
        public string? FechaElaboracion { get; set; }
        public bool ShouldSerializeFechaElaboracion() => !string.IsNullOrEmpty(FechaElaboracion); // Not in E43 but if empty is okay.

        [XmlElement(ElementName = "FechaVencimientoItem", Order = 11)]
        public string? FechaVencimientoItem { get; set; }
        public bool ShouldSerializeFechaVencimientoItem() => !string.IsNullOrEmpty(FechaVencimientoItem); // Not in E43

        [XmlElement(ElementName = "PrecioUnitarioItem", Order = 12)]
        public decimal PrecioUnitario { get; set; }

        [XmlElement(ElementName = "DescuentoMonto", Order = 13)]
        public decimal? DescuentoMonto { get; set; }
        [XmlIgnore]
        public bool DescuentoMontoSpecified => DescuentoMonto.HasValue;
        public bool ShouldSerializeDescuentoMonto() => DescuentoMonto.HasValue; // Items for 43 don't have this.

        [XmlArray(ElementName = "TablaSubDescuento", Order = 14)]
        [XmlArrayItem(ElementName = "SubDescuento")]
        public List<SubDescuento>? TablaSubDescuento { get; set; }
        public bool ShouldSerializeTablaSubDescuento() => TablaSubDescuento != null;

        [XmlElement(ElementName = "RecargoMonto", Order = 15)]
        public decimal? RecargoMonto { get; set; }
        [XmlIgnore]
        public bool RecargoMontoSpecified => RecargoMonto.HasValue;
        public bool ShouldSerializeRecargoMonto() => RecargoMonto.HasValue;

        [XmlArray(ElementName = "TablaSubRecargo", Order = 16)]
        [XmlArrayItem(ElementName = "SubRecargo")]
        public List<SubRecargo>? TablaSubRecargo { get; set; }
        public bool ShouldSerializeTablaSubRecargo() => TablaSubRecargo != null;

        [XmlElement(ElementName = "OtraMonedaDetalle", Order = 17)]
        public OtraMonedaDetalle? OtraMonedaDetalle { get; set; }

        [XmlElement(ElementName = "MontoItem", Order = 18)]
        public decimal MontoItem { get; set; }
    }

    public class CodigoItem
    {
        [XmlElement(ElementName = "TipoCodigo", Order = 1)]
        public string TipoCodigo { get; set; } = string.Empty;

        [XmlElement(ElementName = "CodigoItem", Order = 2)]
        public string Codigo { get; set; } = string.Empty;
    }

    public class Retencion
    {
        [XmlElement(ElementName = "IndicadorAgenteRetencionoPercepcion", Order = 1)]
        public int IndicadorAgenteRetencionoPercepcion { get; set; }

        [XmlElement(ElementName = "MontoITBISRetenido", Order = 2)]
        public decimal? MontoITBISRetenido { get; set; }
        [XmlIgnore]
        public bool MontoITBISRetenidoSpecified => MontoITBISRetenido.HasValue;

        [XmlElement(ElementName = "MontoISRRetenido", Order = 3)]
        public decimal? MontoISRRetenido { get; set; }
        [XmlIgnore]
        public bool MontoISRRetenidoSpecified => MontoISRRetenido.HasValue;
    }

    public class SubDescuento
    {
        [XmlElement(ElementName = "TipoSubDescuento", Order = 1)]
        public string TipoSubDescuento { get; set; } = string.Empty;

        [XmlElement(ElementName = "SubDescuentoPorcentaje", Order = 2)]
        public decimal? SubDescuentoPorcentaje { get; set; }
        [XmlIgnore]
        public bool SubDescuentoPorcentajeSpecified => SubDescuentoPorcentaje.HasValue;

        [XmlElement(ElementName = "MontoSubDescuento", Order = 3)]
        public decimal? MontoSubDescuento { get; set; }
        [XmlIgnore]
        public bool MontoSubDescuentoSpecified => MontoSubDescuento.HasValue;
    }

    public class SubRecargo
    {
        [XmlElement(ElementName = "TipoSubRecargo", Order = 1)]
        public string TipoSubRecargo { get; set; } = string.Empty;

        [XmlElement(ElementName = "SubRecargoPorcentaje", Order = 2)]
        public decimal? SubRecargoPorcentaje { get; set; }
        [XmlIgnore]
        public bool SubRecargoPorcentajeSpecified => SubRecargoPorcentaje.HasValue;

        [XmlElement(ElementName = "MontoSubRecargo", Order = 3)]
        public decimal? MontoSubRecargo { get; set; }
        [XmlIgnore]
        public bool MontoSubRecargoSpecified => MontoSubRecargo.HasValue;
    }

    public class OtraMonedaDetalle
    {
        [XmlElement(ElementName = "PrecioOtraMoneda", Order = 1)]
        public decimal? PrecioOtraMoneda { get; set; }
        [XmlIgnore]
        public bool PrecioOtraMonedaSpecified => PrecioOtraMoneda.HasValue;

        [XmlElement(ElementName = "DescuentoOtraMoneda", Order = 2)]
        public decimal? DescuentoOtraMoneda { get; set; }
        [XmlIgnore]
        public bool DescuentoOtraMonedaSpecified => DescuentoOtraMoneda.HasValue;

        [XmlElement(ElementName = "RecargoOtraMoneda", Order = 3)]
        public decimal? RecargoOtraMoneda { get; set; }
        [XmlIgnore]
        public bool RecargoOtraMonedaSpecified => RecargoOtraMoneda.HasValue;

        [XmlElement(ElementName = "MontoItemOtraMoneda", Order = 4)]
        public decimal? MontoItemOtraMoneda { get; set; }
        [XmlIgnore]
        public bool MontoItemOtraMonedaSpecified => MontoItemOtraMoneda.HasValue;
    }

    public class Subtotales
    {
        [XmlElement(ElementName = "Subtotal", Order = 1)]
        public List<Subtotal> Items { get; set; } = new();
    }

    public class Subtotal
    {
        [XmlElement(ElementName = "NumeroSubTotal", Order = 1)]
        public int NumeroSubTotal { get; set; }

        [XmlElement(ElementName = "DescripcionSubtotal", Order = 2)]
        public string? DescripcionSubtotal { get; set; }

        [XmlElement(ElementName = "Orden", Order = 3)]
        public int Orden { get; set; }

        [XmlElement(ElementName = "SubTotalMontoGravadoTotal", Order = 4)]
        public decimal? SubTotalMontoGravadoTotal { get; set; }
        [XmlIgnore]
        public bool SubTotalMontoGravadoTotalSpecified => SubTotalMontoGravadoTotal.HasValue;

        [XmlElement(ElementName = "SubTotalMontoGravadoI1", Order = 5)]
        public decimal? SubTotalMontoGravadoI1 { get; set; }
        [XmlIgnore]
        public bool SubTotalMontoGravadoI1Specified => SubTotalMontoGravadoI1.HasValue;

        [XmlElement(ElementName = "SubTotalMontoGravadoI2", Order = 6)]
        public decimal? SubTotalMontoGravadoI2 { get; set; }
        [XmlIgnore]
        public bool SubTotalMontoGravadoI2Specified => SubTotalMontoGravadoI2.HasValue;

        [XmlElement(ElementName = "SubTotalMontoGravadoI3", Order = 7)]
        public decimal? SubTotalMontoGravadoI3 { get; set; }
        [XmlIgnore]
        public bool SubTotalMontoGravadoI3Specified => SubTotalMontoGravadoI3.HasValue;

        [XmlElement(ElementName = "SubTotaITBIS", Order = 8)]
        public decimal? SubTotaITBIS { get; set; }
        [XmlIgnore]
        public bool SubTotaITBISSpecified => SubTotaITBIS.HasValue;

        [XmlElement(ElementName = "SubTotaITBIS1", Order = 9)]
        public decimal? SubTotaITBIS1 { get; set; }
        [XmlIgnore]
        public bool SubTotaITBIS1Specified => SubTotaITBIS1.HasValue;

        [XmlElement(ElementName = "SubTotaITBIS2", Order = 10)]
        public decimal? SubTotaITBIS2 { get; set; }
        [XmlIgnore]
        public bool SubTotaITBIS2Specified => SubTotaITBIS2.HasValue;

        [XmlElement(ElementName = "SubTotaITBIS3", Order = 11)]
        public decimal? SubTotaITBIS3 { get; set; }
        [XmlIgnore]
        public bool SubTotaITBIS3Specified => SubTotaITBIS3.HasValue;

        [XmlElement(ElementName = "SubTotalImpuestoAdicional", Order = 12)]
        public decimal? SubTotalImpuestoAdicional { get; set; }
        [XmlIgnore]
        public bool SubTotalImpuestoAdicionalSpecified => SubTotalImpuestoAdicional.HasValue;

        [XmlElement(ElementName = "SubTotalExento", Order = 13)]
        public decimal? SubTotalExento { get; set; }
        [XmlIgnore]
        public bool SubTotalExentoSpecified => SubTotalExento.HasValue;

        [XmlElement(ElementName = "MontoSubTotal", Order = 14)]
        public decimal MontoSubTotal { get; set; }

        [XmlElement(ElementName = "Lineas", Order = 15)]
        public int? Lineas { get; set; }
        [XmlIgnore]
        public bool LineasSpecified => Lineas.HasValue;
    }

    public class DescuentosORecargos
    {
        [XmlElement(ElementName = "DescuentoORecargo", Order = 1)]
        public List<DescuentoORecargo> Items { get; set; } = new();
    }

    public class DescuentoORecargo
    {
        [XmlElement(ElementName = "NumeroLinea", Order = 1)]
        public int NumeroLinea { get; set; }

        [XmlElement(ElementName = "TipoAjuste", Order = 2)]
        public string TipoAjuste { get; set; } = "D"; // D or R

        [XmlElement(ElementName = "IndicadorNorma1007", Order = 3)]
        public int? IndicadorNorma1007 { get; set; }
        [XmlIgnore]
        public bool IndicadorNorma1007Specified => IndicadorNorma1007.HasValue;

        [XmlElement(ElementName = "DescripcionDescuentooRecargo", Order = 4)]
        public string? DescripcionDescuentooRecargo { get; set; }

        [XmlElement(ElementName = "TipoValor", Order = 5)]
        public string? TipoValor { get; set; } // $ or %

        [XmlElement(ElementName = "ValorDescuentooRecargo", Order = 6)]
        public decimal? ValorDescuentooRecargo { get; set; }
        [XmlIgnore]
        public bool ValorDescuentooRecargoSpecified => ValorDescuentooRecargo.HasValue;

        [XmlElement(ElementName = "MontoDescuentooRecargo", Order = 7)]
        public decimal MontoDescuentooRecargo { get; set; }

        [XmlElement(ElementName = "MontoDescuentooRecargoOtraMoneda", Order = 8)]
        public decimal? MontoDescuentooRecargoOtraMoneda { get; set; }
        [XmlIgnore]
        public bool MontoDescuentooRecargoOtraMonedaSpecified => MontoDescuentooRecargoOtraMoneda.HasValue;

        [XmlElement(ElementName = "IndicadorFacturacionDescuentooRecargo", Order = 9)]
        public int? IndicadorFacturacion { get; set; }
        [XmlIgnore]
        public bool IndicadorFacturacionSpecified => IndicadorFacturacion.HasValue;
    }

    public class OtraMoneda
    {
        [XmlElement(ElementName = "TipoMoneda", Order = 1)]
        public string? TipoMoneda { get; set; }

        [XmlElement(ElementName = "TipoCambio", Order = 2)]
        public decimal? TipoCambio { get; set; }
        [XmlIgnore]
        public bool TipoCambioSpecified => TipoCambio.HasValue;

        [XmlElement(ElementName = "MontoGravadoTotalOtraMoneda", Order = 3)]
        public decimal? MontoGravadoTotalOtraMoneda { get; set; }
        [XmlIgnore]
        public bool MontoGravadoTotalOtraMonedaSpecified => MontoGravadoTotalOtraMoneda.HasValue;

        [XmlElement(ElementName = "MontoGravado1OtraMoneda", Order = 4)]
        public decimal? MontoGravado1OtraMoneda { get; set; }
        [XmlIgnore]
        public bool MontoGravado1OtraMonedaSpecified => MontoGravado1OtraMoneda.HasValue;

        [XmlElement(ElementName = "MontoGravado2OtraMoneda", Order = 5)]
        public decimal? MontoGravado2OtraMoneda { get; set; }
        [XmlIgnore]
        public bool MontoGravado2OtraMonedaSpecified => MontoGravado2OtraMoneda.HasValue;

        [XmlElement(ElementName = "MontoGravado3OtraMoneda", Order = 6)]
        public decimal? MontoGravado3OtraMoneda { get; set; }
        [XmlIgnore]
        public bool MontoGravado3OtraMonedaSpecified => MontoGravado3OtraMoneda.HasValue;

        [XmlElement(ElementName = "MontoExentoOtraMoneda", Order = 7)]
        public decimal? MontoExentoOtraMoneda { get; set; }
        [XmlIgnore]
        public bool MontoExentoOtraMonedaSpecified => MontoExentoOtraMoneda.HasValue;

        [XmlElement(ElementName = "TotalITBISOtraMoneda", Order = 8)]
        public decimal? TotalITBISOtraMoneda { get; set; }
        [XmlIgnore]
        public bool TotalITBISOtraMonedaSpecified => TotalITBISOtraMoneda.HasValue;

        [XmlElement(ElementName = "TotalITBIS1OtraMoneda", Order = 9)]
        public decimal? TotalITBIS1OtraMoneda { get; set; }
        [XmlIgnore]
        public bool TotalITBIS1OtraMonedaSpecified => TotalITBIS1OtraMoneda.HasValue;

        [XmlElement(ElementName = "TotalITBIS2OtraMoneda", Order = 10)]
        public decimal? TotalITBIS2OtraMoneda { get; set; }
        [XmlIgnore]
        public bool TotalITBIS2OtraMonedaSpecified => TotalITBIS2OtraMoneda.HasValue;

        [XmlElement(ElementName = "TotalITBIS3OtraMoneda", Order = 11)]
        public decimal? TotalITBIS3OtraMoneda { get; set; }
        [XmlIgnore]
        public bool TotalITBIS3OtraMonedaSpecified => TotalITBIS3OtraMoneda.HasValue;

        [XmlElement(ElementName = "MontoTotalOtraMoneda", Order = 12)]
        public decimal? MontoTotalOtraMoneda { get; set; }
        [XmlIgnore]
        public bool MontoTotalOtraMonedaSpecified => MontoTotalOtraMoneda.HasValue;
    }
}
