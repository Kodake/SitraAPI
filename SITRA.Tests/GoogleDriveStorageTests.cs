using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SITRA.Core.Models.Dgii;
using SITRA.Infrastructure.Services.Storage;
using Xunit;

namespace SITRA.Tests
{
    public class GoogleDriveStorageTests
    {
        [Fact]
        public async Task GoogleDrive_SaveAndReadXml_ShouldSucceed()
        {
            // Arrange
            string credentialsPath = Path.Combine(AppContext.BaseDirectory, "client_secret_995629422879-gag0qo622smmjpv11aiht1843btogb0o.apps.googleusercontent.com.json");
            if (!File.Exists(credentialsPath))
            {
                // Fallback to project folder
                credentialsPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "SITRA.API", "client_secret_995629422879-gag0qo622smmjpv11aiht1843btogb0o.apps.googleusercontent.com.json"));
            }

            Assert.True(File.Exists(credentialsPath), $"Credentials file not found at {credentialsPath}");

            var settings = new DgiiSettings
            {
                StorageProvider = "GoogleDrive",
                GoogleDriveFolderId = "1Y67Zptg_8nHlPr7BvB2GsuLqIZZ3-geU",
                GoogleDriveCredentialsPath = credentialsPath,
                SchemasPath = "Resources/Schemas"
            };

            var options = Options.Create(settings);
            var configuration = new ConfigurationBuilder().Build();
            var logger = NullLogger<GoogleDriveStorageService>.Instance;

            var storageService = new GoogleDriveStorageService(options, configuration, logger);

            string testRnc = "133002868";
            string docType = "32";
            string fileName = $"TEST_{Guid.NewGuid():N}.xml";
            string testXml = "<eCF><Encabezado><IdDoc><TipoeCF>32</TipoeCF><eNCF>E320000000001</eNCF></IdDoc></Encabezado></eCF>";

            // Act - Save
            string driveUrl = await storageService.SaveDocumentXmlAsync(testXml, testRnc, docType, fileName);

            // Assert - URL returned
            Assert.NotNull(driveUrl);
            Assert.Contains("drive.google.com", driveUrl);

            // Act - Read back via URL
            string? readXml = await storageService.ReadDocumentXmlFromUrlAsync(driveUrl);

            // Assert - Content matches
            Assert.NotNull(readXml);
            Assert.Equal(testXml, readXml);

            // Act - Read directly via RNC and docType
            string? readDirectXml = await storageService.ReadDocumentXmlAsync(testRnc, docType, fileName);
            Assert.NotNull(readDirectXml);
            Assert.Equal(testXml, readDirectXml);
        }
    }
}
