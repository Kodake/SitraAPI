namespace SITRA.Core.Interfaces
{
    public interface ISignatureService
    {
        string SignXml(string xmlString, byte[] certBytes, string certPassword);
        string SignAuthenticationSeed(string seedXmlString, byte[] certBytes, string certPassword);
        bool ValidateXmlSignature(string signedXmlString, string caCertPath);
    }
}
