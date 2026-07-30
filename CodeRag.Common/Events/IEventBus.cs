using System;
using System.Collections.Generic;
using System.Text;

namespace CodeRag.Common.Events
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent evt) where TEvent : IEvent;

        IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent;
    }
}
