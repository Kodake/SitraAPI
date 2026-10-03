using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Models.Dgii;

namespace SITRA.API.Endpoints.Storage
{
    public static class StorageEndpoints
    {
        public static IEndpointRouteBuilder MapStorageEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/storage/google-drive").WithTags("GoogleDriveStorage");

            group.MapGet("/auth", (HttpContext context, IConfiguration config) =>
            {
                string clientSecretPath = ResolveCredentialsPath(config);
                if (!File.Exists(clientSecretPath))
                {
                    return Results.BadRequest(new { error = $"Archivo client_secret no encontrado en {clientSecretPath}" });
                }

                var (clientId, _) = ReadClientCredentials(clientSecretPath);
                if (string.IsNullOrEmpty(clientId))
                {
                    return Results.BadRequest(new { error = "No se pudo leer el client_id del archivo JSON." });
                }

                string redirectUri = $"{context.Request.Scheme}://{context.Request.Host}/api/v1/storage/google-drive/callback";
                string scope = Uri.EscapeDataString("https://www.googleapis.com/auth/drive");
                string state = Guid.NewGuid().ToString("N");

                string authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                                 $"client_id={clientId}&" +
                                 $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                                 $"response_type=code&" +
                                 $"scope={scope}&" +
                                 $"access_type=offline&" +
                                 $"prompt=consent&" +
                                 $"state={state}";

                return Results.Redirect(authUrl);
            })
            .WithName("GoogleDriveAuth")
            .WithSummary("Inicia el flujo de autenticación de Google Drive");

            group.MapGet("/callback", async (HttpContext context, string? code, string? error, IConfiguration config, ILogger<StorageEndpointsLogger> logger) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    return Results.Content($"<h3>Error al autorizar con Google: {error}</h3>", "text/html");
                }

                if (string.IsNullOrEmpty(code))
                {
                    return Results.Content("<h3>Error: No se recibió el código de autorización de Google.</h3>", "text/html");
                }

                string clientSecretPath = ResolveCredentialsPath(config);
                var (clientId, clientSecret) = ReadClientCredentials(clientSecretPath);

                string redirectUri = $"{context.Request.Scheme}://{context.Request.Host}/api/v1/storage/google-drive/callback";

                using var httpClient = new HttpClient();
                var postParams = new Dictionary<string, string>
                {
                    { "code", code },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "redirect_uri", redirectUri },
                    { "grant_type", "authorization_code" }
                };

                var tokenResponse = await httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(postParams));
                string responseBody = await tokenResponse.Content.ReadAsStringAsync();

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    logger.LogError("Error intercambiando código por token de Google Drive: {Body}", responseBody);
                    return Results.Content($"<h3>Error al canjear el token: {responseBody}</h3>", "text/html");
                }

                // Guardar token en el formato que Google.Apis.Auth FileDataStore espera
                string tokenFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SITRA", "GoogleDriveToken");
                if (!Directory.Exists(tokenFolder)) Directory.CreateDirectory(tokenFolder);

                string tokenFilePath = Path.Combine(tokenFolder, "Google.Apis.Auth.OAuth2.Responses.TokenResponse-user");
                await File.WriteAllTextAsync(tokenFilePath, responseBody);

                logger.LogInformation("Token de Google Drive guardado exitosamente en {Path}", tokenFilePath);

                string html = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Google Drive Conectado</title>
    <style>
        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; display: flex; align-items: center; justify-content: center; height: 100vh; margin: 0; background: #0f172a; color: #f8fafc; }
        .card { background: #1e293b; padding: 2.5rem; border-radius: 1rem; box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.5); text-align: center; max-width: 480px; border: 1px solid #334155; }
        h1 { color: #38bdf8; font-size: 1.6rem; margin-bottom: 0.5rem; }
        p { color: #94a3b8; line-height: 1.6; }
        .success-icon { font-size: 3.5rem; margin-bottom: 1rem; color: #22c55e; }
    </style>
</head>
<body>
    <div class='card'>
        <div class='success-icon'>✓</div>
        <h1>¡Google Drive Conectado con Éxito!</h1>
        <p>SITRA ahora guardará automáticamente las facturas, XMLs y documentos en tu carpeta de Google Drive.</p>
        <p style='margin-top: 1.5rem; font-size: 0.9rem; color: #64748b;'>Ya puedes cerrar esta ventana y regresar al sistema.</p>
    </div>
</body>
</html>";

                return Results.Content(html, "text/html");
            })
            .WithName("GoogleDriveCallback")
            .WithSummary("Callback de OAuth de Google Drive");

            group.MapGet("/status", async (IDgiiStorageService storageService, IConfiguration config) =>
            {
                string tokenFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SITRA", "GoogleDriveToken");
                string tokenFilePath = Path.Combine(tokenFolder, "Google.Apis.Auth.OAuth2.Responses.TokenResponse-user");
                bool tokenExists = File.Exists(tokenFilePath);

                return Results.Ok(new
                {
                    provider = config["DgiiTest:StorageProvider"] ?? "GoogleDrive",
                    folderId = config["DgiiTest:GoogleDriveFolderId"],
                    isAuthorized = tokenExists,
                    tokenStoragePath = tokenFilePath
                });
            })
            .WithName("GoogleDriveStatus")
            .WithSummary("Consulta el estado de conexión de Google Drive");

            return endpoints;
        }

        private static string ResolveCredentialsPath(IConfiguration config)
        {
            string path = config["DgiiTest:GoogleDriveCredentialsPath"] 
                ?? config["Dgii:GoogleDriveCredentialsPath"] 
                ?? "client_secret_995629422879-gag0qo622smmjpv11aiht1843btogb0o.apps.googleusercontent.com.json";

            if (Path.IsPathRooted(path)) return path;

            string baseApp = Path.Combine(AppContext.BaseDirectory, path);
            if (File.Exists(baseApp)) return baseApp;

            string curDir = Path.Combine(Directory.GetCurrentDirectory(), path);
            if (File.Exists(curDir)) return curDir;

            return baseApp;
        }

        private static (string ClientId, string ClientSecret) ReadClientCredentials(string jsonPath)
        {
            try
            {
                string content = File.ReadAllText(jsonPath);
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                JsonElement clientElement = default;
                if (root.TryGetProperty("installed", out var installed))
                    clientElement = installed;
                else if (root.TryGetProperty("web", out var web))
                    clientElement = web;

                if (clientElement.ValueKind != JsonValueKind.Undefined)
                {
                    string clientId = clientElement.GetProperty("client_id").GetString() ?? "";
                    string clientSecret = clientElement.GetProperty("client_secret").GetString() ?? "";
                    return (clientId, clientSecret);
                }
            }
            catch { }

            return ("", "");
        }
    }

    public class StorageEndpointsLogger { }
}
