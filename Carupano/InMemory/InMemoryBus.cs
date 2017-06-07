using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carupano.InMemory
{
    using Messaging;
    public class InMemoryBus : 
        ICommandBus, 
        IEventBus
    {
        IAggregateManager _dispatcher;
        Action<object, long?> _eventHandler;
        Action<object> _commandHandler;

        public InMemoryBus(IAggregateManager dispatcher)
        {
            _dispatcher = dispatcher;
        }
        public void Publish(IEnumerable<Tuple<object, long>> evts)
        {
            foreach(var msg in evts)
            {
                Publish(msg.Item1, msg.Item2);
            }
        }

        public void Publish(object evt, long? seq)
        {
            _eventHandler(evt, seq);

        }
        public void Publish(object evt, long seq)
        {
            _eventHandler(evt, seq);
        }

        public void Publish(object o)
        {
            _eventHandler(o, null);
        }


        public async Task Send(object cmd)
        {
            await new Task(() =>
            {
                _dispatcher.ExecuteCommand(cmd);
            });
        }

        public void SetCommandHandler(Action<object> handler)
        {
            _commandHandler = handler;
        }

        public void SetEventHandler(Action<object, long?> handler)
        {
            _eventHandler = handler;
        }
    }
}
