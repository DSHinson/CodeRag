using CodeRag.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeRag.Cli.Events
{
    public sealed record InputKeyEvent(ConsoleKeyInfo Key) : IEvent;
}
