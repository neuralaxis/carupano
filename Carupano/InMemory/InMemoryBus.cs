using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carupano.InMemory
{
    public class InMemoryBus : 
        ICommandBus, 
        IEventBus
    {
        IAggregateManager Dispatcher;
        Action<object, long> _eventHandler;
        Action<object> _commandHandler;
        long _eventStart;

        public void Listen(Action<object, long> handler, long start)
        {
            _eventHandler = handler;
            _eventStart = start;
        }

        public void Listen(Action<object> handler)
        {
            _commandHandler = handler;
        }

        public void Publish<T>(T evt)
        {
            _eventHandler(evt, 0); //TODO: not 0
        }

        public void Publish<T>(IEnumerable<T> evts)
        {
            foreach (var evt in evts)
                _eventHandler(evt, 0); //TODO: not 0.
        }

        public void Send<T>(T message)
        {
            _commandHandler(message);
        }
    }
}
