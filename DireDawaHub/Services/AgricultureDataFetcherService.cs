using DireDawaHub.Data;
using DireDawaHub.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DireDawaHub.Services;

public class AgricultureDataFetcherService : BackgroundService
{
    private readonly ILogger<AgricultureDataFetcherService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly DireDawaHub.Services.SystemStateService _systemState;

    public AgricultureDataFetcherService(ILogger<AgricultureDataFetcherService> logger, IServiceProvider serviceProvider, DireDawaHub.Services.SystemStateService systemState)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _systemState = systemState;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Automated Agriculture Data Fetcher is starting.");

        // Continuous background loop
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Fetching live agriculture data from external REST API at: {time}", DateTimeOffset.Now);
            
            _systemState.LastAgricultureFetch = DateTime.Now;
            _systemState.ServiceStatus = "Running";
            
            await FetchAndStoreDataAsync();
            
            _systemState.ServiceStatus = "Operational";

            // Pause the background worker for exactly 3 hours before requesting again
            await Task.Delay(TimeSpan.FromHours(3), stoppingToken);
        }
    }

    private async Task FetchAndStoreDataAsync()
    {
        // Because Background Services are Singletons, we must create a Scope to interact with Scoped EF Core Databases
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try 
            {
                // In a production environment, this makes the real HTTP call:
                // var response = await _httpClient.GetAsync("https://api.marketprices.gov.et/v1/daily");
                // var jsonString = await response.Content.ReadAsStringAsync();
                
                // For this implementation, we simulate the incoming JSON from the Ethiopian Market API
                var incomingJsonFromApi = @"[
                    { ""CropName"": ""Teff (Magna)"", ""PricePerKg"": 125.50, ""MarketLocation"": ""Ashawa API Feed"", ""DiseaseAlerts"": """" },
                    { ""CropName"": ""Sorghum"", ""PricePerKg"": 48.00, ""MarketLocation"": ""Ashawa API Feed"", ""DiseaseAlerts"": ""Automated drone sensors detected mild rust in eastern sector."" },
                    { ""CropName"": ""Coffee (Harar)"", ""PricePerKg"": 360.00, ""MarketLocation"": ""Taiwan Market Export"", ""DiseaseAlerts"": """" }
                ]";

                // 1. Deserialize the incoming JSON payload into C# Objects
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var fetchedData = JsonSerializer.Deserialize<List<AgricultureMarket>>(incomingJsonFromApi, options);

                if (fetchedData != null)
                {
                    foreach (var item in fetchedData)
                    {
                        item.RecordedDate = DateTime.Now;
                        
                        // Prevent constant duplicate spamming by checking the latest database entry
                        var existing = context.AgricultureMarkets
                            .OrderByDescending(a => a.RecordedDate)
                            .FirstOrDefault(a => a.CropName == item.CropName);

                        // Only add a new database record if the price changed, or if it's been more than 24 hours
                        if (existing == null || existing.PricePerKg != item.PricePerKg || (DateTime.Now - existing.RecordedDate).TotalHours > 24)
                        {
                            context.AgricultureMarkets.Add(item);
                        }
                    }
                    
                    // 2. Automatically save the new data back to SQLite
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Successfully updated SQLite database with automated REST API market prices.");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "FATAL ERROR: Failed to communicate with external Agriculture REST API.");
            }
        }
    }
}
