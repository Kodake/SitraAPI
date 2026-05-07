namespace SITRA.Core.Interfaces
{
    public interface ISignatureService
    {
        string SignXml(string xmlString);
        string SignAuthenticationSeed(string seedXmlString);
    }
}
