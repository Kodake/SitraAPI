using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SITRA.Core.DTOs.DGII
{
    [XmlRoot(ElementName = "ECF")]
    public class EcfDocument
    {
        [XmlIgnore] // Propiedad interna de SITRA para logueo de Test, el XML la ignora.
        public string TestCaseName { get; set; } = string.Empty;

        [XmlElement(ElementName = "Encabezado", Order = 1)]
        public Encabezado Encabezado { get; set; } = new();

        [XmlElement(ElementName = "DetallesItems", Order = 2)]
        public Detalles Detalles { get; set; } = new();
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
        public string IndicadorMontoGral { get; set; } = "0";

        [XmlElement(ElementName = "TipoIngresos", Order = 5)]
        public string TipoIngresos { get; set; } = "01";

        [XmlElement(ElementName = "TipoPago", Order = 6)]
        public string TipoPago { get; set; } = "1";
    }

    public class Emisor
    {
        [XmlElement(ElementName = "RNCEmisor", Order = 1)]
        public string RNCEmisor { get; set; } = string.Empty;

        [XmlElement(ElementName = "RazonSocialEmisor", Order = 2)]
        public string RazonSocial { get; set; } = string.Empty;

        [XmlElement(ElementName = "NombreComercial", Order = 3)]
        public string NombreComercial { get; set; } = null; // Omitir si está vacío para evadir error MinLength

        [XmlElement(ElementName = "Sucursal", Order = 4)]
        public string Sucursal { get; set; } = null;

        [XmlElement(ElementName = "DireccionEmisor", Order = 5)]
        public string DireccionEmisor { get; set; } = string.Empty;
        
        [XmlElement(ElementName = "Municipio", Order = 6)]
        public string Municipio { get; set; } = null;

        [XmlElement(ElementName = "Provincia", Order = 7)]
        public string Provincia { get; set; } = null;

        [XmlElement(ElementName = "FechaEmision", Order = 8)]
        public string FechaEmision { get; set; } = string.Empty;
    }

    public class Comprador
    {
        [XmlElement(ElementName = "RNCComprador", Order = 1)]
        public string RNCComprador { get; set; } = string.Empty;

        [XmlElement(ElementName = "RazonSocialComprador", Order = 2)]
        public string RazonSocialComprador { get; set; } = string.Empty;
    }
    
    public class Totales
    {
        [XmlElement(ElementName = "TotalITBIS", Order = 1)] // ITBIS va antes del MontoTotal (Línea 140 XSD)
        public decimal TotalITBIS { get; set; }

        [XmlElement(ElementName = "MontoTotal", Order = 2)] // MontoTotal (Línea 162 XSD)
        public decimal MontoTotal { get; set; }
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

        [XmlElement(ElementName = "PrecioUnitarioItem", Order = 6)]
        public decimal PrecioUnitario { get; set; }

        [XmlElement(ElementName = "MontoItem", Order = 7)]
        public decimal MontoItem { get; set; }
    }
}
