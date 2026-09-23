using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace SITRA.SyncJob
{
    public class SyncDgiiStatusTimerFunction
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SyncDgiiStatusTimerFunction> _logger;

        public SyncDgiiStatusTimerFunction(IHttpClientFactory httpClientFactory, ILogger<SyncDgiiStatusTimerFunction> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [Function("SyncDgiiStatusTimer")]
        public async Task RunTimer([TimerTrigger("%SyncJob:Schedule%")] TimerInfo myTimer)
        {
            _logger.LogInformation("Cron de sincronización DGII iniciado a las: {Time}", DateTimeOffset.UtcNow);

            await ExecuteSyncAsync();
        }

        [Function("ManualSyncDgiiStatus")]
        public async Task<HttpResponseData> RunHttpManual(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "manual-sync")] HttpRequestData req)
        {
            _logger.LogInformation("Disparador manual HTTP de sincronización DGII recibido.");

            var (success, resultString) = await ExecuteSyncAsync();

            var response = req.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            await response.WriteStringAsync(resultString);
            return response;
        }

        private async Task<(bool Success, string Result)> ExecuteSyncAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("SitraApiClient");

                _logger.LogInformation("Enviando petición a SITRA.API POST api/v1/logInvoices/sync-pending...");
                var response = await client.PostAsync("api/v1/logInvoices/sync-pending", null);

                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Sincronización completada exitosamente. Respuesta: {Body}", responseBody);
                    return (true, responseBody);
                }
                else
                {
                    _logger.LogError("Error al sincronizar con SITRA.API. Status: {Status}, Detalle: {Body}",
                        response.StatusCode, responseBody);
                    return (false, responseBody);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción no controlada durante la ejecución del job de sincronización DGII");
                return (false, $"{{\"error\":\"{ex.Message}\"}}");
            }
        }
    }
}
