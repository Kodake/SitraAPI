namespace SITRA.Core.Interfaces
{
    public interface ISignatureService
    {
        string SignXml(string xmlString, string certPath, string certPassword);
        string SignAuthenticationSeed(string seedXmlString, string certPath, string certPassword);
    }
}
