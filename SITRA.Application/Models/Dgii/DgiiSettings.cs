namespace SITRA.Core.Models.Dgii
{
    public class DgiiSettings
    {
        public string CertificatePath { get; set; } = string.Empty;
        public string CertificatePassword { get; set; } = string.Empty;
        public string TestFilePath { get; set; } = string.Empty;
        public string TestFilePath2 { get; set; } = string.Empty;
        public string SchemasPath { get; set; } = string.Empty;
        public string StoragePath { get; set; } = string.Empty;

        // Storage Provider & Settings
        public string StorageProvider { get; set; } = "GoogleDrive"; // "GoogleDrive", "Azure", "Local"
        public string GoogleDriveFolderId { get; set; } = string.Empty;
        public string GoogleDriveCredentialsPath { get; set; } = string.Empty;

        // Azure Storage Settings
        public bool UseAzureStorage { get; set; } = false;
        public string AzureContainerName { get; set; } = "dgii-assets";
        public string Environment { get; set; } = "QA"; // Prefix for blob paths
        
        // Endpoints
        public string BaseUrl { get; set; } = string.Empty;
        public string SemillaUrl { get; set; } = string.Empty;
        public string AutenticacionUrl { get; set; } = string.Empty;
        public string RecepcionUrl { get; set; } = string.Empty;
        public string ConsultaUrl { get; set; } = string.Empty;
        public string RecepcionResumenUrl { get; set; } = string.Empty;
        public string AprobacionComercialUrl { get; set; } = string.Empty;
        public string ConsultaTimbreUrl { get; set; } = string.Empty;
        public string ConsultaTimbreFCUrl { get; set; } = string.Empty;
        
        // Defaults
        public string DefaultRNC { get; set; } = string.Empty;
        public string DefaultRazonSocial { get; set; } = string.Empty;
        public string DefaultDireccion { get; set; } = string.Empty;
        public string DefaultFechaVencimientoSecuencia { get; set; } = string.Empty;
    }
}
