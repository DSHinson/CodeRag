using CodeRag.Cli.Screens;
using CodeRag.Common.Events;

namespace CodeRag.Cli.Events
{
    internal sealed class NavigateEvent :IEvent
    {
        public NavigateEvent(Defs.ScreenIds screenId)
        {
            ScreenId = screenId;
        }

        public Defs.ScreenIds ScreenId { get; }
    }
}
