using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHttpClient("SitraApiClient", client =>
        {
            var baseUrl = context.Configuration["SitraApi:BaseUrl"] ?? "http://localhost:5000";
            if (!baseUrl.EndsWith('/')) baseUrl += "/";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromMinutes(5);

            var apiKey = context.Configuration["SitraApi:SyncApiKey"] ?? "sitra-dgii-sync-secret-key-2026";
            client.DefaultRequestHeaders.Add("X-Sync-Api-Key", apiKey);
        });
    })
    .Build();

host.Run();
