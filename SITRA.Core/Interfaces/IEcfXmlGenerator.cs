using SITRA.Core.DTOs.DGII;

namespace SITRA.Core.Interfaces
{
    public interface IEcfXmlGenerator
    {
        string GenerateEcfXml(EcfDocument document);
        string GenerateRfceXml(RfceDocument document);
    }
}
