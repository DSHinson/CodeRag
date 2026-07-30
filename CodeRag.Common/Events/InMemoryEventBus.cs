using System.Collections.Concurrent;

namespace CodeRag.Common.Events
{
    public sealed class InMemoryEventBus : IEventBus
    {
        private readonly ConcurrentDictionary<Type, List<Func<IEvent, Task>>> _handlers = new();
        private readonly object _lock = new();

        public void Publish<TEvent>(TEvent evt) where TEvent : IEvent
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out var handlers))
            {
                return;
            }

            List<Func<IEvent, Task>> snapshot;
            lock (_lock) { snapshot = new List<Func<IEvent, Task>>(handlers); }

            foreach (var handler in snapshot)
            {
                _ = handler(evt); // fire-and-forget; see ordering note below
            }
        }

        public IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
        {
            Func<IEvent, Task> wrapped = e => handler((TEvent)e);

            lock (_lock)
            {
                var list = _handlers.GetOrAdd(typeof(TEvent), _ => new List<Func<IEvent, Task>>());
                list.Add(wrapped);
            }

            return new Subscription(() =>
            {
                lock (_lock)
                {
                    _handlers[typeof(TEvent)].Remove(wrapped);
                }
            });
        }

        private sealed class Subscription : IDisposable
        {
            private readonly Action _unsubscribe;
            public Subscription(Action unsubscribe) => _unsubscribe = unsubscribe;
            public void Dispose() => _unsubscribe();
        }
    }
}
