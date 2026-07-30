using CodeRag.Cli.Events;
using CodeRag.Cli.Screens;
using CodeRag.Common.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using static CodeRag.Cli.Screens.Defs;

namespace CodeRag.Cli.RenderLoop
{
    internal sealed class RenderLoop : BackgroundService
    {
        private readonly Channel<IEvent> _localChannel = Channel.CreateUnbounded<IEvent>();
        private readonly List<IDisposable> _subscriptions = new();
        private readonly IReadOnlyDictionary<ScreenIds, IScreen> _screens;
        private readonly ILogger<RenderLoop> _logger;
        private IScreen? _activeScreen;

        public RenderLoop(IEventBus eventBus, IEnumerable<IScreen> screens, ILogger<RenderLoop> logger)
        {
            _screens = (screens ?? throw new ArgumentNullException(nameof(screens))).ToDictionary(s => s.ScreenId);
            _logger = (logger ?? throw new ArgumentNullException(nameof(logger)));

            _subscriptions.Add(eventBus.Subscribe<NavigateEvent>(Forward));
            _subscriptions.Add(eventBus.Subscribe<InputKeyEvent>(Forward));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var evt in _localChannel.Reader.ReadAllAsync(stoppingToken))
            {
                if (evt is NavigateEvent nav)
                {
                    HandleNavigate(nav);
                }
                if (evt is InputKeyEvent input)
                {
                    _activeScreen?.HandleEvent(input);
                }
            }
        }

        private void HandleNavigate(NavigateEvent nav)
        {
            if (!_screens.TryGetValue(nav.ScreenId, out var target))
            {
                _logger.LogError("No screen registered for {ScreenId}", nav.ScreenId);
                return;
            }

            var initResult = target.Initialize();

            Console.Clear();
            _activeScreen = target;

            var renderResult = _activeScreen.Render();
        }

        private Task Forward<TEvent>(TEvent evt) where TEvent : IEvent
        {
            _localChannel.Writer.TryWrite(evt);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _subscriptions.ForEach(s => s.Dispose());
            base.Dispose();
        }
    }
}
