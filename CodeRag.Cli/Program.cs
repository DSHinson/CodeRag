using CodeRag.Cli.Events;
using CodeRag.Cli.Input;
using CodeRag.Cli.RenderLoop;
using CodeRag.Cli.Screens;
using CodeRag.Cli.Screens.Components;
using CodeRag.Common.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static CodeRag.Cli.Screens.Defs;

namespace CodeRag
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                            .ConfigureServices(services =>
                            {
                                services.AddSingleton<IEventBus, InMemoryEventBus>();
                                services.AddScreenComponents();
                                services.AddScreens();
                                services.AddHostedService<RenderLoop>();
                                services.AddHostedService<InputReaderService>();
                            })
                            .Build();

            var eventBus = host.Services.GetRequiredService<IEventBus>();
            var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
            lifetime.ApplicationStarted.Register(() =>
            {
                eventBus.Publish(new NavigateEvent(ScreenIds.WelcomeScreen));
            });

            host.Run();
        }
    }
}