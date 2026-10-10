using SITRA.Core.Helpers;
using SITRA.Core.Models.Dgii;
using Xunit;

namespace SITRA.Tests
{
    public class DgiiQrUrlHelperTests
    {
        [Fact]
        public void BuildDgiiQrUrl_FormatsEcfUrlWithLowercaseParametersAndCorrectFechaFirma()
        {
            // Arrange
            var metadata = new DgiiPdfMetadata
            {
                EmisorRnc = "130000001",
                CompradorRnc = "130000002",
                Encf = "e310000000001",
                FechaEmision = "10-10-2020",
                MontoTotal = 2.11m,
                FechaFirma = "10-10-2020 09:00:00",
                SecurityCode = "dcp79q",
                TipoEcf = "31"
            };

            string customUrl = "https://ecf.dgii.gov.do/testecf/consultatimbre";

            // Act
            string qrUrl = DgiiQrUrlHelper.BuildDgiiQrUrl(metadata, customUrl: customUrl);

            // Assert
            string expected = "https://ecf.dgii.gov.do/testecf/consultatimbre?rncemisor=130000001&rnccomprador=130000002&encf=E310000000001&fechaemision=10-10-2020&montototal=2.11&fechafirma=10-10-2020%2009:00:00&codigoseguridad=dcp79q";
            Assert.Equal(expected, qrUrl);
        }

        [Fact]
        public void BuildDgiiQrUrl_Tipo32GreaterThan250kUsesStandardUrl()
        {
            // Arrange
            var metadata = new DgiiPdfMetadata
            {
                EmisorRnc = "133002868",
                CompradorRnc = "00000000000",
                Encf = "E320000002018",
                FechaEmision = "13-08-2026",
                MontoTotal = 590000.00m,
                FechaFirma = "13-08-2026 22:29:15",
                SecurityCode = "BQazJB",
                TipoEcf = "32"
            };

            string customUrl = "https://ecf.dgii.gov.do/CerteCF/ConsultaTimbre";

            // Act
            string qrUrl = DgiiQrUrlHelper.BuildDgiiQrUrl(metadata, customUrl: customUrl);

            // Assert
            string expected = "https://ecf.dgii.gov.do/CerteCF/ConsultaTimbre?rncemisor=133002868&rnccomprador=00000000000&encf=E320000002018&fechaemision=13-08-2026&montototal=590000.00&fechafirma=13-08-2026%2022:29:15&codigoseguridad=BQazJB";
            Assert.Equal(expected, qrUrl);
        }

        [Fact]
        public void BuildDgiiQrUrl_Tipo32LessThanOrEqualTo250kUsesFcUrl()
        {
            // Arrange
            var metadata = new DgiiPdfMetadata
            {
                EmisorRnc = "133002868",
                CompradorRnc = "00000000000",
                Encf = "E320000002018",
                FechaEmision = "13-08-2026",
                MontoTotal = 1500.00m,
                FechaFirma = "13-08-2026 22:29:15",
                SecurityCode = "BQazJB",
                TipoEcf = "32"
            };

            string customFcUrl = "https://fc.dgii.gov.do/CerteCF/ConsultaTimbreFC";

            // Act
            string qrUrl = DgiiQrUrlHelper.BuildDgiiQrUrl(metadata, customFcUrl: customFcUrl);

            // Assert
            string expected = "https://fc.dgii.gov.do/CerteCF/ConsultaTimbreFC?rncemisor=133002868&rnccomprador=00000000000&encf=E320000002018&fechaemision=13-08-2026&montototal=1500.00&fechafirma=13-08-2026%2022:29:15&codigoseguridad=BQazJB";
            Assert.Equal(expected, qrUrl);
        }
    }
}
