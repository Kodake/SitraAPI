using System;
using SITRA.Core.Helpers;

class Program
{
    static void Main()
    {
        string xml = @"<?xml version=""1.0""?><ECF><Encabezado><Version>1.0</Version><IdDoc><TipoeCF>32</TipoeCF><eNCF>E320000000508</eNCF><IndicadorMontoGravado>0</IndicadorMontoGravado><TipoIngresos>01</TipoIngresos><TipoPago>1</TipoPago></IdDoc><Emisor><RNCEmisor>133556545</RNCEmisor><RazonSocialEmisor>SISTRA SRL</RazonSocialEmisor><DireccionEmisor>N/A</DireccionEmisor><FechaEmision>22-05-2026</FechaEmision></Emisor><Comprador><RNCComprador>131880681</RNCComprador><RazonSocialComprador>DOCUMENTOS ELECTRONICOS DE 02</RazonSocialComprador><CorreoComprador>DOCUMENTOSDE02@GMAIL.COM</CorreoComprador><DireccionComprador>30 DE MARZO</DireccionComprador><MunicipioComprador>010101</MunicipioComprador><ProvinciaComprador>010000</ProvinciaComprador></Comprador><Totales><MontoGravadoTotal>100.00</MontoGravadoTotal><MontoGravadoI1>100.00</MontoGravadoI1><ITBIS1>18</ITBIS1><TotalITBIS>18.00</TotalITBIS><TotalITBIS1>18.00</TotalITBIS1><MontoTotal>118.00</MontoTotal></Totales></Encabezado><DetallesItems><Item><NumeroLinea>1</NumeroLinea><IndicadorFacturacion>1</IndicadorFacturacion><NombreItem>ACEITE DE SOYA</NombreItem><IndicadorBienoServicio>1</IndicadorBienoServicio><CantidadItem>1.00</CantidadItem><PrecioUnitarioItem>100.00</PrecioUnitarioItem><MontoItem>100.00</MontoItem></Item></DetallesItems><FechaHoraFirma>22-05-2026 16:21:03</FechaHoraFirma><Signature xmlns=""http://www.w3.org/2000/09/xmldsig#""><SignedInfo><CanonicalizationMethod Algorithm=""http://www.w3.org/TR/2001/REC-xml-c14n-20010315"" /><SignatureMethod Algorithm=""http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"" /><Reference URI=""""><Transforms><Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature"" /></Transforms><DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"" /><DigestValue>q1iUONCgNH9rCfDjb8IaBqHFLR6tAgHhl4LCHpOy42I=</DigestValue></Reference></SignedInfo><SignatureValue>ZO5QCb0y3n+5kLNT4AqYnG3jEOMgaom2QFSCTnSoXQoJm2L/hH/XqJhggAB6HoRprp6MNBeuxJ5YsKUbOHcREFr8w9cA9k+KDPWxCHuyr6/uB1z/5xCKGnyr2lV8pCZzo0rsn+pfblk1mVBLAn/kHH1ohODQkr5BV9yebUjnPFueY6h1CJ2ghJq8gvUsh3yg5UUi0UjLTZY2Si4WjU7oJjuK1q87HKZMk6lG5Bz1ya0vm6djkeJj6pWRpqTWYgMPjfwW9yDeoQMBvqREXA+zc6aphMg1O1mFLP5RYI8R5zIQ1XsODMimwpan/dQAvPcol2jcjlhCKsQw7mRnrTXTIw==</SignatureValue></Signature></ECF>";
        
        var meta = DgiiXmlMetadataReader.ParseXml(xml);
        Console.WriteLine($"FechaFirma: {meta?.FechaFirma}");
        Console.WriteLine($"SecurityCode: {meta?.SecurityCode}");
    }
}
