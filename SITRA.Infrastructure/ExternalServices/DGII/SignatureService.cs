using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Microsoft.Extensions.Options;
using XmlElement = System.Xml.XmlElement;
using SITRA.Core.Interfaces;
using SITRA.Core.DTOs.DGII;
using SITRA.Core.Models.Dgii;

namespace SITRA.Infrastructure.ExternalServices.DGII
{
    public class SignatureService : ISignatureService
    {
        private readonly DgiiSettings _settings;

        public SignatureService(IOptions<DgiiSettings> options)
        {
            _settings = options.Value;
        }

        public string SignXml(string xmlString)
        {
            string path = _settings.CertificatePath;
            string pass = _settings.CertificatePassword;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = false; 
            xmlDoc.LoadXml(xmlString);

            byte[] certBytes = System.IO.File.ReadAllBytes(path);
            var cert = new X509Certificate2(certBytes, pass, X509KeyStorageFlags.Exportable);

            // Create a SignedXml object
            SignedXml signedXml = new SignedXml(xmlDoc);

            // Assign the private key to the SignedXml object
            // To completely bypass IIS "Keyset does not exist" we detach the key from Windows CNG store:
            using System.Security.Cryptography.RSA rsaOriginal = cert.GetRSAPrivateKey();
            System.Security.Cryptography.RSAParameters rsaParams = rsaOriginal.ExportParameters(true);
            
            System.Security.Cryptography.RSA detachedRsa = System.Security.Cryptography.RSA.Create();
            detachedRsa.ImportParameters(rsaParams);
            
            signedXml.SigningKey = detachedRsa;
            
            // As required by DGII documentation (SHA-256)
            signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

            // Create a reference to be signed (entire document)
            Reference reference = new Reference();
            reference.Uri = "";

            // Add an enveloped transformation to the reference
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);
            
            // Required by DGII
            reference.DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";
            
            signedXml.AddReference(reference);

            // Add the KeyInfo object (public key and certificate info)
            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(cert));
            signedXml.KeyInfo = keyInfo;

            // Compute the signature
            signedXml.ComputeSignature();

            // Append the signature to the XML document
            XmlElement xmlDigitalSignature = signedXml.GetXml();
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));

            return xmlDoc.OuterXml;
        }
        
        public string SignAuthenticationSeed(string seedXmlString)
        {
            return SignXml(seedXmlString);
        }
    }
}
