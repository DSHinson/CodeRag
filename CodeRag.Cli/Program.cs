using CodeRag.bl.Indexing;
using CodeRag.Cli.Events;
using CodeRag.Cli.Input;
using CodeRag.Cli.RenderLoop;
using CodeRag.Cli.Screens;
using CodeRag.Cli.Screens.Components;
using CodeRag.Common;
using CodeRag.Common.Events;
using CodeRag.Common.Results;
using CodeRag.db.Migrations;
using CodeRag.db.Store;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SQLitePCL;
using static CodeRag.Cli.Screens.ScreenDefs;
using static CodeRag.Common.Results.Result;

namespace CodeRag
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Batteries_V2.Init();

            var directory = Path.GetDirectoryName(LookUps.StorageLocations.DbPath)!;
            Directory.CreateDirectory(directory);
            var host = Host.CreateDefaultBuilder(args)
                            .ConfigureServices(services =>
                            {
                                services.AddSingleton<IEventBus, InMemoryEventBus>();
                                services.AddScreenComponents();
                                services.AddScreens();
                                services.AddHostedService<RenderLoop>();
                                services.AddHostedService<InputReaderService>();
                                services.AddSingleton<ISqlStore>(_ =>
                                {
                                    return new SqlliteStore(LookUps.StorageLocations.DbPath);
                                });
                                services.AddSingleton<IRepositoryScanner, RepositoryScanner>();
                                services.AddCodeRagMigrations($"Data Source={LookUps.StorageLocations.DbPath}");
                                services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();
                            })
                            .Build();

            var eventBus = host.Services.GetRequiredService<IEventBus>();
            var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
            var dbInit = host.Services.GetRequiredService<IDatabaseInitializer>();
            Result initResult = dbInit.Initialize();
            Console.Clear();
            if (initResult is Failure  dbFail)
            {
                Console.WriteLine($"Failed to initialize database: {dbFail.FailureMessage}");
                return; // don't start the render loop with a broken DB
            }
            lifetime.ApplicationStarted.Register(() =>
            {
                eventBus.Publish(new NavigateEvent(ScreenIds.WelcomeScreen));
            });

            host.Run();
        }
    }
}