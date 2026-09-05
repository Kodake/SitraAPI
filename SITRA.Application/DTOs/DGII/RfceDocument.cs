using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SITRA.Core.DTOs.DGII
{
    [XmlRoot(ElementName = "RFCE")]
    public class RfceDocument
    {
        [XmlElement(ElementName = "Encabezado", Order = 1)]
        public RfceEncabezado Encabezado { get; set; } = new();
    }

    public class RfceEncabezado
    {
        [XmlElement(ElementName = "Version", Order = 1)]
        public string Version { get; set; } = "1.0";

        [XmlElement(ElementName = "IdDoc", Order = 2)]
        public RfceIdDoc IdDoc { get; set; } = new();

        [XmlElement(ElementName = "Emisor", Order = 3)]
        public RfceEmisor Emisor { get; set; } = new();

        [XmlElement(ElementName = "Comprador", Order = 4)]
        public RfceComprador Comprador { get; set; } = new();

        [XmlElement(ElementName = "Totales", Order = 5)]
        public RfceTotales Totales { get; set; } = new();

        [XmlElement(ElementName = "CodigoSeguridadeCF", Order = 6)]
        public string CodigoSeguridadeCF { get; set; } = string.Empty;
    }

    public class RfceIdDoc
    {
        [XmlElement(ElementName = "TipoeCF", Order = 1)]
        public string TipoeCF { get; set; } = "32";

        [XmlElement(ElementName = "eNCF", Order = 2)]
        public string eNCF { get; set; } = string.Empty;

        [XmlElement(ElementName = "TipoIngresos", Order = 3)]
        public string TipoIngresos { get; set; } = "01";

        [XmlElement(ElementName = "TipoPago", Order = 4)]
        public string TipoPago { get; set; } = "1";

        [XmlElement(ElementName = "TablaFormasPago", Order = 5)]
        public RfceTablaFormasPago? TablaFormasPago { get; set; }
    }

    public class RfceTablaFormasPago
    {
        [XmlElement(ElementName = "FormaDePago")]
        public List<RfceFormaDePago> FormaDePago { get; set; } = new();
    }

    public class RfceFormaDePago
    {
        [XmlElement(ElementName = "FormaPago", Order = 1)]
        public string? FormaPago { get; set; }

        [XmlElement(ElementName = "MontoPago", Order = 2)]
        public decimal? MontoPago { get; set; }
        [XmlIgnore]
        public bool MontoPagoSpecified => MontoPago.HasValue;
    }

    public class RfceEmisor
    {
        [XmlElement(ElementName = "RNCEmisor", Order = 1)]
        public string RNCEmisor { get; set; } = string.Empty;

        [XmlElement(ElementName = "RazonSocialEmisor", Order = 2)]
        public string RazonSocialEmisor { get; set; } = string.Empty;

        [XmlElement(ElementName = "FechaEmision", Order = 3)]
        public string FechaEmision { get; set; } = string.Empty;
    }

    public class RfceComprador
    {
        [XmlElement(ElementName = "RNCComprador", Order = 1)]
        public string? RNCComprador { get; set; }

        [XmlElement(ElementName = "IdentificadorExtranjero", Order = 2)]
        public string? IdentificadorExtranjero { get; set; }

        [XmlElement(ElementName = "RazonSocialComprador", Order = 3)]
        public string? RazonSocialComprador { get; set; }
    }

    public class RfceTotales
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

        [XmlElement(ElementName = "TotalITBIS", Order = 6)]
        public decimal? TotalITBIS { get; set; }
        [XmlIgnore]
        public bool TotalITBISSpecified => TotalITBIS.HasValue;

        [XmlElement(ElementName = "TotalITBIS1", Order = 7)]
        public decimal? TotalITBIS1 { get; set; }
        [XmlIgnore]
        public bool TotalITBIS1Specified => TotalITBIS1.HasValue;

        [XmlElement(ElementName = "TotalITBIS2", Order = 8)]
        public decimal? TotalITBIS2 { get; set; }
        [XmlIgnore]
        public bool TotalITBIS2Specified => TotalITBIS2.HasValue;

        [XmlElement(ElementName = "TotalITBIS3", Order = 9)]
        public decimal? TotalITBIS3 { get; set; }
        [XmlIgnore]
        public bool TotalITBIS3Specified => TotalITBIS3.HasValue;

        [XmlElement(ElementName = "MontoImpuestoAdicional", Order = 10)]
        public decimal? MontoImpuestoAdicional { get; set; }
        [XmlIgnore]
        public bool MontoImpuestoAdicionalSpecified => MontoImpuestoAdicional.HasValue;

        [XmlArray(ElementName = "ImpuestosAdicionales", Order = 11)]
        [XmlArrayItem(ElementName = "ImpuestoAdicional")]
        public List<RfceImpuestoAdicional>? ImpuestosAdicionales { get; set; }

        [XmlElement(ElementName = "MontoTotal", Order = 12)]
        public decimal MontoTotal { get; set; }

        [XmlElement(ElementName = "MontoNoFacturable", Order = 13)]
        public decimal? MontoNoFacturable { get; set; }
        [XmlIgnore]
        public bool MontoNoFacturableSpecified => MontoNoFacturable.HasValue;

        [XmlElement(ElementName = "MontoPeriodo", Order = 14)]
        public decimal? MontoPeriodo { get; set; }
        [XmlIgnore]
        public bool MontoPeriodoSpecified => MontoPeriodo.HasValue;
    }

    public class RfceImpuestoAdicional
    {
        [XmlElement(ElementName = "TipoImpuesto", Order = 1)]
        public string? TipoImpuesto { get; set; }

        [XmlElement(ElementName = "MontoImpuestoSelectivoConsumoEspecifico", Order = 2)]
        public decimal? MontoImpuestoSelectivoConsumoEspecifico { get; set; }
        [XmlIgnore]
        public bool MontoImpuestoSelectivoConsumoEspecificoSpecified => MontoImpuestoSelectivoConsumoEspecifico.HasValue;

        [XmlElement(ElementName = "MontoImpuestoSelectivoConsumoAdvalorem", Order = 3)]
        public decimal? MontoImpuestoSelectivoConsumoAdvalorem { get; set; }
        [XmlIgnore]
        public bool MontoImpuestoSelectivoConsumoAdvaloremSpecified => MontoImpuestoSelectivoConsumoAdvalorem.HasValue;

        [XmlElement(ElementName = "OtrosImpuestosAdicionales", Order = 4)]
        public decimal? OtrosImpuestosAdicionales { get; set; }
        [XmlIgnore]
        public bool OtrosImpuestosAdicionalesSpecified => OtrosImpuestosAdicionales.HasValue;
    }
}
