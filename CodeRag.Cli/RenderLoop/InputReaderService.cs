using CodeRag.Cli.Events;
using CodeRag.Common.Events;
using Microsoft.Extensions.Hosting;

namespace CodeRag.Cli.Input
{
    internal sealed class InputReaderService : BackgroundService
    {
        private readonly IEventBus _eventBus;

        public InputReaderService(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (Console.IsInputRedirected)
                return; // no interactive terminal (e.g. invoked as an MCP subprocess) — nothing to read

            while (!stoppingToken.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    _eventBus.Publish(new InputKeyEvent(key));
                }
                else
                {
                    await Task.Delay(15, stoppingToken);
                }
            }
        }
    }
}