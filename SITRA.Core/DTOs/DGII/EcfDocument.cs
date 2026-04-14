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
        
        [XmlElement(ElementName = "Totales", Order = 5)]
        public Totales Totales { get; set; } = new();
    }

    public class IdDoc
    {
        [XmlElement(ElementName = "TipoeCF", Order = 1)]
        public string TipoeCF { get; set; } = string.Empty;

        [XmlElement(ElementName = "eNCF", Order = 2)]
        public string ENCF { get; set; } = string.Empty;

        [XmlElement(ElementName = "FechaVencimientoSecuencia", Order = 3)]
        public string FechaVencimientoSecuencia { get; set; } = string.Empty;

        [XmlElement(ElementName = "IndicadorMontoGravado", Order = 4)]
        public string? IndicadorMontoGral { get; set; }

        [XmlElement(ElementName = "TipoIngresos", Order = 5)]
        public string TipoIngresos { get; set; } = "01";

        [XmlElement(ElementName = "TipoPago", Order = 6)]
        public string TipoPago { get; set; } = "1";
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

        [XmlElement(ElementName = "CorreoEmisor", Order = 8)]
        public string? CorreoEmisor { get; set; }

        [XmlElement(ElementName = "WebSite", Order = 9)]
        public string? WebSite { get; set; }

        [XmlElement(ElementName = "ActividadEconomica", Order = 10)]
        public string? ActividadEconomica { get; set; }

        [XmlElement(ElementName = "CodigoVendedor", Order = 11)]
        public string? CodigoVendedor { get; set; }

        [XmlElement(ElementName = "NumeroFacturaInterna", Order = 12)]
        public string? NumeroFacturaInterna { get; set; }

        [XmlElement(ElementName = "NumeroPedidoInterno", Order = 13)]
        public string? NumeroPedidoInterno { get; set; }

        [XmlElement(ElementName = "ZonaVenta", Order = 14)]
        public string? ZonaVenta { get; set; }

        [XmlElement(ElementName = "RutaVenta", Order = 15)]
        public string? RutaVenta { get; set; }

        [XmlElement(ElementName = "InformacionAdicionalEmisor", Order = 16)]
        public string? InformacionAdicionalEmisor { get; set; }

        [XmlElement(ElementName = "FechaEmision", Order = 17)]
        public string FechaEmision { get; set; } = string.Empty;
    }
    
    // ... Comprador remains mostly same but I'll make sure it's nullable ...
    public class Comprador
    {
        [XmlElement(ElementName = "RNCComprador", Order = 1)]
        public string RNCComprador { get; set; } = string.Empty;

        [XmlElement(ElementName = "RazonSocialComprador", Order = 2)]
        public string RazonSocialComprador { get; set; } = string.Empty;

        [XmlElement(ElementName = "ContactoComprador", Order = 3)]
        public string? ContactoComprador { get; set; }

        [XmlElement(ElementName = "CorreoComprador", Order = 4)]
        public string? CorreoComprador { get; set; }

        [XmlElement(ElementName = "DireccionComprador", Order = 5)]
        public string? DireccionComprador { get; set; }

        [XmlElement(ElementName = "MunicipioComprador", Order = 6)]
        public string? MunicipioComprador { get; set; }

        [XmlElement(ElementName = "ProvinciaComprador", Order = 7)]
        public string? ProvinciaComprador { get; set; }

        [XmlElement(ElementName = "FechaEntrega", Order = 8)]
        public string? FechaEntrega { get; set; }

        [XmlElement(ElementName = "ContactoEntrega", Order = 9)]
        public string? ContactoEntrega { get; set; }

        [XmlElement(ElementName = "DireccionEntrega", Order = 10)]
        public string? DireccionEntrega { get; set; }

        [XmlElement(ElementName = "TelefonoAdicional", Order = 11)]
        public string? TelefonoAdicional { get; set; }

        [XmlElement(ElementName = "FechaOrdenCompra", Order = 12)]
        public string? FechaOrdenCompra { get; set; }

        [XmlElement(ElementName = "NumeroOrdenCompra", Order = 13)]
        public string? NumeroOrdenCompra { get; set; }

        [XmlElement(ElementName = "CodigoInternoComprador", Order = 14)]
        public string? CodigoInternoComprador { get; set; }

        [XmlElement(ElementName = "ResponsablePago", Order = 15)]
        public string? ResponsablePago { get; set; }

        [XmlElement(ElementName = "InformacionAdicionalComprador", Order = 16)]
        public string? InformacionAdicionalComprador { get; set; }
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

        [XmlElement(ElementName = "IndicadorFacturacion", Order = 2)]
        public int IndicadorFacturacion { get; set; } = 1;

        [XmlElement(ElementName = "NombreItem", Order = 3)]
        public string NombreItem { get; set; } = string.Empty;

        [XmlElement(ElementName = "IndicadorBienoServicio", Order = 4)]
        public int IndicadorBienoServicio { get; set; } = 1;

        [XmlElement(ElementName = "CantidadItem", Order = 5)]
        public decimal Cantidad { get; set; }

        [XmlElement(ElementName = "UnidadMedida", Order = 6)]
        public string? UnidadMedida { get; set; }

        [XmlElement(ElementName = "FechaElaboracion", Order = 7)]
        public string? FechaElaboracion { get; set; }

        [XmlElement(ElementName = "FechaVencimientoItem", Order = 8)]
        public string? FechaVencimientoItem { get; set; }

        [XmlElement(ElementName = "PrecioUnitarioItem", Order = 9)]
        public decimal PrecioUnitario { get; set; }

        [XmlElement(ElementName = "DescuentoMonto", Order = 10)]
        public decimal? DescuentoMonto { get; set; }
        [XmlIgnore]
        public bool DescuentoMontoSpecified => DescuentoMonto.HasValue;

        [XmlElement(ElementName = "RecargoMonto", Order = 11)]
        public decimal? RecargoMonto { get; set; }
        [XmlIgnore]
        public bool RecargoMontoSpecified => RecargoMonto.HasValue;

        [XmlElement(ElementName = "MontoItem", Order = 12)]
        public decimal MontoItem { get; set; }
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

        [XmlElement(ElementName = "MontoSubTotal", Order = 4)]
        public decimal MontoSubTotal { get; set; }
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

        [XmlElement(ElementName = "IndicadorFacturacionDescuentooRecargo", Order = 8)]
        public int? IndicadorFacturacion { get; set; }
        [XmlIgnore]
        public bool IndicadorFacturacionSpecified => IndicadorFacturacion.HasValue;
    }
}
