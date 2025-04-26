using EPOCH.Application.Factories;
using EPOCH.Application.Interfaces;
using EPOCH.Application.UseCases;
using EPOCH.Domain.Interfaces;
using EPOCH.Domain.Services;
using EPOCH.Infrastructure.Clients;
using EPOCH.Infrastructure.Persistence;
using EPOCH.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.IO;
using System.ServiceProcess;
using System.Windows;

namespace EPOCH.Presentation;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EPOCH");
        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }
        string dbPath = Path.Combine(directory, "localdb.db");

        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));
        services.AddSingleton<ISatelliteClassifier, SatelliteClassifier>();
        services.AddSingleton<ISatelliteTracker, SgpSatelliteTracker>();
        services.AddSingleton<ITleApiClient, TleApiClient>();
        services.AddSingleton<ISatelliteFactory, SatelliteFactory>();
        services.AddScoped<ISatelliteRepository, SatelliteRepository>();
        services.AddScoped<FetchAndStoreSatellitesIfDbEmptyUseCase>();

        var serviceProvider = services.BuildServiceProvider();

        var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Database.EnsureCreated();

        scope = serviceProvider.CreateScope();
        var fetchUseCase = serviceProvider.GetRequiredService<FetchAndStoreSatellitesIfDbEmptyUseCase>();

        Task.Run(async () =>
        {
            await fetchUseCase.ExecuteAsync();
        }).GetAwaiter().GetResult();

        base.OnStartup(e);
    }
}

