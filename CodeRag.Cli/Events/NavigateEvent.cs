using CodeRag.Cli.Screens;
using CodeRag.Common.Events;

namespace CodeRag.Cli.Events
{
    internal sealed class NavigateEvent :IEvent
    {
        public NavigateEvent(ScreenDefs.ScreenIds screenId)
        {
            ScreenId = screenId;
        }

        public ScreenDefs.ScreenIds ScreenId { get; }
    }
}
